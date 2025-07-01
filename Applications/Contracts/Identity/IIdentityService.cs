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

        Task<AuthResponseRecord> CompleteFirstAccessAsync(FirstAccessPasswordResetDto dto);

        Task<string> RegenerateAccessCodeByClientIdAsync(int clientId);


    }
}
