using System;

namespace GDD2Project1
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ScreenManager screenMgr = new ScreenManager())
            {
                screenMgr.Run();
            }
        }
    }
#endif
}

