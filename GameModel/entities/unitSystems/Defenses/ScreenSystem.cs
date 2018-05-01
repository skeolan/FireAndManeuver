// <copyright file="ScreenSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class ScreenSystem : DefenseSystem
    {
        public ScreenSystem()
            : base()
        {
            this.Rating = 1;
            this.SystemName = "Screen System";
            this.ScreenRating = new ScreenRating(1, false);
        }

        [XmlIgnore]
        public ScreenRating ScreenRating { get; set; }
    }
}