// <copyright file="GameEngineTest.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FireAndManeuver.Common;
    using FireAndManeuver.GameModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GameEngineTest
    {
        [TestMethod]
        public void GameEngineFileSerializeDeserializeTest()
        {
            GameEngine ge = new GameEngine()
            {
                Id = 99,
                Exchange = 99,
                Volley = 99,
                Turn = 99
            };

            var stream = new MemoryStream();

            ge.SaveToStream(stream);

            var newGE = GameEngine.LoadFromStream(stream);

            Assert.AreEqual(newGE.Id, ge.Id);
            Assert.AreEqual(newGE.Exchange, ge.Exchange);
            Assert.AreEqual(newGE.Volley, ge.Volley);
            Assert.AreEqual(newGE.Turn, ge.Turn);
        }

        [TestMethod]
        public void TestPenetratingDamageRollup_Screen2_DRM_Minus_One()
        {
            IDiceUtility roller = new DiceNotationUtility() as IDiceUtility;
            roller.RollFTSuccesses(1);

            var units = new List<GameUnit>();
            var f = GameEngineTestUtilities.GenerateTestFormation(1, "Test Formation", ref units);
            var fu = f.Units.First();
            var u = units[0];

            var result = f.RollManeuverSpeedAndEvasion(f.Orders.First(), 1, f.FormationId, speedDRM: 0, evasionDRM: 0);
            Console.WriteLine($"{u.Name} rolls {result.SpeedSuccesses} for Speed and {result.EvasionSuccesses} for Evasion.");

            Console.WriteLine("Testing penetrating damage versus Screen Rating 2...");
            var damageResult = new DiceNotationUtility()
            .RollFTDamage(
                numberOfDice: 20,
                drm: -1,
                targetScreenRating: 2,
                dealPenetrating: true);
            Console.WriteLine($"Dealt a total of {damageResult.Standard} standard damage and {damageResult.Penetrating} penetrating damage.");

            Assert.IsNotNull(damageResult);

            // Since ScreenRating is 2 and DRM is -1, only 6's hit (and they all penetrate).
            int natural6sOnInitialRoll = damageResult.StandardRolls.Count(r => r == 6);
            int natural6sOnPenetratingRolls = damageResult.PenetratingRolls.Count(r => r == 6);
            int natural4sAnd5sOnPenetratingRolls = damageResult.PenetratingRolls.Count(r => r == 4 || r == 5);

            Assert.AreEqual(natural6sOnInitialRoll, damageResult.Standard);
            Assert.IsTrue(damageResult.PenetratingRolls.Count >= natural6sOnInitialRoll);

            // Penetration dice all come from 6s in the original roll, OR ELSE from rerolls in the penetration pool
            Assert.AreEqual(damageResult.PenetratingRolls.Count - natural6sOnPenetratingRolls, natural6sOnInitialRoll);

            // Penetration dice are unaffected by DRM or screens
            var penetrationDamageCalc = (2 * natural6sOnPenetratingRolls) + natural4sAnd5sOnPenetratingRolls;
            Assert.AreEqual(penetrationDamageCalc, damageResult.Penetrating);
        }

        [TestMethod]
        public void TestPenetratingDamageRollup_Screen1_DRM_Zero()
        {
            IDiceUtility roller = new DiceNotationUtility() as IDiceUtility;
            roller.RollFTSuccesses(1);

            var units = new List<GameUnit>();
            var f = GameEngineTestUtilities.GenerateTestFormation(1, "Test Formation", ref units);
            var fu = f.Units.First();
            var u = units[0];

            var result = f.RollManeuverSpeedAndEvasion(f.Orders.First(), f.FormationId, 1, speedDRM: 0, evasionDRM: 0);
            Console.WriteLine($"{u.Name} rolls {result.SpeedSuccesses} for Speed and {result.EvasionSuccesses} for Evasion.");

            Console.WriteLine("Testing penetrating damage versus Screen Rating 2...");
            var damageResult = new DiceNotationUtility()
            .RollFTDamage(
                numberOfDice: 20,
                drm: 0,
                targetScreenRating: 1,
                dealPenetrating: true);
            Console.WriteLine($"Dealt a total of {damageResult.Standard} standard damage and {damageResult.Penetrating} penetrating damage.");

            Assert.IsNotNull(damageResult);

            int natural6sOnInitialRoll = damageResult.StandardRolls.Count(r => r == 6);
            int natural5sOnInitialRoll = damageResult.StandardRolls.Count(r => r == 5);
            int natural4sOnInitialRoll = damageResult.StandardRolls.Count(r => r == 4);
            int natural6sOnPenetratingRolls = damageResult.PenetratingRolls.Count(r => r == 6);
            int natural5sOnPenetratingRolls = damageResult.PenetratingRolls.Count(r => r == 5);
            int natural4sOnPenetratingRolls = damageResult.PenetratingRolls.Count(r => r == 4);

            // Since ScreenRating is 1 and DRM is 0, 6s hit for 2 and penetrate; 5s hit for 1; and 4s miss.
            int expectedStandardDamage = (2 * natural6sOnInitialRoll) + natural5sOnInitialRoll + (0 * natural4sOnInitialRoll);

            // Penetrating damage ignores screens and DRM
            int expectedPenetratingDamage = (2 * natural6sOnPenetratingRolls) + natural5sOnPenetratingRolls + natural4sOnPenetratingRolls;

            Assert.AreEqual(expectedStandardDamage, damageResult.Standard);

            // Penetration dice all come from 6s in the original roll, OR ELSE from rerolls in the penetration pool
            Assert.AreEqual(damageResult.PenetratingRolls.Count - natural6sOnPenetratingRolls, natural6sOnInitialRoll);

            // Penetration dice are unaffected by DRM or screens
            Assert.AreEqual(expectedPenetratingDamage, damageResult.Penetrating);
        }

        [TestMethod]
        public void TestCloning()
        {
            // GameEngine uses a "cheat" and routes its clone method through a serialize/deserialize operation:
            GameEngine ge = new GameEngine()
            {
                Id = 99,
                Exchange = 99,
                Volley = 99,
                Turn = 99
            };
            GameEngine newGE = GameEngine.Clone(ge);
            newGE.Id = 0;
            newGE.Volley = 0;

            Assert.AreNotEqual(newGE.Id, ge.Id);
            Assert.AreNotEqual(newGE.Volley, ge.Volley);
            Assert.AreEqual(newGE.Exchange, ge.Exchange);
            Assert.AreEqual(newGE.Turn, ge.Turn);

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
            ((ArmorSystem)a2).TotalArmor = "5,5";
            ((ArmorSystem)a2).RemainingArmor = "4,4";
            var a2a = a1.Clone() as DefenseSystem;

            b2.Arcs = "AP, P, FP";
            d2.Status = "Damaged";

            Console.WriteLine($"B1 is a {b1.GetType().FullName} and its clone B2 is a {b2.GetType().FullName}");
            Assert.AreEqual(b1.GetType(), b2.GetType());
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

            // Ensure cloning results in no type changes
            Assert.AreEqual(d1.GetType(), d2.GetType());
            Assert.AreEqual(h1.GetType(), h2.GetType());
            Assert.AreEqual(a1.GetType(), a2.GetType());

            // Ensure cloning results in separate references
            Assert.AreNotEqual(h1.HullType, h1a.HullType);
            Assert.AreNotEqual(h1.HullType, h2.HullType);
            Assert.AreNotEqual(b1.Arcs, b2.Arcs);
            Assert.AreNotEqual(a1.TotalArmor, ((ArmorSystem)a2).TotalArmor);

            // Ensure cloning results in identical values unless changed
            Assert.AreEqual(h2.HullType, h2a.HullType);
        }
    }
}
