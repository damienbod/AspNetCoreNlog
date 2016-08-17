using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;

namespace AspNetCoreNlog
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            services.AddMvc();

            services.AddScoped<LogFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();

            var configDir = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

            if (configDir != string.Empty)
            {
                var logEventInfo = NLog.LogEventInfo.CreateNullEvent();


                foreach (FileTarget target in LogManager.Configuration.AllTargets.Where(t => t is FileTarget))
                {
                    var filename = target.FileName.Render(logEventInfo).Replace("'", "");
                    target.FileName = Path.Combine(configDir, filename);
                }

                LogManager.ReconfigExistingLoggers();
            }

            //env.ConfigureNLog("nlog.config");

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
