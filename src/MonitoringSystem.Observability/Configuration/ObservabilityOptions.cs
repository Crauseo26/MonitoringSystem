namespace MonitoringSystem.Observability.Configuration;

/// <summary>
/// Configuration options for the observability system
/// </summary>
public class ObservabilityOptions
{
    /// <summary>
    /// Application name to be included in logs
    /// </summary>
    public string ApplicationName { get; set; } = "Application";

    /// <summary>
    /// Environment name (Development, Staging, Production)
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Logging configuration
    /// </summary>
    public LoggingOptions Logging { get; set; } = new();

    /// <summary>
    /// Sensitive data masking configuration
    /// </summary>
    public SensitiveDataOptions SensitiveData { get; set; } = new();
}

/// <summary>
/// Logging-specific configuration options
/// </summary>
public class LoggingOptions
{
    /// <summary>
    /// Minimum log level (Verbose, Debug, Information, Warning, Error, Fatal)
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Enable console logging
    /// </summary>
    public bool EnableConsole { get; set; } = true;

    /// <summary>
    /// Enable file logging
    /// </summary>
    public bool EnableFile { get; set; } = true;

    /// <summary>
    /// File logging configuration
    /// </summary>
    public FileLoggingOptions File { get; set; } = new();

    /// <summary>
    /// Enable structured JSON logging
    /// </summary>
    public bool UseJsonFormat { get; set; } = true;

    /// <summary>
    /// Enable request logging middleware
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Override minimum levels for specific namespaces
    /// </summary>
    public Dictionary<string, string> MinimumLevelOverrides { get; set; } = new()
    {
        ["Microsoft"] = "Warning",
        ["Microsoft.Hosting.Lifetime"] = "Information",
        ["System"] = "Warning"
    };
}

/// <summary>
/// File logging configuration
/// </summary>
public class FileLoggingOptions
{
    /// <summary>
    /// Path to the log file directory
    /// </summary>
    public string Path { get; set; } = "./logs";

    /// <summary>
    /// Log file name pattern
    /// </summary>
    public string FileNamePattern { get; set; } = "log-.txt";

    /// <summary>
    /// Rolling interval (Infinite, Year, Month, Day, Hour, Minute)
    /// </summary>
    public string RollingInterval { get; set; } = "Day";

    /// <summary>
    /// Number of log files to retain (null = unlimited)
    /// </summary>
    public int? RetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// Maximum file size in bytes (null = unlimited)
    /// </summary>
    public long? FileSizeLimitBytes { get; set; } = 100 * 1024 * 1024; // 100 MB

    /// <summary>
    /// Shared file access mode
    /// </summary>
    public bool Shared { get; set; } = true;
}

/// <summary>
/// Sensitive data masking configuration
/// </summary>
public class SensitiveDataOptions
{
    /// <summary>
    /// Enable automatic masking of sensitive data
    /// </summary>
    public bool EnableMasking { get; set; } = true;

    /// <summary>
    /// Property names to mask (case-insensitive)
    /// </summary>
    public List<string> PropertyNamesToMask { get; set; } = new()
    {
        "password",
        "pwd",
        "secret",
        "token",
        "apikey",
        "api_key",
        "authorization",
        "auth",
        "creditcard",
        "credit_card",
        "ssn",
        "social_security"
    };

    /// <summary>
    /// Mask character to use
    /// </summary>
    public string MaskCharacter { get; set; } = "*";

    /// <summary>
    /// Number of characters to show at the end (0 = full mask)
    /// </summary>
    public int ShowLastCharacters { get; set; } = 0;
}
