namespace HowManyTimes.Shared
{
    /// <summary>
    /// Possible types of counters
    /// </summary>
    public enum CounterType
    {
        Manual,
        Automatic
    }

    /// <summary>
    /// Types of log message
    /// </summary>
    public enum LogType
    {
        Error,
        Info
    }

    /// <summary>
    /// Available repositories for logging
    /// </summary>
    public enum LogRepository
    {
        None,
        File,
        Console,
        Database
    }
}
