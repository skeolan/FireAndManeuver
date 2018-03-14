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
    using FireAndManeuver.Common.ConsoleUtilities;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static IConfiguration Configuration { get; set; }

        public static void Main(string[] args)
        {
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                // Print help
                Console.WriteLine("Call program with one or more paths to json configs and/or xml unit designs.");
                Console.WriteLine("{0} <\\path\\to\\foo.json> <\\path\\to\\bar.json> <\\path\\to\\design.xml> <\\path\\to\\otherDesign.xml>", "[ProgramName]");

                // Exit early
                return;
            }

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(Path.Combine(workingDir, "appsettings.json"), true, true);
            foreach (var jsonConfig in args.Where(a => a.EndsWith(".json")))
            {
                configBuilder.AddJsonFile(jsonConfig);
            }

            var config = configBuilder.Build();

            string defaultUnitXML = Path.Combine(workingDir, config["Default_Unit_Xml"] ?? "DefaultUnit.xml");
            string defaultEngineXML = Path.Combine(workingDir, config["Default_GameEngine_Xml"] ?? "DefaultGameEngine.xml");

            var srcFilesFromArgs = args.Where(x => x.EndsWith(".xml")).ToArray();
            var srcFilesFromConfig = config["srcFiles"] == null ? new string[0] : config["srcFiles"].Split(',');
            var srcDirsFromConfig = config["srcDirectories"] == null ? new string[0] : config["srcDirectories"].Split(',');
            var unitXMLFiles = DataLoadUtilities.GetXMLFileList(srcFilesFromArgs, srcFilesFromConfig, srcDirsFromConfig, defaultUnitXML);
            List<GameUnit> unitSet = DataLoadUtilities.LoadDesignXML(unitXMLFiles);

            // ... and list their stats
            foreach (var u in unitSet)
            {
                Console.WriteLine("\n{0} : \"{1}\"", u.ClassName, u.SourceFile);
                ConsoleReadoutUtilities.GenerateUnitReadout(u, unitSet).ForEach(l => Console.WriteLine(l));
            }

            Console.WriteLine("{0} Unit(s) loaded and displayed successfully.", unitSet.Count);
        }
    }
}
