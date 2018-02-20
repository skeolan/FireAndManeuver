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
        private readonly List<ManeuverOrder> defaultManeuverOrders = new List<ManeuverOrder>(Constants.DefaultManeuverOrders);
        private List<ManeuverOrder> maneuveringOrders;

        public VolleyOrders()
        {
            this.Volley = 0;
            this.Speed = 0;
            this.Evasion = 0;
            this.ManeuveringOrders = new List<ManeuverOrder>();
            this.FiringOrders = new List<FireOrder>();
        }

        [XmlAttribute("volley")]
        public int Volley { get; set; } = 0;

        [XmlAttribute("speed")]
        public int Speed { get; set; }

        [XmlAttribute("evasion")]
        public int Evasion { get; set; }

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
            return $"v{this.Volley} - Speed {this.Speed} - Evasion {this.Evasion} | Maneuvering: [{this.ManeuveringOrders.Count}] | Firing: [{this.FiringOrders.Count}]";
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