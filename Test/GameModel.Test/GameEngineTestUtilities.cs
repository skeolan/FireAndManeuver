// <copyright file="GameEngineTestUtilities.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FireAndManeuver.GameModel;

    public static class GameEngineTestUtilities
    {
        /// <summary>
        /// Produces a <see cref="GameUnit"/> representing a NAC Harrison-class Scoutship,
        ///   pretty much the simplest FTL-capable ship that can move and shoot
        /// </summary>
        /// <param name="id">Desired <see cref="GameUnit.IdNumeric"/> of the new unit.</param>
        /// <param name="name">Desired <see cref="GameUnit.InstanceName"/> of the new unit.</param>
        /// <param name="faction">Desired <see cref="GameUnit.Race"/> of the new unit. Defaults to null, which coalesces to "NAC".</param>
        /// <returns>A simple Harrison-class Scoutship <see cref="GameUnit"/> with appropriately initialized values.</returns>
        public static GameUnit GenerateTestUnit(int id, string name, string faction = null)
        {
            int currentSystemId = 1;

            var u = new GameUnit()
            {
                IdNumeric = id,
                Name = string.IsNullOrWhiteSpace(name) ? "Test Unit" : name,
                ClassName = "Harrison",
                ShipClass = "Scoutship",
                ClassAbbrev = "SC",
                Race = faction ?? "NAC",
                Status = "OK",
                CrewQuality = "First Rate",
                Mass = 6,
                PointValue = 21,

                Electronics = new List<ElectronicsSystem>() { new FireControlSystem() { Id = currentSystemId++ } },
                FtlDrive = new FTLDriveSystem() { Id = currentSystemId++ },
                Hull = new HullSystem() { Id = currentSystemId++, HullClass = "Military", HullType = HullTypeLookup.Average },
                MainDrive = new DriveSystem(4) { Id = currentSystemId++ },
                Weapons = new List<WeaponSystem>() { new BeamBatterySystem(1, "(All arcs)") { Id = currentSystemId++ } },
            };

            return u;
        }

        /// <summary>
        /// Generates a <see cref="GameFormation"/> with a single <see cref="GameUnit"/> via <see cref="GenerateTestUnit"/>
        ///   and assigns a default, simple set of <see cref="VolleyOrders"/> to it for Volley 1; also assigns the new GameUnit
        ///   a set of <see cref="GameUnitFireAllocation"/> orders to match up with the <see cref="FireOrder"/>s.
        /// </summary>
        /// <param name="id">Desired ID of the new formation</param>
        /// <param name="name">Desired name of the new formation</param>
        /// <param name="unitList">Reference to a list of GameUnits, so that the Formation's new GameUnit can be added to it.</param>
        /// <param name="targetFormationId">Optionally, specify the ID of a target Formation for this Formation's Orders to specify. Default is 0.</param>
        /// <returns>The new Formation with included <see cref="GameUnitFormationInfo"/> for the new GameUnit. Side Effect: adds the new GameUnit to <paramref name="unitList"/></returns>
        public static GameFormation GenerateTestFormation(int id, string name, ref List<GameUnit> unitList, int targetFormationId = 0)
        {
            GameUnit u = GenerateTestUnit(id: 1, name: $"{name}-1");

            unitList.Add(u);

            var f = new GameFormation()
            {
                FormationId = id,
                FormationName = name,
                MaxThrust = 99,
                Orders = new List<VolleyOrders>(),
                PlayerId = 0,
                Units = new List<GameUnitFormationInfo>()
                {
                    new GameUnitFormationInfo(u)
                }
            };

            var fOrder = new VolleyOrders()
            {
                Volley = 1,
                EvasionDice = 2,
                SpeedDice = 4,
                FiringOrders = new List<FireOrder>()
                {
                    new FireOrder()
                    {
                        TargetID = targetFormationId,
                        FireType = "Normal",
                        Priority = "Primary",
                        TargetFormationName = string.Empty,
                        DiceAssigned = 0
                    }
                },
                ManeuveringOrders = new List<ManeuverOrder>()
                {
                    new ManeuverOrder()
                    {
                        TargetID = targetFormationId,
                        ManeuverType = "Close",
                        Priority = "Primary"
                    }
                }
            };

            u.FireAllocation = new List<GameUnitFireAllocation>() { new GameUnitFireAllocation(volley: 1, fireConId: u.Electronics.OfType<FireControlSystem>().First().Id, fireMode: "Normal", priority: "Primary", weaponIds: u.Weapons.Select(w => w.Id).ToList()) };

            f.Orders.Add(fOrder);

            return f;
        }

        public static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}
