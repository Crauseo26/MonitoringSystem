using Serilog.Core;
using Serilog.Events;

namespace MonitoringSystem.Observability.Enrichers;

/// <summary>
/// Enriches log events with a correlation ID for request tracking
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public CorrelationIdEnricher(ICorrelationIdProvider correlationIdProvider)
    {
        _correlationIdProvider = correlationIdProvider;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var correlationId = _correlationIdProvider.GetCorrelationId();
        if (!string.IsNullOrEmpty(correlationId))
        {
            var property = propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}

/// <summary>
/// Interface for providing correlation IDs
/// </summary>
public interface ICorrelationIdProvider
{
    string? GetCorrelationId();
    void SetCorrelationId(string correlationId);
}

/// <summary>
/// Default implementation using AsyncLocal for correlation ID storage
/// </summary>
public class AsyncLocalCorrelationIdProvider : ICorrelationIdProvider
{
    private static readonly AsyncLocal<string?> CorrelationId = new();

    public string? GetCorrelationId()
    {
        return CorrelationId.Value;
    }

    public void SetCorrelationId(string correlationId)
    {
        CorrelationId.Value = correlationId;
    }
}
