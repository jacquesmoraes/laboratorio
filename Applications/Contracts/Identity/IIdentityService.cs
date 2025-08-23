namespace Applications.Contracts.Identity
{
    public interface IIdentityService
    {
        // REGISTRO
        Task<AuthResponseRecord> RegisterAdminUserAsync ( RegisterAdminUserDto dto );
        Task<ClientUserRegistrationResponseDto> RegisterClientUserAsync ( RegisterClientUserDto dto );

        // LOGIN
        Task<AuthResponseRecord> LoginAsync ( LoginDto dto );

        Task ChangePasswordAsync ( string userId, ChangePasswordDto dto );


        Task<AuthResponseRecord> CompleteFirstAccessAsync ( FirstAccessPasswordResetDto dto );

        Task<string> RegenerateAccessCodeByUserIdAsync ( string userId );

        Task<string> ForgotPasswordAsync ( ForgotPasswordDto dto );
        Task<string> ResetPasswordAsync ( ResetPasswordDto dto );

        Task<Pagination<ClientUserListRecord>> GetClientUsersPaginatedAsync ( QueryParams parameters );
        Task<ClientUserDetailsRecord> GetClientUserDetailsByUserIdAsync ( string userId );
        Task BlockClientUserAsync ( string userId );
        Task UnblockClientUserAsync ( string userId );

    }
}
