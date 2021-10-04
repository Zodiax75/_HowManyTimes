using HowManyTimes.Shared;

namespace HowManyTimes.Services
{
    /// <summary>
    /// Log service
    /// </summary>
    public static class LogService
    {
        #region Methods
        /// <summary>
        /// Logs message into repository
        /// </summary>
        /// <param name="message">log message</param>
        public static void Log(LogType type, string message)
        {
            switch (LogRepository)
            {
                // do nothing, no log
                case LogRepository.None:
                    return;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Active repository for log messages
        /// </summary>
        public static LogRepository LogRepository {  get; set; }
        #endregion

        #region Private properties

        #endregion
    }
}
