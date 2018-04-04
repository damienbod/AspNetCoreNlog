using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace AspNetCoreNlog
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GlobalDiagnosticsContext.Set("configDir", "C:\\git\\damienbod\\AspNetCoreNlog\\Logs");
            GlobalDiagnosticsContext.Set("connectionString", "Data Source=N051\\MSSQLSERVER2014;Initial Catalog=Nlogs;Integrated Security=True;");
            // GlobalDiagnosticsContext.Set("connectionString", Configuration.GetConnectionString("NLogDb"));
            
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                //logger.Debug("init main");
                BuildWebHost(args).Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                //logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()  // NLog: Setup NLog for Dependency injection
                .Build();
    }
}
