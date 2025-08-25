using Core.Models.WebSite;

namespace API.Controllers.WebSite
{
    [Route ( "api/[controller]" )]
    [ApiController]
    [Authorize ( Roles = "admin" )]
    public class FileUploadController (
        IFileStorageService fileStorageService,
        ILogger<FileUploadController> logger ) : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;
        private readonly ILogger<FileUploadController> _logger = logger;

        [HttpPost ( "image" )]
        public async Task<IActionResult> UploadImage ( IFormFile file )
        {
            var fileModel = new FileUploadModel
            {
                Content = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            var imageUrl = await _fileStorageService.UploadImageAsync(fileModel);

            _logger.LogInformation ( "Imagem enviada com sucesso para {ImageUrl} por {User}",
                imageUrl, User.Identity?.Name );

            // Extrair nome do arquivo da URL
            var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            return Ok ( new
            {
                success = true,
                imageUrl = imageUrl,
                fileName = fileName,
                originalName = file.FileName,
                size = file.Length
            } );
        }

        [HttpDelete ( "image" )]
        public async Task<IActionResult> DeleteImage ( [FromQuery] string imageUrl )
        {
            if ( string.IsNullOrEmpty ( imageUrl ) )
                throw new BadRequestException ( "URL da imagem é obrigatória." );

            var success = await _fileStorageService.DeleteImageAsync(imageUrl);

            if ( success )
            {
                _logger.LogInformation ( "Imagem deletada com sucesso: {ImageUrl}", imageUrl );
                return Ok ( new { success = true } );
            }

            throw new NotFoundException ( "Imagem não encontrada." );
        }
    }

}