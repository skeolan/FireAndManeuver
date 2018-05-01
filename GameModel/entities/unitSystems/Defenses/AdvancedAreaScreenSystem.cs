// <copyright file="AdvancedAreaScreenSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class AdvancedAreaScreenSystem : AreaScreenSystem
    {
        public AdvancedAreaScreenSystem()
            : base()
        {
            this.SpecialProperties = DefenseSpecialProperties.Area_Defense;
            this.SystemName = "Advanced Area Screen System";
        }
    }
}