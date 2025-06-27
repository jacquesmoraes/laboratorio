using Applications.Contracts.Identity;
using Applications.Dtos.Identity;
using Applications.Identity;
using Core.Exceptions;
using Infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService (
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration config,
            ILogger<IdentityService> logger )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthResponseRecord> RegisterAdminUserAsync ( RegisterAdminUserDto dto )
        {
            if ( await _userManager.FindByEmailAsync ( dto.Email ) is not null )
                throw new ConflictException ( "Este e-mail já está em uso." );

            if ( dto.Password != dto.ConfirmPassword )
                throw new BadRequestException ( "As senhas não coincidem." );

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                DisplayName = dto.DisplayName,
                IsActive = true,
                IsFirstLogin = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if ( !result.Succeeded )
                throw new BadRequestException ( string.Join ( ", ", result.Errors.Select ( e => e.Description ) ) );

            await EnsureRoleExistsAsync ( "admin" );
            await _userManager.AddToRoleAsync ( user, "admin" );

            return BuildAuthResponse ( user, "admin" );
        }

        public async Task<AuthResponseRecord> RegisterClientUserAsync ( RegisterClientUserDto dto )
        {
            if ( await _userManager.FindByEmailAsync ( dto.Email ) is not null )
                throw new ConflictException ( "Este e-mail já está em uso." );

            if ( dto.Password != dto.ConfirmPassword )
                throw new BadRequestException ( "As senhas não coincidem." );

            var accessCode = GenerateAccessCode();
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                DisplayName = dto.DisplayName,
                ClientId = dto.ClientId,
                AccessCode = accessCode,
                AccessCodeExpiresAt = DateTime.UtcNow.AddHours(24),
                IsActive = false,
                IsFirstLogin = true,
                CreatedAt = DateTime.UtcNow
            };

            var tempPassword = Guid.NewGuid().ToString("N")[..10];
            var result = await _userManager.CreateAsync(user, tempPassword);
            if ( !result.Succeeded )
                throw new BadRequestException ( "Erro ao registrar cliente." );

            await EnsureRoleExistsAsync ( "client" );
            await _userManager.AddToRoleAsync ( user, "client" );

            // Aqui você pode acionar um serviço de notificação para enviar o access code
            return BuildAuthResponse ( user, "client" );
        }

        public async Task<AuthResponseRecord> LoginAsync ( LoginDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

            if ( !user.IsActive )
                throw new UnauthorizedAccessException ( "Conta desativada." );

            if ( user == null )
            {
                _logger?.LogWarning ( "Tentativa de login com e-mail inválido: {Email}", dto.Email );
                throw new UnauthorizedAccessException ( "Credenciais inválidas." );
            }
            if ( !await _userManager.CheckPasswordAsync ( user, dto.Password ) )
            {
                _logger?.LogWarning ( "Senha incorreta para o e-mail: {Email}", dto.Email );
                throw new UnauthorizedAccessException ( "Credenciais inválidas." );
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync ( user );

            var roles = await _userManager.GetRolesAsync(user);
            return BuildAuthResponse ( user, roles.FirstOrDefault ( ) ?? "client" );
        }

        public async Task<bool> ValidateAccessCodeAsync ( ValidateAccessCodeDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if ( user == null || !user.IsFirstLogin || !user.AccessCodeExpiresAt.HasValue )
                return false;

            return user.AccessCode == dto.AccessCode &&
                   user.AccessCodeExpiresAt > DateTime.UtcNow;
        }

        public async Task<AuthResponseRecord> CompleteFirstAccessAsync ( FirstAccessPasswordResetDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if ( dto.NewPassword != dto.ConfirmNewPassword )
                throw new BadRequestException ( "As senhas não coincidem." );

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if ( !resetResult.Succeeded )
                throw new BadRequestException ( "Erro ao redefinir a senha." );

            user.AccessCode = null;
            user.AccessCodeExpiresAt = null;
            user.IsActive = true;
            user.IsFirstLogin = false;

            await _userManager.UpdateAsync ( user );

            var roles = await _userManager.GetRolesAsync(user);
            return BuildAuthResponse ( user, roles.FirstOrDefault ( ) ?? "client" );
        }

        // ---------- MÉTODOS PRIVADOS ----------

        private AuthResponseRecord BuildAuthResponse ( ApplicationUser user, string role )
        {
            var token = GenerateJwtToken(user, role);

            return new AuthResponseRecord ( token, new UserInfoRecord
            {
                UserId = user.Id,
                Email = user.Email ?? "",
                DisplayName = user.DisplayName,
                Role = role,
                ClientId = user.ClientId,
                IsFirstLogin = user.IsFirstLogin
            } );
        }

        private string GenerateJwtToken ( ApplicationUser user, string role )
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Name, user.DisplayName),
                new(ClaimTypes.Role, role),
                new("clientId", user.ClientId?.ToString() ?? ""),
                new("isFirstLogin", user.IsFirstLogin.ToString())
            };

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key não configurado.");
            var jwtIssuer = _config["Jwt:Issuer"] ?? "LabSystem";
            var jwtAudience = _config["Jwt:Audience"] ?? "LabSystemClient";


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler ( ).WriteToken ( token );
        }

        private static string GenerateAccessCode ( )
        {
            return Random.Shared.Next ( 100000, 999999 ).ToString ( );
        }

        private async Task EnsureRoleExistsAsync ( string roleName )
        {
            if ( !await _roleManager.RoleExistsAsync ( roleName ) )
                await _roleManager.CreateAsync ( new ApplicationRole { Name = roleName } );
        }
    }
}
