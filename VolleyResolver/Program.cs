// <copyright file="Program.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FireAndManeuver.Common;
    using FireAndManeuver.Common.ConsoleUtilities;
    using FireAndManeuver.GameEngine;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Program
    {
        private const int DefaultVolleysPerExchange = 3;

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            // TODO: add ability to pass an arbitrary GameEngineXML path argument instead of using default
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(Path.Combine(workingDir, "appsettings.json"), true, true);
            foreach (var jsonConfig in args.Where(a => a.EndsWith(".json")))
            {
                configBuilder.AddJsonFile(jsonConfig);
            }

            // Woohoo Dependency Injection!
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IDiceUtility, DiceNotationUtility>()
                .AddSingleton(configBuilder.Build())
                .BuildServiceProvider();

            services.GetService<ILoggerFactory>()
                .AddConsole()
                .AddDebug();

            var logger = services.GetLogger("VolleyResolver");

            logger.LogInformation("Service Collection initialized successfully");

            IConfigurationRoot config = services.GetService<IConfigurationRoot>();

            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                // Print help
                logger.LogInformation("??? NEEDS HELP TEXT");

                // Exit early
                return;
            }

            // TODO: adapt this so it can take args on the commandline for input-XML and output-XML locations
            // TODO: adapt this so it can take unit-data XML files / dirs on the commandline and use them to flesh out Player/Ship entries
            string defaultUnitXML = Path.Combine(workingDir, config["Default_Unit_Xml"] ?? "DefaultUnit.xml");
            string defaultEngineXML = Path.Combine(workingDir, config["Default_GameEngine_Xml"] ?? "DefaultGameEngine.xml");
            string destinationFolder = config["GameEngine_Xml_OutPath"] ?? workingDir;

            int volleysPerExchange = DefaultVolleysPerExchange;
            var vPEStr = config["Volleys_Per_Exchange"];
            if (!int.TryParse(vPEStr, out volleysPerExchange))
            {
                volleysPerExchange = DefaultVolleysPerExchange;
            }

            GameState gameState = GameStateStreamUtilities.LoadFromXml(defaultEngineXML);
            var originalSource = defaultEngineXML;

            gameState.SourceFile = originalSource;

            logger.LogInformation($"GameEngine [{gameState.Id}] from {gameState.SourceFile} loaded successfully.");
            logger.LogInformation("Begin Volley Resolution!");

            for (int v = gameState.Volley; v <= volleysPerExchange; v++)
            {
                logger.LogInformation("***");
                logger.LogInformation($"EXCHANGE {gameState.Exchange}, VOLLEY {v}");

                PrintDistanceGraph(gameState, logger);

                gameState = VolleyResolutionEngine.ResolveVolley(gameState, v, gameState.SourceFile, services);

                PrintDistanceGraph(gameState, logger);

                VolleyResolutionEngine.RecordVolleyReport(gameState, originalSource, destinationFolder, logger);
            }

            // Set up for a new Exchange by clearing out this Exchange's scripting
            gameState.SourceFile = originalSource;
            VolleyResolutionEngine.RecordExchangeReport(gameState, originalSource, logger);

            var oldFile = new FileInfo(originalSource);
            var oldFileName = oldFile.Name;
            var oldFileExt = oldFile.Extension;
            var oldFilePath = oldFile.DirectoryName;

            var newFileName = $"E{gameState.Exchange + 1}-{oldFileName}".Replace($"E{gameState.Exchange}", string.Empty);
            var newFile = new FileInfo(Path.Combine(oldFilePath, newFileName));

            logger.LogInformation("***");
            logger.LogInformation($"Exchange {gameState.Exchange} resolution completed! " +
                $"\n     Saving resulting state back to enable Exchange {gameState.Exchange + 1} scripting:" +
                $"\n     {newFile.FullName}");

            gameState.Exchange++;
            gameState.Volley = 1;

            gameState.ClearOrders();
            gameState.SourceFile = newFile.FullName;

            // Dispose the serviceCollection to cause all included services to dispose;
            // Among other possible finalization effects, this *should* flush the logger.
            // (Does this need to be done BEFORE SaveToFile() is done?
            (services as IDisposable)?.Dispose();

            GameStateStreamUtilities.SaveToFile(newFile.FullName, gameState);
        }

        private static void PrintDistanceGraph(GameState gameState, ILogger logger)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(string.Empty.PadRight(100, '*'));
            var distanceGraph = ConsoleReadoutUtilities.GenerateDistanceReadout(gameState.Distances);
            foreach (var d in distanceGraph)
            {
                foreach (var line in ConsoleReadoutUtilities.WrapDecorated(d, 100, "* ", " *"))
                {
                    builder.AppendLine(line);
                }
            }

            builder.AppendLine(string.Empty.PadRight(100, '*'));

            logger.LogInformation(builder.ToString());
        }
    }
}
