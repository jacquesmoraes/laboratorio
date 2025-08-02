using System.Diagnostics;

namespace Applications.Services
{
    public interface IPerformanceLoggingService
    {
        IDisposable MeasureOperation ( string operationName, Dictionary<string, object>? metadata = null );
        void LogOperation ( string operationName, long elapsedMs, Dictionary<string, object>? metadata = null );
    }

    public class PerformanceLoggingService : IPerformanceLoggingService
    {
        private readonly ILogger<PerformanceLoggingService> _logger;

        public PerformanceLoggingService ( ILogger<PerformanceLoggingService> logger )
        {
            _logger = logger;
        }

        public IDisposable MeasureOperation ( string operationName, Dictionary<string, object>? metadata = null )
        {
            return new OperationTimer ( _logger, operationName, metadata );
        }

        public void LogOperation ( string operationName, long elapsedMs, Dictionary<string, object>? metadata = null )
        {
            var metadataStr = metadata != null ? string.Join(", ", metadata.Select(kv => $"{kv.Key}={kv.Value}")) : "";

            _logger.LogInformation (
                "Performance - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                operationName, elapsedMs, metadataStr );

            // Log de warning para operações lentas (> 500ms)
            if ( elapsedMs > 500 )
            {
                _logger.LogWarning (
                    "SLOW OPERATION DETECTED - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                    operationName, elapsedMs, metadataStr );
            }

            // Log de error para operações muito lentas (> 2000ms)
            if ( elapsedMs > 2000 )
            {
                _logger.LogError (
                    "VERY SLOW OPERATION DETECTED - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                    operationName, elapsedMs, metadataStr );
            }
        }

        private class OperationTimer : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _operationName;
            private readonly Dictionary<string, object>? _metadata;
            private readonly Stopwatch _stopwatch;

            public OperationTimer ( ILogger logger, string operationName, Dictionary<string, object>? metadata )
            {
                _logger = logger;
                _operationName = operationName;
                _metadata = metadata;
                _stopwatch = Stopwatch.StartNew ( );
            }

            public void Dispose ( )
            {
                _stopwatch.Stop ( );
                var elapsedMs = _stopwatch.ElapsedMilliseconds;

                var metadataStr = _metadata != null ? string.Join(", ", _metadata.Select(kv => $"{kv.Key}={kv.Value}")) : "";

                _logger.LogInformation (
                    "Performance - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                    _operationName, elapsedMs, metadataStr );

                // Log de warning para operações lentas (> 500ms)
                if ( elapsedMs > 500 )
                {
                    _logger.LogWarning (
                        "SLOW OPERATION DETECTED - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                        _operationName, elapsedMs, metadataStr );
                }

                // Log de error para operações muito lentas (> 2000ms)
                if ( elapsedMs > 2000 )
                {
                    _logger.LogError (
                        "VERY SLOW OPERATION DETECTED - Operation: {OperationName}, Duration: {ElapsedMs}ms, Metadata: {Metadata}",
                        _operationName, elapsedMs, metadataStr );
                }
            }
        }
    }
}