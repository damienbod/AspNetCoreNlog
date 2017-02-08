
# NLog posts in this series:

<strong>Version:</strong> <a href="https://github.com/damienbod/AspNetCoreNlog">VS2017 RC3 csproj</a> | <a href="https://github.com/damienbod/AspNetCoreNlog/tree/VS2015_project_json">VS2015 project.json</a>


<ol>
	<li><a href="https://damienbod.com/2016/08/17/asp-net-core-logging-with-nlog-and-microsoft-sql-server/">ASP.NET Core logging with NLog and Microsoft SQL Server</a></li>
	<li><a href="https://damienbod.com/2016/08/20/asp-net-core-logging-with-nlog-and-elasticsearch/">ASP.NET Core logging with NLog and Elasticsearch</a></li>
	<li><a href="https://damienbod.com/2016/09/22/setting-the-nlog-database-connection-string-in-the-asp-net-core-appsettings-json/">Settings the NLog database connection string in the ASP.NET Core appsettings.json</a></li>
</ol>


## ASP.NET Core logging with NLog and MS SQLServer

This article shows how to setup logging in an ASP.NET Core application which logs to a Microsoft SQL Server using NLog.

The NLog.Extensions.Logging Nuget package as well as the System.Data.SqlClient are added to the dependencies in the csproj file.

```javascript
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp1.1</TargetFramework>
    <PreserveCompilationContext>true</PreserveCompilationContext>
    <AssemblyName>AspNetCoreNlog</AssemblyName>
    <OutputType>Exe</OutputType>
    <PackageId>AspNetCoreNlog</PackageId>
    <PackageTargetFallback>$(PackageTargetFallback);dotnet5.6;portable-net45+win8</PackageTargetFallback>
  </PropertyGroup>
  <ItemGroup>
    <Content Update="wwwroot\**\*;Views;Areas\**\Views;appsettings.json;nlog.config;web.config">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\NLog.Targets.ElasticSearch\NLog.Targets.ElasticSearch.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Localization" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Localization" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Routing" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Server.IISIntegration" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Server.Kestrel" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="1.1.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.0.0-msbuild3-final" />
    <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="1.1.0" />
    <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="1.1.0" />
    <PackageReference Include="NLog.Web.AspNetCore" Version="4.3.0" />
    <PackageReference Include="System.Data.SqlClient" Version="4.3.0" />
  </ItemGroup>
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0-msbuild3-final" />
    <DotNetCliToolReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Tools" Version="1.0.0-msbuild3-final" />
  </ItemGroup>
</Project>
```


Now a nlog.config file is created and added to the project. This file contains the configuration for NLog. In the file, the targets for the logs are defined as well as the rules. An internal log file is also defined, so that if something is wrong with the logging configuration, you can find out why. 

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Warn"
      internalLogFile="C:\git\damienbod\AspNetCoreNlog\Logs\internal-nlog.txt">
    
  <targets>
    <target xsi:type="File" name="allfile" fileName="nlog-all.log"
                layout="${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|${message} ${exception}" />

    <target xsi:type="File" name="ownFile-web" fileName="nlog-own.log"
             layout="${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|  ${message} ${exception}" />

    <target xsi:type="Null" name="blackhole" />

    <target name="database" xsi:type="Database" >

        <connectionString>
            Data Source=N275\MSSQLSERVER2014;Initial Catalog=Nlogs;Integrated Security=True;
        </connectionString>
<!--
  Remarks:
    The appsetting layouts require the NLog.Extended assembly.
    The aspnet-* layouts require the NLog.Web assembly.
    The Application value is determined by an AppName appSetting in Web.config.
    The "NLogDb" connection string determines the database that NLog write to.
    The create dbo.Log script in the comment below must be manually executed.

  Script for creating the dbo.Log table.

  SET ANSI_NULLS ON
  SET QUOTED_IDENTIFIER ON
  CREATE TABLE [dbo].[Log] (
      [Id] [int] IDENTITY(1,1) NOT NULL,
      [Application] [nvarchar](50) NOT NULL,
      [Logged] [datetime] NOT NULL,
      [Level] [nvarchar](50) NOT NULL,
      [Message] [nvarchar](max) NOT NULL,
      [Logger] [nvarchar](250) NULL,
      [Callsite] [nvarchar](max) NULL,
      [Exception] [nvarchar](max) NULL,
    CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
      WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
  ) ON [PRIMARY]
