// <copyright file="ArcWeaponSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class ArcWeaponSystem : WeaponSystem
    {
        public ArcWeaponSystem()
            : base()
        {
            this.SystemName = "Abstract Arc-Firing Weapon System";
            this.Arcs = "(F)";
        }

        [XmlAttribute("arcs")]
        public virtual string Arcs { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} - {this.Arcs}";
        }
    }
}