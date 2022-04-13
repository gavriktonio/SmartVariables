using System;
using UnityEngine;

namespace SmartVariables
{
    public enum LogLevel
    {
        Off = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        All = 4,
        Debug = 4,
    }

    public class Logger
    {
        public string Prefix { get; set; } = "[SmartVariables]: ";
        private LogLevel logLevel = SmartLogger.GlobalLogLevel;

        public LogLevel LogLevel
        {
            get => logLevel;
            set
            {
                if (!SmartLogger.IgnoreOverrides)
                    logLevel = value;
            }
        }

        public void LogDebug(string format, params object[] args)
        {
            if (LogLevel < LogLevel.Debug)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogFormat(Prefix + format, args);
#endif
        }

        public void Log(string format, params object[] args)
        {
            if (LogLevel < LogLevel.Info)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogFormat(Prefix + format, args);
#endif
        }

        public void LogWarning(string format, params object[] args)
        {
            if (LogLevel < LogLevel.Warning)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarningFormat(Prefix + format, args);
#endif
        }

        public void LogError(string format, params object[] args)
        {
            if (LogLevel < LogLevel.Error)
                return;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogErrorFormat(Prefix + format, args);
#endif
        }
    }

    public class SmartLogger
    {
        public static readonly Logger DefaultLogger = new Logger();
        public static LogLevel GlobalLogLevel => SmartLoggerSettings.Config.GlobalLogLevel;
        public static bool IgnoreOverrides => SmartLoggerSettings.Config.IgnoreOverrides;

        public static void LogDebug(string format, params object[] args)
        {
            DefaultLogger.LogDebug(format, args);
        }

        public static void Log(string format, params object[] args)
        {
            DefaultLogger.Log(format, args);
        }

        public static void LogWarning(string format, params object[] args)
        {
            DefaultLogger.LogWarning(format, args);
        }

        public static void LogError(string format, params object[] args)
        {
            DefaultLogger.LogError(format, args);
        }
    }
}
