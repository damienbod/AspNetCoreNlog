using System;
using NLog;

namespace ConsoleNLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalDiagnosticsContext.Set("configDir", "C:\\git\\damienbod\\AspNetCoreNlog\\Logs"); 

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Warn("console logging is great");

            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
