// <copyright file="VolleyOrders.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("FM.VolleyOrders")]
    public class VolleyOrders
    {
        private readonly List<ManeuverOrder> defaultManeuverOrders = new List<ManeuverOrder>() { Constants.DefaultManeuverOrder };
        private List<ManeuverOrder> maneuveringOrders;

        public VolleyOrders()
        {
            this.Volley = 0;
            this.SpeedDice = 0;
            this.EvasionDice = 0;
            this.ManeuveringOrders = new List<ManeuverOrder>(Constants.DefaultManeuverOrders);
            this.FiringOrders = new List<FireOrder>();
        }

        public VolleyOrders(int volley, int speed, int evasion, List<ManeuverOrder> maneuveringOrders, List<FireOrder> firingOrders)
        {
            this.Volley = 0;
            this.SpeedDice = speed;
            this.EvasionDice = evasion;
            this.ManeuveringOrders = maneuveringOrders;
            this.FiringOrders = firingOrders;
        }

        [XmlAttribute("volley")]
        public int Volley { get; set; } = 0;

        [XmlAttribute("evasion")]
        public int EvasionDice { get; set; }

        [XmlAttribute("speed")]
        public int SpeedDice { get; set; }

        [XmlIgnore]
        public int EvasionSuccesses { get; set; } = 0;

        [XmlIgnore]
        public int SpeedSuccesses { get; set; } = 0;

        [XmlElement("Maneuver")]
        public List<ManeuverOrder> ManeuveringOrders
        {
            get
            {
                return this.maneuveringOrders.Count == 0 ? this.defaultManeuverOrders : this.maneuveringOrders;
            }

            set
            {
                this.maneuveringOrders = value;
            }
        }

        [XmlElement("Fire")]
        public List<FireOrder> FiringOrders { get; set; }

        public override string ToString()
        {
            return $"v{this.Volley} - Speed {this.SpeedDice} - Evasion {this.EvasionDice} | Maneuvering: [{this.ManeuveringOrders.Count}] | Firing: [{this.FiringOrders.Count}]";
        }

        public IOrderedEnumerable<ManeuverOrder> GetSortedManeuveringOrders()
        {
            // Console.WriteLine($"({this.ManeuveringOrders.Count}) Maneuvers -- Speed {this.SpeedSuccesses}, Evasion {this.EvasionSuccesses}");

            // Orders should get evaluated in priority order: Primary, then non-default non-primary, then default
            var sortedOrders = this.ManeuveringOrders.OrderByDescending(o => o.Priority.ToLowerInvariant() == Constants.PrimaryManeuverPriority.ToLowerInvariant() && o.Priority.ToLowerInvariant() != Constants.DefaultManeuverPriority.ToLowerInvariant());

            return sortedOrders;
        }

        internal static VolleyOrders Clone(VolleyOrders o)
        {
            return o.Clone();
        }

        internal VolleyOrders Clone()
        {
            XmlSerializer srz = new XmlSerializer(typeof(VolleyOrders));
            MemoryStream ms = new MemoryStream();
            srz.Serialize(ms, this);

            var clone = srz.Deserialize(ms) as VolleyOrders;

            return clone;
        }
    }
}