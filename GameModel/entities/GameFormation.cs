// <copyright file="GameFormation.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using FireAndManeuver.Common;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    [XmlRoot("Formation")]
    public class GameFormation
    {
        private int maxThrust = 0;

        public GameFormation()
        {
        }

        [XmlAttribute("id")]
        public int FormationId { get; set; } = 0;

        [XmlAttribute("playerId")]
        public int PlayerId { get; set; } = 0;

        [XmlAttribute("name")]
        public string FormationName { get; set; } = "Default Formation";

        [XmlAttribute("maxThrust")]
        public int MaxThrust
        {
            get
            {
                if (this.Units.Count > 0)
                {
                    this.maxThrust = this.Units.Min(u => u.MaxThrust);
                }

                return this.maxThrust;
            }

            set
            {
                this.maxThrust = value; // ... but going to get overwritten on "get" calls
            }
        }

        [XmlArray("Units")]
        [XmlArrayItem("Unit")]
        public List<GameUnitFormationInfo> Units { get; set; } = new List<GameUnitFormationInfo>();

        [XmlArray("Orders")]
        [XmlArrayItem("VolleyOrders")]
        public List<VolleyOrders> Orders { get; set; } = new List<VolleyOrders>();

        public GameFormation Clone()
        {
            /*
            var newF = this.MemberwiseClone() as GameFormation;

            newF.Orders = new List<VolleyOrders>(newF.Orders.Count);
            newF.Units = new List<GameUnitFormationInfo>(newF.Units.Count);

            var newO = newF.Orders.Select( o => o.Clone());
            */

            // Cheat!
            XmlSerializer srz = new XmlSerializer(typeof(GameFormation));
            MemoryStream ms = new MemoryStream();
            srz.Serialize(ms, this);

            var newF = srz.Deserialize(ms) as GameFormation;

            return newF;
        }

        public int GetTotalMass()
        {
            return this.Units.Sum(u => u.Mass);
        }

        public Tuple<int, int> GetHitRangeForUnit(int targetUnitId)
        {
            // Default case: unit is the only one in the Formation
            Tuple<int, int> hitRange = new Tuple<int, int>(1, 100);

            // Don't bother evaluating unless there are >1 units
            if (this.Units.Count > 1)
            {
                int hitFloor = 0;
                int hitCeiling = 0;

                foreach (var formationMember in this.Units)
                {
                    var hitCoverage = this.GetHitChancePercentage(formationMember.UnitId);

                    if (formationMember.UnitId != targetUnitId)
                    {
                        // Target lies further down the list, exclude percentage covered by this unit
                        hitFloor += hitCoverage;
                        hitCeiling = hitFloor;
                    }
                    else
                    {
                        hitCeiling = Math.Min(hitFloor + hitCoverage, 100);

                        hitFloor = hitFloor + 1;

                        hitRange = new Tuple<int, int>(hitFloor, hitCeiling);

                        break;
                    }
                }
            }

            return hitRange;
        }

        public int GetHitChancePercentage(int unitID)
        {
            var unit = this.Units.Where(u => u.UnitId == unitID).FirstOrDefault() ?? throw new InvalidOperationException($"ID provided ({unitID}) is not found in Formation's Unit list");

            var hitChance = (int)Math.Round(((decimal)unit.Mass / this.GetTotalMass() * (decimal)100) + (decimal)unit.HitModifier);

            return Math.Max(hitChance, 1); // Hit chance is never less than 1%
        }

        public ManeuverSuccessSet RollManeuverSpeedAndEvasion(IServiceProvider services, VolleyOrders formationOrders, int formationId, int currentVolley, int speedDRM = 0, int evasionDRM = 0)
        {
            var logger = services.GetLogger();
            var roller = services.GetService<IDiceUtility>();
            var rolls = new List<int>();
            var builder = new StringBuilder();

            builder.Append($" -- [{this.FormationId}]{this.FormationName} rolls {formationOrders.SpeedDice}D for Speed: ");
            formationOrders.SpeedSuccesses = roller.RollFTSuccesses(formationOrders.SpeedDice, out rolls);
            builder.AppendLine($" -- {formationOrders.SpeedSuccesses}s ({string.Join(", ", rolls)})");

            builder.Append($" -- [{this.FormationId}]{this.FormationName} rolls {formationOrders.EvasionDice}D for Evasion: ");
            formationOrders.EvasionSuccesses = roller.RollFTSuccesses(formationOrders.EvasionDice, out rolls);
            builder.AppendLine($" -- {formationOrders.EvasionSuccesses}s ({string.Join(", ", rolls)})");

            logger.LogInformation(builder.ToString());

            return new ManeuverSuccessSet() { FormationId = formationId, Volley = currentVolley, SpeedSuccesses = formationOrders.SpeedSuccesses, EvasionSuccesses = formationOrders.EvasionSuccesses };
        }

        public VolleyOrders GetOrdersForVolley(int currentVolley)
        {
            return this.Orders.Where(o => o.Volley == currentVolley).FirstOrDefault() ?? Constants.DefaultVolleyOrders;
        }
    }
}
