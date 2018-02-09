// <copyright file="PointDefenseSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class PointDefenseSystem : WeaponSystem
    {
        public PointDefenseSystem()
            : base()
        {
            this.SystemName = "Point Defense System";
        }
    }
}