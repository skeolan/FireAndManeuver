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

        public decimal GetTotalMass()
        {
            return this.Units.Sum(u => u.Mass);
        }

        public decimal GetHitChancePercentage(int unitID)
        {
            var unit = this.Units.Where(u => u.UnitId == unitID).FirstOrDefault() ?? throw new InvalidOperationException($"ID provided ({unitID}) is not found in Formation's Unit list");

            var hitChance = ((decimal)unit.Mass / this.GetTotalMass() * (decimal)100) + (decimal)unit.HitModifier;

            return hitChance;
        }

        public ManeuverSuccessSet RollManeuverSpeedAndEvasion(VolleyOrders formationOrders, int formationId, int currentVolley, int speedDRM = 0, int evasionDRM = 0)
        {
            var roller = new DiceNotationUtility() as IDiceUtility;

            Console.Write($" -- [{this.FormationId}]{this.FormationName} rolls {formationOrders.SpeedDice}D for Speed: ");
            formationOrders.SpeedSuccesses = roller.RollFTSuccesses(formationOrders.SpeedDice);
            Console.WriteLine($" -- {formationOrders.SpeedSuccesses}s");

            Console.Write($" -- [{this.FormationId}]{this.FormationName} rolls {formationOrders.EvasionDice}D for Evasion: ");
            formationOrders.EvasionSuccesses = roller.RollFTSuccesses(formationOrders.EvasionDice);
            Console.WriteLine($" -- {formationOrders.EvasionSuccesses}s");

            return new ManeuverSuccessSet() { FormationId = formationId, Volley = currentVolley, SpeedSuccesses = formationOrders.SpeedSuccesses, EvasionSuccesses = formationOrders.EvasionSuccesses };
        }

        public VolleyOrders GetOrdersForVolley(int currentVolley)
        {
            return this.Orders.Where(o => o.Volley == currentVolley).FirstOrDefault() ?? Constants.DefaultVolleyOrders;
        }
    }
}
