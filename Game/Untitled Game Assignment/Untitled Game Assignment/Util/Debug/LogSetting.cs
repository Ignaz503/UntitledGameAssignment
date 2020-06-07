using System;

namespace Util.CustomDebug
{
    public struct LogSetting
    {
        public string Header;
        public bool WriteLogHeader;
        public bool WriteTimestamp;
        public ConsoleColor HeaderColor;
        public ConsoleColor HeaderBackgroundColor;
        public ConsoleColor TextColor;

        public ConsoleColor BackgroundColor;

        public void ApplySetting()
        {
            Console.BackgroundColor =  HeaderBackgroundColor;
            Console.ForegroundColor =  HeaderColor;
            if( WriteTimestamp)
            {
                Console.Write($"[");
                Console.Write(System.DateTime.UtcNow.TimeOfDay.ToString(@"hh\:mm\:ss\.ff"));                
                Console.Write("]");
            }
            if( WriteLogHeader)
            {
                Console.Write(Header+" ");
            }
            Console.BackgroundColor =  BackgroundColor;
            Console.ForegroundColor =  TextColor;
        }

        public static LogSetting DefaultLogSetting => new LogSetting()
                                                        { Header ="[LOG]",
                                                          WriteLogHeader = true,
                                                          WriteTimestamp = true,
                                                          HeaderColor = ConsoleColor.Green,
                                                          HeaderBackgroundColor = ConsoleColor.Black,
                                                          TextColor = ConsoleColor.White,
                                                          BackgroundColor = ConsoleColor.Black};
        public static LogSetting DefaultErrorLogSetting => new LogSetting()
                                                        { Header ="[ERROR]",
                                                          WriteLogHeader = true,
                                                          WriteTimestamp = true,
                                                          HeaderColor = ConsoleColor.DarkRed,
                                                          HeaderBackgroundColor = ConsoleColor.Black,
                                                          TextColor = ConsoleColor.White,
                                                          BackgroundColor = ConsoleColor.Black};
        public static LogSetting DefaultWarningLogSetting => new LogSetting()
                                                        { Header ="[WARNING]",
                                                          WriteLogHeader = true,
                                                          WriteTimestamp = true,
                                                          HeaderColor = ConsoleColor.DarkYellow,
                                                          HeaderBackgroundColor = ConsoleColor.Black,
                                                          TextColor = ConsoleColor.White,
                                                          BackgroundColor = ConsoleColor.Black};
    }
}