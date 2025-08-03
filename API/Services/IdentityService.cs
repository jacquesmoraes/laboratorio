

namespace API.Services
{
    public class IdentityService (
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration config,
        ILogger<IdentityService> logger ) : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IConfiguration _config = config;
        private readonly ILogger<IdentityService> _logger = logger;

        public async Task<AuthResponseRecord> RegisterAdminUserAsync ( RegisterAdminUserDto dto )
        {
            if ( await _userManager.FindByEmailAsync ( dto.Email ) is not null )
                throw new ConflictException ( "This email is already in use." );

            if ( dto.Password != dto.ConfirmPassword )
                throw new BadRequestException ( "Passwords do not match." );

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

        public async Task<ClientUserRegistrationResponseDto> RegisterClientUserAsync ( RegisterClientUserDto dto )
        {
            if ( await _userManager.FindByEmailAsync ( dto.Email ) is not null )
                throw new ConflictException ( "This email is already in use." );

            if ( dto.Password != dto.ConfirmPassword )
                throw new BadRequestException ( "Passwords do not match." );

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
                CreatedAt = DateTime.UtcNow,
            };

            var tempPassword = Guid.NewGuid().ToString("N")[..10];
            var result = await _userManager.CreateAsync(user, tempPassword);
            if ( !result.Succeeded )
                throw new BadRequestException ( "Error while registering client user." );

            await EnsureRoleExistsAsync ( "client" );
            await _userManager.AddToRoleAsync ( user, "client" );

            return new ClientUserRegistrationResponseDto
            {
                ExpiresAt = user.AccessCodeExpiresAt.Value,
                User = new ClientUserRecord
                {
                    UserId = user.Id,
                    Email = user.Email ?? "",
                    DisplayName = user.DisplayName,
                    Role = "client",
                    ClientId = user.ClientId.Value,
                    IsFirstLogin = user.IsFirstLogin,
                    AccessCode = user.AccessCode ?? ""
                }
            };
        }


        public async Task<Pagination<ClientUserListRecord>> GetClientUsersPaginatedAsync ( QueryParams parameters )
        {
            var query = _userManager.Users
        .Where(u => u.ClientId.HasValue) // Apenas usuários client
        .AsQueryable();

            // Aplicar filtros
            if ( !string.IsNullOrEmpty ( parameters.Search ) )
            {
                query = query.Where ( u =>
                    u.Email.Contains ( parameters.Search ) ||
                    u.DisplayName.Contains ( parameters.Search ) );
            }

            // Aplicar ordenação
            query = parameters.Sort?.ToLower ( ) switch
            {
                "email" => query.OrderBy ( u => u.Email ),
                "createdat" => query.OrderByDescending ( u => u.CreatedAt ),
                "lastlogin" => query.OrderByDescending ( u => u.LastLoginAt ),
                _ => query.OrderBy ( u => u.DisplayName )
            };

            var totalItems = await query.CountAsync();

            // Aplicar paginação
            var users = await query
        .Skip((parameters.PageNumber - 1) * parameters.PageSize)
        .Take(parameters.PageSize)
        .ToListAsync();

            var records = users.Select(u => new ClientUserListRecord
            {
                UserId = u.Id,
                ClientId = u.ClientId.Value,
                ClientName = u.DisplayName, // Nome do cliente
                Email = u.Email ?? "",
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                HasValidAccessCode = u.AccessCodeExpiresAt.HasValue &&
                           u.AccessCodeExpiresAt > DateTime.UtcNow
            }).ToList();

            return new Pagination<ClientUserListRecord> (
                parameters.PageNumber,
                parameters.PageSize,
                totalItems,
                records
            );
        }



        public async Task<ClientUserDetailsRecord> GetClientUserDetailsByUserIdAsync ( string userId )
        {
            var user = await _userManager.FindByIdAsync(userId)
        ?? throw new NotFoundException("Usuário não encontrado.");

            if ( !user.ClientId.HasValue )
                throw new BadRequestException ( "Usuário não é vinculado a um cliente." );

            return new ClientUserDetailsRecord
            {
                UserId = user.Id,
                ClientId = user.ClientId.Value,
                ClientName = user.DisplayName,
                Email = user.Email ?? "",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsFirstLogin = user.IsFirstLogin,
                AccessCode = user.AccessCode,
                AccessCodeExpiresAt = user.AccessCodeExpiresAt,
                IsAccessCodeValid = user.AccessCodeExpiresAt.HasValue && user.AccessCodeExpiresAt > DateTime.UtcNow,
                LoginHistory = null
            };
        }


