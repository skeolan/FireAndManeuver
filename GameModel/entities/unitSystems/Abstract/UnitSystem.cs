// <copyright file="UnitSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class UnitSystem
    {
        public UnitSystem()
        {
            this.Status = UnitSystemStatus.Operational;
        }

        [XmlIgnore]
        public string SystemName { get; protected set; } = "Abstract base Unit System";

        [XmlAttribute("id")]
        public int Id { get; set; } = 0;

        [XmlAttribute("xSSD")]
        public int SSDXCoordinate { get; set; }

        [XmlAttribute("ySSD")]
        public int SSDYCoordinate { get; set; }

        [XmlAttribute("status")]
        public string StatusString
        {
            get => this.Status.ToString();
            set => this.Status = (UnitSystemStatus)System.Enum.Parse(typeof(UnitSystemStatus), value, true);
        }

        [XmlIgnore]
        public UnitSystemStatus Status { get; set; }

        public override string ToString()
        {
            string idStr = this.Id == 0 ? string.Empty : string.Format("[{0:00}]", this.Id);
            return $"{idStr, 2} - {this.SystemName, -30} - {this.StatusString, -12}";
        }

        // As long as all properties are primitive type, no need to override this for derived classes
        public virtual dynamic Clone()
        {
            return (dynamic)this.MemberwiseClone();
        }
    }
}