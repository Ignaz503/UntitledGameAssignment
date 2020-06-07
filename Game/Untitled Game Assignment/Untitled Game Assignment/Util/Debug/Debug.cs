using System;

namespace Util.CustomDebug
{
    public class Debug : ILogger
    {
        LogSetting logSettings = LogSetting.DefaultLogSetting;

        LogSetting warningSettings = LogSetting.DefaultWarningLogSetting;

        LogSetting errorSettings = LogSetting.DefaultErrorLogSetting;


        protected static Lazy<Debug> _instance = new Lazy<Debug>(()=>new Debug());
        protected static Debug instance => _instance.Value;
        protected Debug()
        {}

        public void LogMessage(string message)
        {
            logSettings.ApplySetting();
            Console.WriteLine(message);
        }

        public void LogErrorMessage(string message)
        {
            errorSettings.ApplySetting();
            Console.WriteLine(message);
        }

        public void LogWarningMessage(string message)
        {
            warningSettings.ApplySetting();
            Console.WriteLine(message);
        }

        public void LogMessage( object obj )
        {
            LogMessage( obj.ToString() );
        }

        public void LogErrorMessage( object obj )
        {
            LogErrorMessage( obj.ToString() );
        }

        public void LogWarningMessage( object obj )
        {
            LogWarning( obj.ToString() );
        }

        public static void Log(string message)
        {
            instance.LogMessage(message);
        }

        public static void Log( object obj)
        {
            instance.LogMessage( obj );
        }
        public static void Log(string message,LogSetting oneOfSetting)
        {
            var revertTo = instance.logSettings;
            ChangeLogSettings( oneOfSetting );
            instance.LogMessage(message);
            ChangeLogSettings( revertTo );
        }

        public static void Log( object obj, LogSetting oneOfSetting )
        {
            var revertTo = instance.logSettings;
            ChangeLogSettings( oneOfSetting );
            instance.LogMessage( obj );
            ChangeLogSettings( revertTo );
        }

        public static void LogError(string error)
        {
            instance.LogErrorMessage(error);
        } 
        
        public static void LogError(object obj)
        {
            instance.LogErrorMessage(obj);
        }
        public static void LogError(string error,LogSetting oneOfSetting )
        {
            var revertTo = instance.errorSettings;
            ChangeLogErrorSetting( oneOfSetting  );
            instance.LogErrorMessage(error);
            ChangeLogErrorSetting( revertTo );
        } 
        
        public static void LogError(object obj,LogSetting oneOfSetting )
        {
            var revertTo = instance.errorSettings;
            ChangeLogErrorSetting( oneOfSetting );
            instance.LogErrorMessage(obj);
            ChangeLogErrorSetting( revertTo );
        }

        public static void  LogWarning(string warning)
        {
            instance.LogWarningMessage(warning);
        } 
        
        public static void  LogWarning(object obj)
        {
            instance.LogWarningMessage(obj);
        }

        public static void  LogWarning(string warning,LogSetting oneOfSetting )
        {
            var revertTo = instance.warningSettings;
            ChangeLogWarningSetting( oneOfSetting );
            instance.LogWarningMessage(warning);
            ChangeLogWarningSetting( revertTo );
        } 
        
        public static void  LogWarning(object obj, LogSetting oneOfSetting )
        {
            var revertTo = instance.warningSettings;
            ChangeLogWarningSetting( oneOfSetting );
            instance.LogWarningMessage(obj);
            ChangeLogWarningSetting( revertTo );
        }
    
        public static void ChangeLogSettings(LogSetting newSetting)
        {
            instance.logSettings = newSetting;
        }

        public static void ChangeLogWarningSetting(LogSetting newSetting)
        {
            instance.warningSettings = newSetting;
        }

        public static void ChangeLogErrorSetting(LogSetting newSetting)
        {
            instance.errorSettings = newSetting;
        }
    }
}