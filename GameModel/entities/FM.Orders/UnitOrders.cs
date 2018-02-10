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

        [XmlAttribute("target")]
        public string TargetID { get; set; } = "*";

        [XmlAttribute("priority")]
        public string Priority { get; set; } = "secondary";

        public override string ToString()
        {
            string tgtStr = this.TargetID == "*" ? "Default" : $"Target:[{this.TargetID}]";
            string priStr = this.Priority.ToLowerInvariant() == "primary" ? "(Primary)" : string.Empty;
            return $"{tgtStr}{priStr}";
        }
    }
}