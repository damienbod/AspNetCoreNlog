using System;
using NLog;

namespace ConsoleNLogPostgreSQL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalDiagnosticsContext.Set("configDir", "C:\\git\\damienbod\\AspNetCoreNlog\\Logs");
            GlobalDiagnosticsContext.Set("connectionString", "User ID=damienbod;Password=damienbod;Host=localhost;Port=5432;Database=log;Pooling=true;");

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
 
            logger.Warn("console logging is great");
            logger.Error(new ArgumentException("oh no"));
            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
