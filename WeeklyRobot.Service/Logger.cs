using System;

namespace WeeklyRobot.Service
{
    public static class Logger
    {
        public static string LogLevel { get; } = Config.TryGet<string>(nameof(LogLevel), "Debug");
        private static LogType _level = LogType.Info;

        static Logger()
        {
            Enum.TryParse(LogLevel, out _level);
        }

        public static void Debug(string format, params object[] arg) => Write(LogType.Debug, format, arg);

        public static void Error(string format, params object[] arg) => Write(LogType.Error, format, arg);

        public static void Fatal(string format, params object[] arg) => Write(LogType.Fatal, format, arg);

        public static void Info(string format, params object[] arg) => Write(LogType.Info, format, arg);

        public static void Trace(string format, params object[] arg) => Write(LogType.Trace, format, arg);

        public static void Warn(string format, params object[] arg) => Write(LogType.Warn, format, arg);

        private static void Write(LogType type, string format, params object[] arg)
        {
            if (_level >= type)
            {
                var formatMsg = arg.Length > 0 ? string.Format(format, arg) : format;
                Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] [{type}] {formatMsg}");
            }
        }

        enum LogType
        {
            Fatal, Error, Warn, Info, Debug, Trace
        }
    }
}
