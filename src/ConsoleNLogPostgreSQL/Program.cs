using System;
using NLog;

namespace ConsoleNLogPostgreSQL
{
    public class Program
    {
        public static void Main(string[] args)
        {

            LogManager.Configuration.Variables["configDir"] = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

            var logger = LogManager.GetLogger("console");
            logger.Warn("console logging is great");
            logger.Error(new ArgumentException("oh no"));
            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
