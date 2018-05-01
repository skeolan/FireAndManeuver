// <copyright file="AdvancedScreenSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class AdvancedScreenSystem : ScreenSystem
    {
        public AdvancedScreenSystem()
            : base()
        {
            this.Rating = 1;
            this.SystemName = "Advanced Screen System";
            this.ScreenRating = new ScreenRating(1, true);
        }
    }
}