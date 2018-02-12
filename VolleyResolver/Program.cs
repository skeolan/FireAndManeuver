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

            // TestCloning();
            // TestDeserialize(ge);
            TestDice();

            Console.WriteLine($"Exchange {ge.Exchange - 1} resolution completed!");

            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void TestDeserialize(GameEngine ge)
        {
            var destFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SaveAttempt.xml");
            FileInfo f = new FileInfo(destFileName);

            ge.SaveToFile(destFileName);

            if (f.Exists)
            {
                Console.WriteLine($"Saved to {destFileName} successfully!");
            }
        }

        private static void TestDice()
        {
            IDiceUtility roller = new DiceNotationUtility() as IDiceUtility;
            roller.RollFTSuccesses(1);

            var u = new GameUnit() { Name = "Example Unit" };
            u.Orders.Add(
                new VolleyOrders()
                {
                    Volley = 1,
                    Speed = 2,
                    Evasion = 2,
                    FiringOrders = new List<FireOrder> { new FireOrder { FireConID = 1, Priority = "Primary", TargetID = "1", WeaponIDs = "1, 2" } },
                    ManeuveringOrders = new List<ManeuverOrder> { new ManeuverOrder { TargetID = "1", Priority = "Primary", ManeuverType = "Close" } }
                });

            var result = u.ResolveManeuver(u.Orders.FirstOrDefault(), speedDRM: 0, evasionDRM: 0);

            Console.WriteLine($"{u.Name} rolls {result.SpeedSuccesses} for Speed and {result.EvasionSuccesses} for Evasion.");

            Console.WriteLine("Testing penetrating damage versus Screen Rating 2...");
            var damageResult = new DiceNotationUtility()
            .RollFTDamage(
                numberOfDice: 20,
                drm: -1,
                targetScreenRating: 2,
                dealPenetrating: true);
            Console.WriteLine($"Dealt a total of {damageResult.Standard} standard damage and {damageResult.Penetrating} penetrating damage.");
        }

        // TODO: Refactor this out into a test project
        private static void TestCloning()
        {
            BeamBatterySystem b1 = new BeamBatterySystem() { Arcs = "FP,F,FS", Rating = 2 };
            ScreenSystem d1 = new ScreenSystem() { Status = "Operational" };
            HullSystem h1 = new HullSystem() { HullType = HullTypeLookup.Average };

            var b2 = b1.Clone() as ArcWeaponSystem;
            var d2 = d1.Clone() as DefenseSystem;
            var h2 = h1.Clone() as HullSystem;
            h1.HullType = HullTypeLookup.Strong;
            var h1a = h1.Clone() as HullSystem;
            h1a.HullType = HullTypeLookup.Super;
            var h2a = h2.Clone() as HullSystem;
            var a1 = new ArmorSystem() { TotalArmor = "4,5", RemainingArmor = "4,5" };
            DefenseSystem a2 = (DefenseSystem)a1.Clone();
            var a2a = a1.Clone() as DefenseSystem;

            b2.Arcs = "AP, P, FP";
            d2.Status = "Damaged";

            Console.WriteLine($"B1 is a {b1.GetType().FullName} and its clone B2 is a {b2.GetType().FullName}");
            Console.WriteLine($"D1 is a {d1.GetType().FullName} and its clone D2 is a {d2.GetType().FullName}");
            Console.WriteLine($"H1 is a {h1.GetType().FullName} and its clone H2 is a {h2.GetType().FullName}");
            Console.WriteLine($"A1 is a {a1.GetType().FullName} and its clone A2 is a {a2.GetType().FullName}");
            Console.WriteLine($"Changing H1's HullType and then making a clone H1a, which is also a {h1a.GetType().FullName}");
            Console.WriteLine($"Making a clone of H2 H2a, which is also a {h2a.GetType().FullName}");
            Console.WriteLine($"B1 arcs  : [{b1.Arcs}]");
            Console.WriteLine($"B2 arcs  : [{b2.Arcs}]");
            Console.WriteLine($"D1 status: [{d1.Status}]");
            Console.WriteLine($"D2 status: [{d2.Status}]");
            Console.WriteLine($"H1 type  : [{h1.HullType}]");
            Console.WriteLine($"H1a type : [{h1a.HullType}]");
            Console.WriteLine($"H2 type  : [{h2.HullType}]");
            Console.WriteLine($"H2a type : [{h2a.HullType}]");
            Console.WriteLine($"A1 totalArmor : [{a1.TotalArmor}]");
            Console.WriteLine($"A2 totalArmor : [{((ArmorSystem)a2).TotalArmor}] (Cloned as DefenseSystem, cast to ArmorSystem)");
        }
    }
}
