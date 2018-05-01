// <copyright file="AreaScreenSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class AreaScreenSystem : ScreenSystem
    {
        public AreaScreenSystem()
            : base()
        {
            this.SpecialProperties = DefenseSpecialProperties.Area_Defense;
            this.SystemName = "Screen System";
        }
    }
}