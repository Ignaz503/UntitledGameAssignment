using System;

namespace UntitledGameAssignment
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (var game = new GameMain())
                    game.Run();
            } catch (Exception e) 
            {
                System.Diagnostics.Debug.WriteLine( e );
                Console.ReadKey();
            }
        }
    }
#endif
}
