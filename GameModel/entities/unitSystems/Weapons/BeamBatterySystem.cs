// <copyright file="BeamBatterySystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class BeamBatterySystem : ArcWeaponSystem
    {
        [XmlIgnore]
        private string arcs;

        public BeamBatterySystem()
            : base()
        {
            this.SystemName = "Beam Battery System";
            this.Rating = 1;
            this.Arcs = "(All arcs)";
        }

        public BeamBatterySystem(int rating)
        {
            this.SystemName = "Beam Battery System";
            this.Rating = rating;
            this.Arcs = rating == 1 ? "(All arcs)" : "(F)";
        }

        public BeamBatterySystem(int rating, string arcs)
        {
            this.SystemName = "Beam Battery System";
            this.Rating = rating;
            this.Arcs = arcs;
        }

        [XmlAttribute("rating")]
        public int Rating { get; set; } = 1;

        [XmlAttribute("arcs")]
        public override string Arcs
        {
            get
            {
                var arcString = this.Rating == 1 ? "(All arcs)" : string.IsNullOrWhiteSpace(this.arcs) ? "(F)" : this.arcs;
                return arcString;
            }

            set
            {
                this.arcs = value;
            }
        }
    }
}