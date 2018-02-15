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
                /*
                 * if (this.Units.Count > 0)
                {
                    this.maxThrust = this.Units.Max(u => u.MaxThrust);
                }
                */

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
    }
}
