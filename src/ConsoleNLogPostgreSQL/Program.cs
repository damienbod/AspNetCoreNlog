using System;
using NLog;

namespace ConsoleNLogPostgreSQL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            LogManager.Configuration.Variables["configDir"] = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

            logger.Warn("console logging is great");
            logger.Error(new ArgumentException("oh no"));
            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
