
using API.Models;
using Core.Exceptions;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware ( RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _env = env;

        public async Task InvokeAsync ( HttpContext context )
        {
            try
            {
                await _next ( context );
            }
            catch ( Exception ex )
            {
                // Fix for CA2254: Use a constant string for the log message template
                const string logMessageTemplate = "An exception occurred: {ExceptionMessage}";
                _logger.LogError ( ex, logMessageTemplate, ex.Message );

                context.Response.ContentType = "application/json";

                var response = ex switch
                {
                    BadRequestException => HandleBadRequestException(ex),
                    UnauthorizedAccessException => HandleUnauthorizedException(ex),
                    ForbiddenException => HandleForbiddenException(ex),
                    NotFoundException => HandleNotFoundException(ex),
                    ConflictException => HandleConflictException(ex),
                    UnprocessableEntityException => HandleUnprocessableEntityExceptionException(ex),
                    InvalidOperationException => HandleInvalidException(ex),
                    CustomValidationException => HandleValidationException(ex),
                    _ => HandleGenericException(ex),
                };
                context.Response.StatusCode = response.StatusCode;
                await WriteResponse ( context, response );
            }
        }
        private ApiResponse HandleNotFoundException ( Exception ex )
        {
            return new ApiException ( 404, ex.Message );
        }

        private ApiResponse HandleBadRequestException ( Exception ex )
        {
            return new ApiException ( 400, ex.Message );
        }

        private ApiResponse HandleUnauthorizedException ( Exception ex )
        {
            return new ApiException ( 401, ex.Message );
        }

        private ApiResponse HandleForbiddenException ( Exception ex )
        {
            return new ApiException ( 403, ex.Message );
        }

        
        private ApiResponse HandleInvalidException ( Exception ex )
        {
            return new ApiException ( 400, "Operação inválida: " + ex.Message );
        }

        private ApiResponse HandleValidationException ( Exception ex )
        {
            if ( ex is CustomValidationException  vex )
            {
                return new ApiValidationErrorResponse
                {
                    Errors = vex.Errors
                };
            }

            return new ApiValidationErrorResponse
            {
                Errors = [ex.Message]
            };
        }

        private ApiResponse HandleConflictException ( Exception ex )
        {
            return new ApiException ( 409, ex.Message );
        }

         private ApiResponse HandleUnprocessableEntityExceptionException ( Exception ex )
        {
            return new ApiException ( 422, ex.Message );
        }
        private ApiResponse HandleGenericException ( Exception ex )
        {
            return _env.IsDevelopment ( )
                ? new ApiException ( 500, ex.Message, ex.StackTrace )
                : new ApiException ( 500, "An unexpected error occurred." );
        }

        private async Task WriteResponse ( HttpContext context, ApiResponse response )
        {
            JsonSerializerOptions options = new ( )
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize ( response, options );

            await context.Response.WriteAsync ( json );
        }

    }
}