-->

          <commandText>
              insert into dbo.Log (
              Application, Logged, Level, Message,
              Logger, CallSite, Exception
              ) values (
              @Application, @Logged, @Level, @Message,
              @Logger, @Callsite, @Exception
              );
          </commandText>

          <parameter name="@application" layout="AspNetCoreNlog" />
          <parameter name="@logged" layout="${date}" />
          <parameter name="@level" layout="${level}" />
          <parameter name="@message" layout="${message}" />

          <parameter name="@logger" layout="${logger}" />
          <parameter name="@callSite" layout="${callsite:filename=true}" />
          <parameter name="@exception" layout="${exception:tostring}" />
      </target>
      
  </targets>

  <rules>
    <!--All logs, including from Microsoft-->
    <logger name="*" minlevel="Trace" writeTo="allfile" />

    <logger name="*" minlevel="Trace" writeTo="database" />
      
    <!--Skip Microsoft logs and so log only own logs-->
    <logger name="Microsoft.*" minlevel="Trace" writeTo="blackhole" final="true" />
    <logger name="*" minlevel="Trace" writeTo="ownFile-web" />
  </rules>
</nlog>

```

The nlog.config also needs to be added to the publishOptions in the csproj file.

```javascript
  <ItemGroup>
    <Content Update="wwwroot\**\*;Views;Areas\**\Views;appsettings.json;nlog.config;web.config">
      <CopyToPublishDirectory>PreserveNewest</CopyToPublishDirectory>
    </Content>
  </ItemGroup>
```

Now the database can be setup. You can create a new database, or use and existing one and add the dbo.Log table to it using the script below. 

```csharp
  SET ANSI_NULLS ON
  SET QUOTED_IDENTIFIER ON
  CREATE TABLE [dbo].[Log] (
      [Id] [int] IDENTITY(1,1) NOT NULL,
      [Application] [nvarchar](50) NOT NULL,
      [Logged] [datetime] NOT NULL,
      [Level] [nvarchar](50) NOT NULL,
      [Message] [nvarchar](max) NOT NULL,
      [Logger] [nvarchar](250) NULL,
      [Callsite] [nvarchar](max) NULL,
      [Exception] [nvarchar](max) NULL,
    CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
      WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
  ) ON [PRIMARY]

```

The table in the database must match the configuration defined in the nlog.config file. The database target defines the connection string, the command used to add a log and also the parameters required.

You can change this as required. As yet, most of the NLog parameters, do not work with ASP.NET Core, but this will certainly change as it is in early development. The NLog.Web Nuget package, when completed will contain the ASP.NET Core parameters.


Now NLog can be added to the application in the Startup class in the configure method. The AddNLog extension method is used and the logging directory can be defined.

```csharp
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

```

Now the logging can be used, using the default logging framework from ASP.NET Core.

An example of an ActionFilter

```csharp
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace AspNetCoreNlog
{
    public class LogFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public LogFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LogFilter");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("OnActionExecuting");
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("OnActionExecuted");
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            _logger.LogInformation("OnResultExecuting");
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("OnResultExecuted");
            base.OnResultExecuted(context);
        }
    }
}


```

The action filter is added in the Startup ConfigureServices services.

```csharp
 public void ConfigureServices(IServiceCollection services)
{

    // Add framework services.
    services.AddMvc();

    services.AddScoped<LogFilter>();
}

```

 And some logging can be added to a MVC controller.
```csharp
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreNlog.Controllers
{
    [ServiceFilter(typeof(LogFilter))]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private  ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogCritical("nlog is working from a controller");
            throw new ArgumentException("way wrong");
            return new string[] { "value1", "value2" };
        }

```

When the application is started, the logs are written to a local file in the Logs folder and also to the database. 

    
<img src="https://damienbod.files.wordpress.com/2016/08/sqlaspnetdatabselogger_01.png?w=600" alt="sqlaspnetdatabselogger_01" width="600" height="287" class="alignnone size-medium wp-image-7058" />

## Notes

NLog for ASP.NET Core is in early development, and the documetation is for .NET and not for dotnetcore, so a lot of parameters, layouts, targets, etc do not work. 
    This project is open source, so you can extend it and contribute to if if you want.

## Links

https://github.com/NLog/NLog.Extensions.Logging

https://github.com/NLog

https://docs.asp.net/en/latest/fundamentals/logging.html

https://msdn.microsoft.com/en-us/magazine/mt694089.aspx

https://github.com/nlog/NLog/wiki/Database-target
