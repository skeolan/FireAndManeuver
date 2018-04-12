// <copyright file="DefenseSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class DefenseSystem : UnitSystem
    {
        public DefenseSystem()
        {
            this.SystemName = "Abstract Defense System";
        }

        [XmlIgnore]
        public DefenseSpecialProperties SpecialProperties { get; set; } = DefenseSpecialProperties.None;

        [XmlIgnore]
        public int Rating { get; set; } = 0;
    }
}