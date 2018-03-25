using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;

namespace DomainService
{
    /// <summary>
    /// Generated class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Generated main entry point for the application
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Generated web host build method
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHost BuildWebHost(string[] args)
        {
            
            CLIOption test1 = new CLIOption()
            {
                Command = "--port",
                Description = "Port Number",
                OptionType = CLIOptionTypes.SingleValue
            };

            List<CLIOption> cliOptions = new List<CLIOption>() { test1 };
            CommandLineArguments cla = new CommandLineArguments(args, cliOptions);


            cla.Args.TryGetValue("port", out string testport);

            if (testport.Length > 0)
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"http://localhost:{testport}");
                Console.WriteLine($"ARG|| key: Port, value: {testport}");
            }


            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    IHostingEnvironment env = builderContext.HostingEnvironment;
                    config.AddCommandLine(args);
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                    
                    
                    var builtConfig = config.Build();
                    
                    if (!String.IsNullOrWhiteSpace(builtConfig["KeyVault:Location"]))
                    {
                        config.AddAzureKeyVault(
                            builtConfig["KeyVault:Location"]
                            , builtConfig["KeyVault:Authorization:ClientId"]
                            , builtConfig["KeyVault:Authorization:ClientSecret"]
                            , new EnvironmentSecretManager(
                                $"{builtConfig["KeyVault:EnvironmentName"]}{builtConfig["KeyVault:SecretSeparator"]}")

                            );
                    }
                })
                .UseKestrel(options =>
                {

                    options.Limits.MaxRequestBufferSize = int.MaxValue;
                    options.Limits.MaxRequestLineSize = int.MaxValue;
                    options.Limits.MaxResponseBufferSize = int.MaxValue;
                    options.AddServerHeader = false;
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseIISIntegration()
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();

            return builder;
        }

    }
}
