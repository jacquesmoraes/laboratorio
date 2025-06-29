namespace Applications.Contracts
{
    public interface ISystemSettingsService : IGenericService<SystemSettings>
    {
        Task<SystemSettings> GetAsync ( );
        Task UpdateAsync ( UpdateSystemSettingsDto dto );
        Task UpdateLogoFileNameAsync ( string fileName, string logosDirectory );
    }
}
