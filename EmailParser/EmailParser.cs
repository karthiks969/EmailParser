#region Import

using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using EmailParser;
using Microsoft.Practices.Unity;

#endregion

namespace Totient.EmailParser
{
    #region EmailParserClass
    class EmailParser
    {
        static void Main(string[] args)
        {
            try
            {
                var container = new UnityContainer();
                
                // Registration of container
                Bootstrap.Start(container);

                // Resolve to get the actual implementation
                var emailAdapter = container.Resolve<IPOPEmailClientAdapter>();

                // Action to process the unread messages
                emailAdapter.ProcessUnseenMessages();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class Bootstrap
    {
        public static void Start(UnityContainer container)
        {
            container.RegisterType<IPOPEmailClientAdapter, POPEmailClientAdapter>();
        }
    }

    class ErrorLogConfiguration
    {
        public static void Log()
        {
            // Step 1. Create configuration object
            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration
            ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties
            consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/file.txt";
            fileTarget.Layout = "${message}";

            // Step 4. Define rules
            LoggingRule rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            LoggingRule rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }
    #endregion
}
