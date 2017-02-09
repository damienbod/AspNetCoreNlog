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
using Microsoft.AspNetCore.Http;
using NLog.Web;

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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add framework services.
            services.AddMvc();

            services.AddScoped<LogFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddNLog();

			//add NLog.Web
			app.AddNLogWeb();

			////foreach (DatabaseTarget target in LogManager.Configuration.AllTargets.Where(t => t is DatabaseTarget))
			////{
			////	target.ConnectionString = Configuration.GetConnectionString("NLogDb");
			////}
			
			////LogManager.ReconfigExistingLoggers();

			LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("NLogDb");
			LogManager.Configuration.Variables["configDir"] = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

			app.UseMvc();
		}
    }
}
