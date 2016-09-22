using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;

namespace ConsoleNLog
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var configDir = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

            if (configDir != string.Empty)
            {
                var logEventInfo = NLog.LogEventInfo.CreateNullEvent();


                foreach (FileTarget target in LogManager.Configuration.AllTargets.Where(t => t is FileTarget))
                {
                    var filename = target.FileName.Render(logEventInfo).Replace("'", "");
                    target.FileName = System.IO.Path.Combine(configDir, filename);
                }

                LogManager.ReconfigExistingLoggers();
            }

            var logger = LogManager.GetLogger("console");
            logger.Warn("console logging is great");

            Console.WriteLine("log sent");
            Console.ReadKey();
        }
    }
}
