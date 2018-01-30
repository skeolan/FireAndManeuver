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
    public class VolleyResolver
    {
        public static IConfiguration Configuration { get; set; }
        private static int DEFAULT_VOLLEYS_PER_EXCHANGE = 3;


        public static void Main(string[] args)
        {            
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
            IConfigurationRoot config = configBuilder.Build();

            string _DefaultUnitXML = Path.Combine(workingDir, config["Default_Unit_Xml"] ?? "DefaultUnit.xml");
            string _DefaultEngineXML = Path.Combine(workingDir, config["Default_GameEngine_Xml"] ?? "DefaultGameEngine.xml");
            int VolleysPerExchange = DEFAULT_VOLLEYS_PER_EXCHANGE;
            var VPEStr = config["Volleys_Per_Exchange"];
            if (!int.TryParse(VPEStr, out VolleysPerExchange)) VolleysPerExchange = DEFAULT_VOLLEYS_PER_EXCHANGE;

            Console.WriteLine($"Initializing game engine with {VolleysPerExchange} volleys per exchange.");

            GameEngine ge = GameEngine.LoadFromXml(_DefaultEngineXML);
            Console.WriteLine($"GameEngine [{ge.id}] from {ge.SourceFile} loaded successfully.");
            Console.WriteLine("Begin Volley Resolution!");

            GameEngine newState = ResolveVolley(ge, config);

            //TestCloning();

            Console.WriteLine("Volley resolution completed!");
        }

        private static GameEngine ResolveVolley(GameEngine ge, IConfigurationRoot config)
        {
            GameEngine newGE = GameEngine.Clone(ge);

            return newGE;
        }


        //TODO: Refactor this out into a test project
        private static void TestCloning()
        {
            BeamBatterySystem B1 = new BeamBatterySystem() { arcs = "FP,F,FS", rating = 2 };
            ScreenSystem D1 = new ScreenSystem() { status = "Operational" };
            HullSystem H1 = new HullSystem() { type = HullTypeLookup.Average };

            var B2 = B1.Clone() as ArcWeaponSystem;
            var D2 = D1.Clone() as DefenseSystem;
            var H2 = H1.Clone() as HullSystem;
            H1.type = HullTypeLookup.Strong;
            var H1a = H1.Clone() as HullSystem;
            H1a.type = HullTypeLookup.Super;
            var H2a = H2.Clone() as HullSystem;
            var A1 = new ArmorSystem() { totalArmor = "4,5", remainingArmor = "4,5" };
            DefenseSystem A2 = (DefenseSystem)A1.Clone();
            var A2a = A1.Clone() as DefenseSystem;

            B2.arcs = "AP, P, FP";
            D2.status = "Damaged";

            Console.WriteLine($"B1 is a {B1.GetType().FullName} and its clone B2 is a {B2.GetType().FullName}");
            Console.WriteLine($"D1 is a {D1.GetType().FullName} and its clone D2 is a {D2.GetType().FullName}");
            Console.WriteLine($"H1 is a {H1.GetType().FullName} and its clone H2 is a {H2.GetType().FullName}");
            Console.WriteLine($"A1 is a {A1.GetType().FullName} and its clone A2 is a {A2.GetType().FullName}");
            Console.WriteLine($"Changing H1's HullType and then making a clone H1a, which is also a {H1a.GetType().FullName}");
            Console.WriteLine($"Making a clone of H2 H2a, which is also a {H2a.GetType().FullName}");
            Console.WriteLine($"B1 arcs  : [{B1.arcs}]");
            Console.WriteLine($"B2 arcs  : [{B2.arcs}]");
            Console.WriteLine($"D1 status: [{D1.status}]");
            Console.WriteLine($"D2 status: [{D2.status}]");
            Console.WriteLine($"H1 type  : [{H1.type}]");
            Console.WriteLine($"H1a type : [{H1a.type}]");
            Console.WriteLine($"H2 type  : [{H2.type}]");
            Console.WriteLine($"H2a type : [{H2a.type}]");
            Console.WriteLine($"A1 totalArmor : [{A1.totalArmor}]");
            Console.WriteLine($"A2 totalArmor : [{((ArmorSystem)A2).totalArmor}] (Cloned as DefenseSystem, cast to ArmorSystem)");

        }
    }

}
