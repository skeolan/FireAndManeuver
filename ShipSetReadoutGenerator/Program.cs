using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FireAndManeuver.Common;
using FireAndManeuver.GameModel;

namespace FireAndManeuver.Clients
{
    public class ShipSetReadoutGenerator
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                //Print help
                Console.WriteLine("Call program with one or more paths to json configs and/or xml unit designs.");
                Console.WriteLine("{0} <\\path\\to\\foo.json> <\\path\\to\\bar.json> <\\path\\to\\design.xml> <\\path\\to\\otherDesign.xml>", "[ProgramName]");
                //Exit early
                return;
            }

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile(Path.Combine(workingDir, "appsettings.json"), true, true);
            foreach (var jsonConfig in args.Where(a => a.EndsWith(".json")))
            {
                configBuilder.AddJsonFile(jsonConfig);
            }
            var config = configBuilder.Build();

            string _DefaultUnitXML = Path.Combine(workingDir, config["Default_Unit_Xml"] ?? "DefaultUnit.xml");
            string _DefaultEngineXML = Path.Combine(workingDir, config["Default_GameEngine_Xml"] ?? "DefaultGameEngine.xml");

            var srcFilesFromArgs = args.Where(x => x.EndsWith(".xml")).ToArray();
            var srcFilesFromConfig = (config["srcFiles"] == null ? new string[0] : config["srcFiles"].Split(','));
            var srcDirsFromConfig = (config["srcDirectories"] == null ? new string[0] : config["srcDirectories"].Split(','));
            var unitXMLFiles = DataLoadUtilities.getXMLFileList(srcFilesFromArgs, srcFilesFromConfig, srcDirsFromConfig, _DefaultUnitXML);
            List<Unit> unitSet = DataLoadUtilities.LoadDesignXML(unitXMLFiles);

            //... and list their stats
            foreach (var u in unitSet)
            {
                Console.WriteLine("\n{0} : \"{1}\"", u.ClassName, u.SourceFile);
                ConsoleReadoutUtilities.generateUnitReadout(u, unitSet).ForEach(l => Console.WriteLine(l));
            }

            Console.WriteLine("{0} Unit(s) loaded and displayed successfully.", unitSet.Count);

        }
    }
}
