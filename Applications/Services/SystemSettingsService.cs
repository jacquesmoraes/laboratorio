using Applications.Contracts;
using Applications.Dtos.Settings;
using Applications.Records.Settings;
using Applications.Services;
using Core.FactorySpecifications;
using Core.Interfaces;
using Core.Models.LabSettings;
using Microsoft.Extensions.Logging;

public class SystemSettingsService (
    IGenericRepository<SystemSettings> repo,
    ILogger<SystemSettingsService> logger
    ) : GenericService<SystemSettings> ( repo ), ISystemSettingsService
{
    private readonly IGenericRepository<SystemSettings> _repo = repo;
    private readonly ILogger<SystemSettingsService> _logger = logger;

    public async Task<SystemSettings> GetAsync ( )
    {
        var settings = await _repo.GetByIdAsync(1, new SystemSettingsByIdSpec(1));
        return settings ?? throw new InvalidOperationException ( "Configurações do sistema não encontradas." );
    }

    public async Task UpdateAsync ( UpdateSystemSettingsDto dto )
    {
        var settings = await GetAsync();

        settings.LabName = dto.LabName;
        settings.Email = dto.Email;
        settings.Phone = dto.Phone;
        settings.CNPJ = dto.CNPJ;
        settings.FooterMessage = dto.FooterMessage;
        settings.LastUpdated = DateTime.UtcNow;

        MapAddress ( settings.Address, dto.LabAddressRecord );

        await _repo.UpdateAsync ( settings.Id, settings );
    }

    public async Task UpdateLogoFileNameAsync ( string newFileName, string logosDirectory )
    {
        var settings = await GetAsync();

        if ( !string.IsNullOrEmpty ( settings.LogoFileName ) )
        {
            var oldPath = Path.Combine(logosDirectory, settings.LogoFileName);
            if ( File.Exists ( oldPath ) )
            {
                try
                {
                    File.SetAttributes ( oldPath, FileAttributes.Normal ); // remove atributo de somente leitura
                    File.Delete ( oldPath );
                    _logger.LogInformation ( "Logo anterior removido: {File}", settings.LogoFileName );
                }
                catch ( Exception ex )
                {
                    _logger.LogError ( ex, "Erro ao remover logo anterior: {File}", settings.LogoFileName );
                    // Não lançar exceção — só logar e seguir com o upload
                }

            }
        }

        settings.LogoFileName = newFileName;
        settings.LastUpdated = DateTime.UtcNow;

        await _repo.UpdateAsync ( settings.Id, settings );
    }

    private static void MapAddress ( LabAddress destination, LabAddressRecord source )
    {
        destination.Street = source.Street;
        destination.Cep = source.Cep;
        destination.Number = source.Number;
        destination.Complement = source.Complement;
        destination.Neighborhood = source.Neighborhood;
        destination.City = source.City;
    }
}
