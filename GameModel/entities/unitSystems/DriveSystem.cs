// <copyright file="DriveSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    [XmlRoot("MainDrive")]
    public class DriveSystem : UnitSystem
    {
        private int? currentThrustInternal = null;

        public DriveSystem()
            : base()
        {
            this.SystemName = "Standard Drive System";
            this.DriveType = "Standard";
            this.InitialThrust = 0;
            this.CurrentThrust = 0;
            this.Active = true;
        }

        public DriveSystem(int thrustRating, bool advanced = false)
        {
            this.SystemName = "Standard Drive System";
            this.DriveType = advanced ? "Advanced" : "Standard";
            this.InitialThrust = thrustRating;
            this.CurrentThrust = thrustRating;
            this.Active = true;
        }

        [XmlAttribute("type")]
        public string DriveType { get; set; } = "Standard";

        [XmlAttribute("initialThrust")]
        public int InitialThrust { get; set; } = 0;

        [XmlAttribute("currentThrust")]
        public int CurrentThrust
        {
            get { return this.currentThrustInternal ?? this.InitialThrust; }
            set { this.currentThrustInternal = value; }
        }

        [XmlAttribute("active")]
        public bool Active { get; set; } = false;

        public override string ToString()
        {
            return $"{base.ToString()} - {(this.Active ? "Active" : "Inactive")}";
        }
    }
}