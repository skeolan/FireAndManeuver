// <copyright file="CargoHoldSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class CargoHoldSystem : UnitSystem
    {
        public CargoHoldSystem()
            : base()
        {
            this.SystemName = "Cargo Hold System";
        }

        [XmlAttribute("type")]
        public string HoldType { get; set; }

        [XmlAttribute("totalSize")]
        public int TotalSize { get; set; }

        public override string ToString()
        {
            return string.Format("{0} [Total Size {1}]", base.ToString(), this.TotalSize);
        }
    }
}