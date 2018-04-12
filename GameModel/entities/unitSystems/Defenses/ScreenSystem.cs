// <copyright file="ScreenSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class ScreenSystem : DefenseSystem
    {
        public ScreenSystem()
        {
            this.Rating = 1;
            this.SystemName = "Screen System";
        }
    }
}