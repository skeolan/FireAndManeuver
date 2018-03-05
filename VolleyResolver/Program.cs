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
    using FireAndManeuver.Common;
    using FireAndManeuver.GameEngine;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        private const int DefaultVolleysPerExchange = 3;

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            // TODO: add ability to pass an arbitrary GameEngineXML path argument instead of using default
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                // Print help
                Console.WriteLine("??? NEEDS HELP TEXT");

                // Exit early
                return;
            }

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(Path.Combine(workingDir, "appsettings.json"), true, true);
            foreach (var jsonConfig in args.Where(a => a.EndsWith(".json")))
            {
                configBuilder.AddJsonFile(jsonConfig);
            }

            IConfigurationRoot config = configBuilder.Build();

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

            Console.WriteLine($"GameEngine [{gameState.Id}] from {gameState.SourceFile} loaded successfully.");
            Console.WriteLine("Begin Volley Resolution!");

            for (int v = gameState.Volley; v <= volleysPerExchange; v++)
            {
                Console.WriteLine();
                Console.WriteLine($"EXCHANGE {gameState.Exchange}, VOLLEY {v}");

                PrintDistanceGraph(gameState);

                gameState = VolleyResolutionEngine.ResolveVolley(gameState, v, gameState.SourceFile);

                PrintDistanceGraph(gameState);

                VolleyResolutionEngine.RecordVolleyReport(gameState, originalSource, destinationFolder);
            }

            // Set up for a new Exchange by clearing out this Exchange's scripting
            gameState.SourceFile = originalSource;
            VolleyResolutionEngine.RecordExchangeReport(gameState, originalSource);

            var oldFile = new FileInfo(originalSource);
            var oldFileName = oldFile.Name;
            var oldFileExt = oldFile.Extension;
            var oldFilePath = oldFile.DirectoryName;

            var newFileName = $"E{gameState.Exchange + 1}-{oldFileName}".Replace($"E{gameState.Exchange}", string.Empty);
            var newFile = new FileInfo(Path.Combine(oldFilePath, newFileName));

            Console.WriteLine();
            Console.WriteLine($"Exchange {gameState.Exchange} resolution completed! Saving resulting state back to enable Exchange {gameState.Exchange + 1} scripting:");
            Console.WriteLine($"     {newFile.FullName}");

            gameState.Exchange++;
            gameState.Volley = 1;

            gameState.ClearOrders();
            gameState.SourceFile = newFile.FullName;
            GameStateStreamUtilities.SaveToFile(newFile.FullName, gameState);

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void PrintDistanceGraph(GameState ge)
        {
            Console.WriteLine(string.Empty.PadRight(100, '*'));
            var distanceGraph = ConsoleReadoutUtilities.GenerateDistanceReadout(ge.Distances);
            foreach (var d in distanceGraph)
            {
                foreach (var line in ConsoleReadoutUtilities.WrapDecorated(d, 100, "* ", " *"))
                {
                    Console.WriteLine(line);
                }
            }

            Console.WriteLine(string.Empty.PadRight(100, '*'));
        }
    }
}
