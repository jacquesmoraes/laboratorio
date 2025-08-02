namespace API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;
        private readonly IHostEnvironment _env = env;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                //  Capture HTTP status codes after pipeline (e.g., 401, 403, 404)
                await HandleStatusCodeAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {ExceptionMessage}", ex.Message);

                context.Response.ContentType = "application/json";

                var response = ex switch
                {
                    BadRequestException => HandleBadRequestException(ex),
                    UnauthorizedAccessException => HandleUnauthorizedException(ex),
                    ForbiddenException => HandleForbiddenException(ex),
                    BusinessRuleException => HandleBusinessRuleException(ex),

                    NotFoundException => HandleNotFoundException(ex),
                    ConflictException => HandleConflictException(ex),
                    UnprocessableEntityException => HandleUnprocessableEntityException(ex),
                    InvalidOperationException => HandleInvalidException(ex),
                    CustomValidationException => HandleValidationException(ex),
                    _ => HandleGenericException(ex),
                };

                context.Response.StatusCode = response.StatusCode;
                await WriteResponse(context, response);
            }
        }

        //  Handle default HTTP status codes (without thrown exceptions)
        private async Task HandleStatusCodeAsync(HttpContext context)
        {
            if (context.Response.HasStarted)
                return;
            if (context.Response.ContentLength != null || !context.Response.Body.CanWrite)
                return;

            if (context.Response.StatusCode >= 400 &&
                !context.Request.Path.StartsWithSegments("/swagger"))
            {
                context.Response.ContentType = "application/json";

                var response = context.Response.StatusCode switch
                {
                    401 => new ApiResponse(401, "Unauthorized. Please log in to continue."),
                    403 => new ApiResponse(403, "Access denied. You do not have permission to access this resource."),
                    404 => new ApiResponse(404, "Resource not found."),
                    422 => new ApiResponse(422, "Unprocessable entity."),
                    500 => new ApiResponse(500, "Internal server error."),
                    _ => new ApiResponse(context.Response.StatusCode, "Unexpected error.")
                };

                await WriteResponse(context, response);
            }
        }

        private ApiResponse HandleNotFoundException(Exception ex) =>
            new ApiException(404, ex.Message);

        private ApiResponse HandleBadRequestException(Exception ex) =>
            new ApiException(400, ex.Message);

        private ApiResponse HandleUnauthorizedException(Exception ex) =>
            new ApiException(401, ex.Message);

        private ApiResponse HandleForbiddenException(Exception ex) =>
            new ApiException(403, ex.Message);

        private ApiResponse HandleBusinessRuleException(Exception ex) =>
    new ApiException(400, ex.Message);

        private ApiResponse HandleInvalidException(Exception ex) =>
            new ApiException(400, "Invalid operation: " + ex.Message);

        private ApiResponse HandleValidationException(Exception ex)
        {
            if (ex is CustomValidationException vex)
            {
                return new ApiValidationErrorResponse(vex.Errors.FirstOrDefault())
                {
                    Errors = vex.Errors
                };
            }

            return new ApiValidationErrorResponse(ex.Message)
            {
                Errors = [ex.Message]
            };
        }

        private ApiResponse HandleConflictException(Exception ex) =>
            new ApiException(409, ex.Message);

        private ApiResponse HandleUnprocessableEntityException(Exception ex) =>
            new ApiException(422, ex.Message);

        private ApiResponse HandleGenericException(Exception ex) =>
            _env.IsDevelopment()
                ? new ApiException(500, ex.Message, ex.StackTrace)
                : new ApiException(500, "Unexpected server error.");

        private async Task WriteResponse(HttpContext context, ApiResponse response)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
