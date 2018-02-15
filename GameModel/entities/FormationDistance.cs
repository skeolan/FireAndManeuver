// <copyright file="FormationDistance.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the current one-way range or distance to a Formation in the game space.
    /// </summary>
    [XmlRoot("Distance")]
    public class FormationDistance
    {
        public FormationDistance()
        {
        }

        [XmlAttribute("sourceId")]
        public int SourceFormationId { get; set; } = 0;

        [XmlAttribute("sourceName")]
        public string SourceFormationName { get; set; } = string.Empty;

        [XmlAttribute("targetId")]
        public int TargetFormationId { get; set; } = 0;

        [XmlAttribute("targetName")]
        public string TargetFormationName { get; set; } = string.Empty;

        [XmlText]
        public int Value { get; set; } = FireAndManeuver.GameModel.GameEngineOptions.DefaultStartingRange;

        public override string ToString()
        {
            var sourceString = string.IsNullOrWhiteSpace(this.SourceFormationName) ? this.SourceFormationId.ToString() : this.SourceFormationName;
            var targetString = string.IsNullOrWhiteSpace(this.TargetFormationName) ? this.TargetFormationId.ToString() : this.TargetFormationName;

            return $"{sourceString} <--> {targetString} : {this.Value} MU";
        }
    }
}