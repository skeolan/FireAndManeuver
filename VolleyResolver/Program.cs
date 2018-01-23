﻿using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FireAndManeuver.Common;
using FireAndManeuver.GameModel;

namespace FireAndManeuver.Clients
{
    public class VolleyResolver
    {
        public static IConfiguration Configuration { get; set; }


        public static void Main(string[] args)
        {
            //TODO: Refactor out a "UnitDisplayer" console app and a "GameEngineDisplayer" console app, leaving behind a largely empty "VolleyResolver" app

            string workingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                //Print help
                Console.WriteLine("??? NEEDS HELP TEXT");
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


            GameEngine ge = GameEngine.loadFromXml(_DefaultEngineXML);
            Console.WriteLine($"GameEngine [{ge.id}] from {ge.SourceFile} loaded successfully.");
            Console.WriteLine("Begin Volley Resolution!");

        }



    }

}
