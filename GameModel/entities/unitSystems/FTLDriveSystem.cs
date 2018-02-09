// <copyright file="FTLDriveSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : UnitSystem
    {
        public FTLDriveSystem()
        {
            this.SystemName = "FTL Drive System";
        }

        [XmlAttribute("active")]
        public bool Active { get; set; } = false;

        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), this.Active ? "Active" : "Inactive");
        }
    }
}