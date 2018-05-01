// <copyright file="BeamBatterySystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class BeamBatterySystem : ArcWeaponSystem
    {
        [XmlIgnore]
        private const string BaseSystemName = "Beam Battery System";

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
            this.SystemName = $"Beam Battery System";
            this.Rating = rating;

            // Default is "all arcs" for a B1
            this.Arcs = rating == 1 ? "(All arcs)" : "(F)";
        }

        public BeamBatterySystem(int rating, string arcs)
        {
            this.SystemName = $"Beam Battery System";
            this.Rating = rating;
            this.Arcs = arcs;
        }

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

        public override string ToString()
        {
            this.SystemName = $"Class-{this.Rating} {BaseSystemName}";
            return base.ToString();
        }
    }
}