// <copyright file="VolleyOrders.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("FM.VolleyOrders")]
    public class VolleyOrders
    {
        public VolleyOrders()
        {
            this.Speed = 0;
            this.Evasion = 0;
            this.ManeuveringInternal = new List<ManeuverOrder>();
            this.FiringInternal = new List<FireOrder>();
        }

        [XmlAttribute("volley")]
        public int Volley { get; set; } = 0;

        [XmlElement("FM.Speed")]
        public int Speed { get; set; }

        [XmlElement("FM.Evasion")]
        public int Evasion { get; set; }

        public List<ManeuverOrder> ManeuveringOrders
        {
            get
            {
                // Default maneuvering order is "Maintain vs. everyone"
                if (this.ManeuveringInternal == null || this.ManeuveringInternal.Count == 0)
                {
                    this.ManeuveringInternal = new List<ManeuverOrder>() { new ManeuverOrder() { ManeuverType = "Maintain", Priority = "default", TargetID = "*" } };
                }

                return this.ManeuveringInternal.OrderBy(x => x.Priority.ToLowerInvariant() != "primary").ToList();
            }

            set
            {
                this.ManeuveringInternal = value;
            }
        }

        public List<FireOrder> FiringOrders
        {
            get
            {
                return this.FiringInternal.OrderByDescending(x => x.TargetID.ToLowerInvariant() != "pd").OrderByDescending(x => x.Priority.ToLowerInvariant() == "primary").ToList();
            }

            set
            {
                this.FiringInternal = value;
            }
        }

        [XmlArray("FM.ManeuveringOrders")]
        [XmlArrayItemAttribute("FM.Maneuver", Type = typeof(ManeuverOrder))]
        internal List<ManeuverOrder> ManeuveringInternal { get; set; }

        [XmlArray("FM.FiringOrders")]
        [XmlArrayItemAttribute("FM.Fire", Type = typeof(FireOrder))]
        internal List<FireOrder> FiringInternal { get; set; } = new List<FireOrder> { };

        public override string ToString()
        {
            var maneuverStrings = string.Join("; ", this.ManeuveringOrders.Select(m => m.ToString()));
            var firingStrings = string.Join(";", this.FiringOrders.Select(f => f.ToString()));
            return $"Speed {this.Speed} - Evasion {this.Evasion} | Maneuvering: [{maneuverStrings}] | Firing: [{firingStrings}]";
        }

        internal static VolleyOrders Clone(VolleyOrders o)
        {
            throw new NotImplementedException();
        }
    }
}