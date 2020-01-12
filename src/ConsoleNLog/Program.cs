using System;
using NLog;

namespace ConsoleNLog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalDiagnosticsContext.Set("configDir", "C:\\git\\damienbod\\AspNetCoreNlog\\Logs");
            GlobalDiagnosticsContext.Set("connectionString", "Data Source=.\\SQLEXPRESS;Initial Catalog=NLogDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            logger.Warn("console logging is great");

            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
