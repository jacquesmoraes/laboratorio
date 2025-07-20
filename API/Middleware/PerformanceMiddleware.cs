using System.Diagnostics;

namespace API.Middleware
{
    public class PerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMiddleware> _logger;

        public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Só medir performance para endpoints da client area
            if (!context.Request.Path.StartsWithSegments("/api/client-area"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                stopwatch.Stop();
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);

                // Log de performance
                LogPerformance(context, stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private void LogPerformance(HttpContext context, long elapsedMs)
        {
            var endpoint = context.Request.Path;
            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;
            var clientId = GetClientIdFromContext(context);

            _logger.LogInformation(
                "Client Area Performance - Endpoint: {Method} {Endpoint}, ClientId: {ClientId}, StatusCode: {StatusCode}, Duration: {ElapsedMs}ms",
                method, endpoint, clientId, statusCode, elapsedMs);

            // Log de warning para endpoints lentos (> 1000ms)
            if (elapsedMs > 1000)
            {
                _logger.LogWarning(
                    "SLOW ENDPOINT DETECTED - Endpoint: {Method} {Endpoint}, ClientId: {ClientId}, Duration: {ElapsedMs}ms",
                    method, endpoint, clientId, elapsedMs);
            }

            // Log de error para endpoints muito lentos (> 3000ms)
            if (elapsedMs > 3000)
            {
                _logger.LogError(
                    "VERY SLOW ENDPOINT DETECTED - Endpoint: {Method} {Endpoint}, ClientId: {ClientId}, Duration: {ElapsedMs}ms",
                    method, endpoint, clientId, elapsedMs);
            }
        }

        private int? GetClientIdFromContext(HttpContext context)
        {
            // Tenta extrair clientId do token JWT
            var clientIdClaim = context.User?.FindFirst("clientId")?.Value;
            if (int.TryParse(clientIdClaim, out var clientId))
                return clientId;

            // Fallback para desenvolvimento (hardcoded)
            return 6;
        }
    }
}