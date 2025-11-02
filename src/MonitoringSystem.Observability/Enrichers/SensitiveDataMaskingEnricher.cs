using Serilog.Core;
using Serilog.Events;
using MonitoringSystem.Observability.Configuration;

namespace MonitoringSystem.Observability.Enrichers;

/// <summary>
/// Enricher that masks sensitive data in log events
/// </summary>
public class SensitiveDataMaskingEnricher : ILogEventEnricher
{
    private readonly SensitiveDataOptions _options;
    private readonly ISensitiveDataMasker _masker;

    public SensitiveDataMaskingEnricher(SensitiveDataOptions options, ISensitiveDataMasker? masker = null)
    {
        _options = options;
        _masker = masker ?? new DefaultSensitiveDataMasker(options);
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!_options.EnableMasking)
            return;

        var propertiesToMask = logEvent.Properties
            .Where(p => ShouldMaskProperty(p.Key))
            .ToList();

        foreach (var property in propertiesToMask)
        {
            logEvent.RemovePropertyIfPresent(property.Key);
            var maskedValue = _masker.Mask(property.Value.ToString());
            var maskedProperty = propertyFactory.CreateProperty(property.Key, maskedValue);
            logEvent.AddPropertyIfAbsent(maskedProperty);
        }
    }

    private bool ShouldMaskProperty(string propertyName)
    {
        return _options.PropertyNamesToMask.Any(mask =>
            propertyName.Contains(mask, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Interface for masking sensitive data
/// </summary>
public interface ISensitiveDataMasker
{
    string Mask(string value);
}

/// <summary>
/// Default implementation of sensitive data masking
/// </summary>
public class DefaultSensitiveDataMasker : ISensitiveDataMasker
{
    private readonly SensitiveDataOptions _options;

    public DefaultSensitiveDataMasker(SensitiveDataOptions options)
    {
        _options = options;
    }

    public string Mask(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Remove quotes if present
        var cleanValue = value.Trim('"');

        if (cleanValue.Length <= _options.ShowLastCharacters)
            return new string(_options.MaskCharacter[0], cleanValue.Length);

        var maskLength = cleanValue.Length - _options.ShowLastCharacters;
        var masked = new string(_options.MaskCharacter[0], maskLength);
        var visible = cleanValue.Substring(maskLength);

        return $"\"{masked}{visible}\"";
    }
}
