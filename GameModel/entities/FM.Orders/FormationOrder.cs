// <copyright file="FormationOrder.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public abstract class FormationOrder
    {
        public FormationOrder()
        {
        }

        [XmlAttribute("targetId")]
        public int TargetID { get; set; } = 0;

        [XmlAttribute("priority")]
        public string Priority { get; set; } = string.Empty;

        [XmlAttribute("targetName")]
        public string TargetFormationName { get; set; } = string.Empty;

        public override string ToString()
        {
            string tgtStr = this.TargetID == 0 ? "Default" : $"Target:[{this.TargetID}]{this.TargetFormationName}";
            return $"{tgtStr} - Priority:{this.Priority}";
        }
    }
}