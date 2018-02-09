// <copyright file="ElectronicsSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class ElectronicsSystem : UnitSystem
    {
        public ElectronicsSystem()
        {
            this.SystemName = "Abstract Electronics System";
        }
    }
}