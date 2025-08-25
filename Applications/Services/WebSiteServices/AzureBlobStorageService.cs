using Applications.Settings;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace Applications.Services.WebSiteServices
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobStorageSettings _settings;

        public AzureBlobStorageService (
            BlobServiceClient blobServiceClient,
            IOptions<BlobStorageSettings> options )
        {
            _blobServiceClient = blobServiceClient;
            _settings = options.Value;

            if ( string.IsNullOrEmpty ( _settings.ContainerName ) )
                throw new InvalidOperationException ( "ContainerName não configurado" );
        }

        public async Task<string> UploadImageAsync ( FileUploadModel file )
        {
            if ( file?.Content == null || file.FileSize == 0 )
                throw new BadRequestException ( "Nenhum arquivo foi enviado." );

            // Validar tipo de arquivo
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if ( !allowedExtensions.Contains ( fileExtension ) )
                throw new BadRequestException ( "Tipo de arquivo não permitido. Use apenas: jpg, jpeg, png, gif, webp" );

            // Validar tamanho (máximo 5MB)
            if ( file.FileSize > 5 * 1024 * 1024 )
                throw new BadRequestException ( "Arquivo muito grande. Tamanho máximo: 5MB" );

            try
            {
                // Obter container
                var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                await containerClient.CreateIfNotExistsAsync ( PublicAccessType.Blob );

                // Gerar nome único para o arquivo
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var blobName = $"website-images/{uniqueFileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                // Upload do arquivo
                await blobClient.UploadAsync ( file.Content, overwrite: true );

                // Definir content type
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                };
                await blobClient.SetHttpHeadersAsync ( blobHttpHeaders );

                // Retornar URL da imagem
                var imageUrl = !string.IsNullOrEmpty(_settings.CdnUrl)
                    ? $"{_settings.CdnUrl}/{blobName}"
                    : blobClient.Uri.ToString();

                return imageUrl;
            }
            catch ( Azure.RequestFailedException ex ) when ( ex.Status == 409 )
            {
                throw new BadRequestException ( "Erro de conflito no Azure Storage. Tente novamente." );
            }
            catch ( Azure.RequestFailedException ex ) when ( ex.Status == 403 )
            {
                throw new UnauthorizedException ( "Acesso negado ao Azure Storage. Verifique as credenciais." );
            }
            catch ( Azure.RequestFailedException ex )
            {
                throw new BadRequestException ( $"Erro no Azure Storage: {ex.Message}" );
            }
            catch ( Exception ex )
            {
                throw new BadRequestException ( $"Erro inesperado: {ex.Message}" );
            }
        }

        public async Task<bool> DeleteImageAsync ( string imageUrl )
        {
            if ( string.IsNullOrEmpty ( imageUrl ) )
                throw new BadRequestException ( "URL da imagem é obrigatória." );

            try
            {
                // Extrair a chave do blob da URL
                var blobName = ExtractBlobNameFromUrl(imageUrl);
                if ( string.IsNullOrEmpty ( blobName ) )
                    throw new BadRequestException ( "URL da imagem inválida." );

                var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var response = await blobClient.DeleteIfExistsAsync();
                return response.Value;
            }
            catch ( Azure.RequestFailedException ex ) when ( ex.Status == 404 )
            {
                throw new NotFoundException ( "Arquivo não encontrado no Azure Storage." );
            }
            catch ( Azure.RequestFailedException ex ) when ( ex.Status == 403 )
            {
                throw new UnauthorizedException ( "Acesso negado ao Azure Storage. Verifique as credenciais." );
            }
            catch ( Azure.RequestFailedException ex )
            {
                throw new BadRequestException ( $"Erro no Azure Storage: {ex.Message}" );
            }
            catch ( BadRequestException )
            {
                throw; // Re-throw BadRequestException
            }
            catch ( NotFoundException )
            {
                throw; // Re-throw NotFoundException
            }
            catch ( Exception ex )
            {
                throw new BadRequestException ( $"Erro inesperado: {ex.Message}" );
            }
        }

        public async Task<bool> ImageExistsAsync ( string imageUrl )
        {
            if ( string.IsNullOrEmpty ( imageUrl ) )
                throw new BadRequestException ( "URL da imagem é obrigatória." );

            try
            {
                var blobName = ExtractBlobNameFromUrl(imageUrl);
                if ( string.IsNullOrEmpty ( blobName ) )
                    throw new BadRequestException ( "URL da imagem inválida." );

                var containerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var response = await blobClient.ExistsAsync();
                return response.Value;
            }
            catch ( Azure.RequestFailedException ex ) when ( ex.Status == 403 )
            {
                throw new UnauthorizedException ( "Acesso negado ao Azure Storage. Verifique as credenciais." );
            }
            catch ( Azure.RequestFailedException ex )
            {
                throw new BadRequestException ( $"Erro no Azure Storage: {ex.Message}" );
            }
            catch ( BadRequestException )
            {
                throw; // Re-throw BadRequestException
            }
            catch ( Exception ex )
            {
                throw new BadRequestException ( $"Erro inesperado: {ex.Message}" );
            }
        }

        private string? ExtractBlobNameFromUrl ( string imageUrl )
        {
            try
            {
                var uri = new Uri(imageUrl);

                // Se for CDN URL
                if ( !string.IsNullOrEmpty ( _settings.CdnUrl ) && imageUrl.StartsWith ( _settings.CdnUrl ) )
                {
                    return imageUrl.Replace ( _settings.CdnUrl + "/", "" );
                }

                // Se for Blob Storage URL direta
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if ( pathSegments.Length >= 2 )
                {
                    // Pular o nome do container (primeiro segmento)
                    return string.Join ( "/", pathSegments.Skip ( 1 ) );
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}