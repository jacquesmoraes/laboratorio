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

        public async Task<AuthResponseRecord> RegisterClientUserAsync ( RegisterClientUserDto dto )
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

            // TODO: You can trigger a notification service here to send the access code
            return BuildAuthResponse ( user, "client" );
        }

        public async Task<AuthResponseRecord> LoginAsync ( LoginDto dto )
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Invalid credentials.");

            if ( !user.IsActive )
                throw new UnauthorizedAccessException ( "Account is deactivated." );

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

        public async Task<string> RegenerateAccessCodeByClientIdAsync ( int clientId )
        {
            var user = await _userManager.Users
                .Where(u => u.ClientId == clientId && u.IsFirstLogin)
                .FirstOrDefaultAsync();

            if ( user is null )
                throw new NotFoundException ( "Client user not found or has already completed first access." );

            user.AccessCode = GenerateAccessCode();
            user.AccessCodeExpiresAt = DateTime.UtcNow.AddHours(24); // Always use UTC for consistency

            var result = await _userManager.UpdateAsync(user);
            if ( !result.Succeeded )
                throw new Exception ( "Failed to update access code." );

            _logger.LogInformation ( "Access code regenerated for client {ClientId} ({Email})", clientId, user.Email );

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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateAccessCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if ( !await _roleManager.RoleExistsAsync(roleName) )
                await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }
    }
}
