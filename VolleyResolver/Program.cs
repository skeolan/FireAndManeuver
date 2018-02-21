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
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        private const int DefaultVolleysPerExchange = 3;

        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
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

            string defaultUnitXML = Path.Combine(workingDir, config["Default_Unit_Xml"] ?? "DefaultUnit.xml");
            string defaultEngineXML = Path.Combine(workingDir, config["Default_GameEngine_Xml"] ?? "DefaultGameEngine.xml");
            int volleysPerExchange = DefaultVolleysPerExchange;
            var vPEStr = config["Volleys_Per_Exchange"];
            if (!int.TryParse(vPEStr, out volleysPerExchange))
            {
                volleysPerExchange = DefaultVolleysPerExchange;
            }

            GameEngine ge = GameEngine.LoadFromXml(defaultEngineXML);
            Console.WriteLine($"GameEngine [{ge.Id}] from {ge.SourceFile} loaded successfully.");
            Console.WriteLine("Begin Volley Resolution!");

            ge = GameEngine.ResolveVolleys(ge, config, volleysPerExchange, ge.SourceFile);

            Console.WriteLine($"Exchange {ge.Exchange - 1} resolution completed!");

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