        public async Task BlockClientUserAsync ( string userId )
        {
            var user = await _userManager.FindByIdAsync(userId)
        ?? throw new NotFoundException("User not found.");

            if ( !user.ClientId.HasValue )
                throw new BadRequestException ( "User is not a client user." );

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if ( !result.Succeeded )
                throw new BadRequestException ( "Failed to block user." );
        }

        public async Task UnblockClientUserAsync ( string userId )
        {
            var user = await _userManager.FindByIdAsync(userId)
        ?? throw new NotFoundException("User not found.");

            if ( !user.ClientId.HasValue )
                throw new BadRequestException ( "User is not a client user." );

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if ( !result.Succeeded )
                throw new BadRequestException ( "Failed to unblock user." );
        }

        public async Task<AuthResponseRecord> LoginAsync ( LoginDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if ( !user.IsActive )
                throw new UnauthorizedAccessException ( "Account is not activated." );

            if ( !await _userManager.CheckPasswordAsync ( user, dto.Password ) )
            {
                _logger?.LogWarning ( "Incorrect password for email: {Email}", dto.Email );
                throw new UnauthorizedAccessException ( "Invalid credentials." );
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync ( user );

            var roles = await _userManager.GetRolesAsync(user);
            return BuildAuthResponse ( user, roles.FirstOrDefault ( ) ?? "client" );
        }

        public async Task<AuthResponseRecord> CompleteFirstAccessAsync ( FirstAccessPasswordResetDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new NotFoundException("User not found.");

            // Validate if user is in first access
            if ( !user.IsFirstLogin || !user.AccessCodeExpiresAt.HasValue )
                throw new BadRequestException ( "User is not in first access state." );

            // Validate if the access code has expired
            if ( user.AccessCodeExpiresAt <= DateTime.UtcNow )
                throw new BadRequestException ( "The access code has expired. Request a new one from the administrator." );

            // Validate the access code
            if ( user.AccessCode != dto.AccessCode )
                throw new BadRequestException ( "Invalid access code." );

            if ( dto.NewPassword != dto.ConfirmNewPassword )
                throw new BadRequestException ( "Passwords do not match." );

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            if ( !resetResult.Succeeded )
                throw new BadRequestException ( "Failed to reset password." );

            user.AccessCode = null;
            user.AccessCodeExpiresAt = null;
            user.IsActive = true;
            user.IsFirstLogin = false;

            await _userManager.UpdateAsync ( user );

            var roles = await _userManager.GetRolesAsync(user);
            return BuildAuthResponse ( user, roles.FirstOrDefault ( ) ?? "client" );
        }

        public async Task ChangePasswordAsync ( string userId, ChangePasswordDto dto )
        {
            var user = await _userManager.FindByIdAsync(userId)
        ?? throw new UnauthorizedAccessException("User not found.");

            if ( dto.NewPassword != dto.ConfirmNewPassword )
                throw new BadRequestException ( "Passwords do not match." );

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if ( !result.Succeeded )
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException ( $"Failed to change password: {errors}" );
            }
        }

       public async Task<string> RegenerateAccessCodeByUserIdAsync(string userId)
{
    var user = await _userManager.FindByIdAsync(userId)
        ?? throw new NotFoundException("Usuário não encontrado.");

    if (!user.IsFirstLogin)
        throw new BadRequestException("Usuário já completou o primeiro acesso.");

    user.AccessCode = GenerateAccessCode();
    user.AccessCodeExpiresAt = DateTime.UtcNow.AddHours(24);

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
        throw new Exception("Falha ao atualizar o código de acesso.");

    _logger.LogInformation("Novo código de acesso gerado para o usuário {UserId} ({Email})", user.Id, user.Email);

    return user.AccessCode;
}




        // ---------- PRIVATE METHODS ----------

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
                IsFirstLogin = user.IsFirstLogin,
                AccessCode = user.AccessCode ?? string.Empty
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

            var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            var jwtIssuer = _config["Jwt:Issuer"] ?? "LabSystem";
            var jwtAudience = _config["Jwt:Audience"] ?? "LabSystemClient";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
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
