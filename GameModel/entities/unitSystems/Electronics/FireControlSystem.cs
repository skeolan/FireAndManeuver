// <copyright file="FireControlSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    [XmlRoot("FireControl")]
    public class FireControlSystem : ElectronicsSystem
    {
        public FireControlSystem()
        {
            this.SystemName = "Fire Control System";
        }
    }
}