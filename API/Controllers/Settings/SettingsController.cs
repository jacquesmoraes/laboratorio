using Applications.Contracts;
using Applications.Dtos.Settings;
using Applications.Records.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Settings
{
    [Route ( "api/[controller]" )]
    [ApiController]
    public class SettingsController ( IMapper mapper, ISystemSettingsService systemSettingsService, IWebHostEnvironment env ) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<SystemSettingsRecord>> Get ( )
        {
            var entity = await systemSettingsService.GetAsync();
            var record = _mapper.Map<SystemSettingsRecord>(entity);
            return Ok ( record );
        }


        [HttpPut]
        public async Task<IActionResult> Update ( [FromBody] UpdateSystemSettingsDto dto )
        {
            await systemSettingsService.UpdateAsync ( dto );
            return NoContent ( );
        }

        [HttpPost ( "logo" )]
        [RequestSizeLimit ( 5 * 1024 * 1024 )] // Limita o upload a 5MB
        public async Task<IActionResult> UploadLogo ( IFormFile file )
        {
            if ( file == null || file.Length == 0 )
                return BadRequest ( "Arquivo inválido." );

            if ( string.IsNullOrWhiteSpace ( env.WebRootPath ) )
                return StatusCode ( 500, "Diretório raiz da web não está configurado." );

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if ( !allowedExtensions.Contains ( extension ) )
                return BadRequest ( "Tipo de arquivo não permitido." );

            var fileName = $"logo_{Guid.NewGuid()}{extension}";
            var directory = Path.Combine(env.WebRootPath, "uploads", "logos");

            Directory.CreateDirectory ( directory );
            var uploadPath = Path.Combine(directory, fileName);

            using var stream = new FileStream(uploadPath, FileMode.Create);
            await file.CopyToAsync ( stream );

            await systemSettingsService.UpdateLogoFileNameAsync ( fileName, directory );

            return Ok ( new
            {
                success = true,
                fileName,
                url = $"/uploads/logos/{fileName}"
            } );
        }


    }

}
