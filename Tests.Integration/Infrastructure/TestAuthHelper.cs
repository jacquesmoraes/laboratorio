using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infra.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Tests.Integration.Infrastructure
{
    public static class TestAuthHelper
    {
        private const string TestJwtKey = "SuperSecretKeyForTestingPurposesOnly12345678901234567890";
        
        public static string GenerateJwtToken(string userId, string email, string role, int? clientId = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Name, "Test User"),
                new(ClaimTypes.Role, role),
                new("clientId", clientId?.ToString() ?? ""),
                new("isFirstLogin", "false")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "LabSystem",
                audience: "LabSystemClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static HttpClient CreateAuthenticatedClient(this HttpClient client, string userId = "test-user-id", string email = "admin@sistema.com", string role = "admin", int? clientId = null)
        {
            var token = GenerateJwtToken(userId, email, role, clientId);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}

