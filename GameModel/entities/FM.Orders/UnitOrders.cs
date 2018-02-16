// <copyright file="UnitOrders.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class UnitOrders
    {
        public UnitOrders()
        {
        }

        [XmlAttribute("targetId")]
        public string TargetID { get; set; } = "0";

        [XmlAttribute("priority")]
        public string Priority { get; set; } = string.Empty;

        [XmlAttribute("targetName")]
        public string TargetFormationName { get; set; } = string.Empty;

        public override string ToString()
        {
            string tgtStr = this.TargetID == "*" ? "Default" : $"Target:[{this.TargetID}]{this.TargetFormationName}";
            return $"{tgtStr} - Priority:{this.Priority}";
        }
    }
}