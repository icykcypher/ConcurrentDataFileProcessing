namespace ConcurrentDataFileProcessing.src.Infrastructure
{
    /// <summary>
    /// Configuration settings for file processing.
    /// </summary>
    public class AppConfig
    {
        public string InputDirectory { get; set; } = "input";
        public string ProcessedDirectory { get; set; } = "processed";
        public string ErrorDirectory { get; set; } = "error";
        public DatabaseConfig Database { get; set; } = new DatabaseConfig();
        public LoggingConfig Logging { get; set; } = new LoggingConfig();
        public int Workers { get; set; } = 4;
    }

    public class DatabaseConfig
    {
        public string Path { get; set; } = "data.db";
    }

    public class LoggingConfig
    {
        public string LogFile { get; set; } = "logs/log-.txt";
        public string MinimumLevel { get; set; } = "Information";
    }
}