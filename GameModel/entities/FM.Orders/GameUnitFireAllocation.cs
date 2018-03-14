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
        public const string DefaultFireMode = "Fire";
        public const string DefaultFirePriority = "Primary";

        public GameUnitFireAllocation()
        {
            this.Volley = 0;
            this.FireConId = 0;
            this.FireMode = GameUnitFireAllocation.DefaultFireMode;
            this.Priority = "Primary";
        }

        public GameUnitFireAllocation(int volley = 0, int fireConId = 0, string fireMode = GameUnitFireAllocation.DefaultFireMode, string priority = GameUnitFireAllocation.DefaultFirePriority, List<int> weaponIds = null)
        {
            this.Volley = volley;
            this.FireConId = fireConId;
            this.FireMode = fireMode;
            this.Priority = priority;
            if (weaponIds == null)
            {
                this.WeaponIDsRaw = "0";
            }
            else
            {
                this.WeaponIDs = weaponIds;
            }
        }

        [XmlAttribute]
        public int Volley { get; set; } = 0;

        [XmlAttribute]
        public int FireConId { get; set; } = 0;

        [XmlIgnore]
        public List<int> WeaponIDs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.WeaponIDsRaw))
                {
                    return new List<int>();
                }
                else
                {
                    return this.WeaponIDsRaw.Split(',').Select(int.Parse).ToList();
                }
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
        public string WeaponIDsRaw { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"v{this.Volley} -- FC[{this.FireConId}] -- {this.FireMode} -- {this.Priority} -- {this.WeaponIDsRaw}";
        }
    }
}
