// <copyright file="GameUnitFireAllocation.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    [XmlRoot("FireAllocation")]
    public class GameUnitFireAllocation
    {
        [XmlAttribute]
        public int Volley { get; set; } = 0;

        [XmlAttribute]
        public int FireConId { get; set; } = 0;

        [XmlIgnore]
        public List<int> WeaponIDs
        {
            get
            {
                return this.WeaponIDsRaw.Split(',').Select(int.Parse).ToList();
            }

            set
            {
                this.WeaponIDsRaw = string.Join(",", value.Select(v => v.ToString()));
            }
        }

        [XmlAttribute]
        public string FireMode { get; set; } = "Normal";

        [XmlAttribute]
        public string Priority { get; set; } = "Primary";

        [XmlAttribute("WeaponIDs")]
        public string WeaponIDsRaw { get; set; } = "0";

        public override string ToString()
        {
            return $"v{this.Volley} -- FC[{this.FireConId}] -- {this.FireMode} -- {this.Priority} -- {this.WeaponIDsRaw}";
        }
    }
}
