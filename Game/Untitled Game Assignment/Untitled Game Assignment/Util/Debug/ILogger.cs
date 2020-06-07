namespace Util.CustomDebug
{
    public interface ILogger
    {
        void LogMessage(string message);

        void LogErrorMessage(string message);

        void LogWarningMessage(string message); 
        
        void LogMessage(object obj);

        void LogErrorMessage( object obj );

        void LogWarningMessage( object obj );
    }
}