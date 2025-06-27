using Applications.Dtos.Identity;
using Applications.Identity;

namespace Applications.Contracts.Identity
{
    public interface IIdentityService
    {
        // REGISTRO
        Task<AuthResponseRecord> RegisterAdminUserAsync(RegisterAdminUserDto dto);
        Task<AuthResponseRecord> RegisterClientUserAsync(RegisterClientUserDto dto);

        // LOGIN
        Task<AuthResponseRecord> LoginAsync(LoginDto dto);

        // PRIMEIRO ACESSO (2 etapas)
        Task<bool> ValidateAccessCodeAsync(ValidateAccessCodeDto dto);
        Task<AuthResponseRecord> CompleteFirstAccessAsync(FirstAccessPasswordResetDto dto);
    }
}
