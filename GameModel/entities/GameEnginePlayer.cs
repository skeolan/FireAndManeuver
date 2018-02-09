// <copyright file="GameEnginePlayer.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("Player")]
    public class GameEnginePlayer
    {
        public GameEnginePlayer()
        {
            this.Units = new List<GameUnit>();
        }

        [XmlAttribute("id")]
        public string Id { get; set; } = "-1";

        [XmlAttribute("name")]
        public string Name { get; set; } = "Anonymous Coward";

        [XmlAttribute("email")]
        public string Email { get; set; } = "none@tempuri.org";

        [XmlAttribute("team")]
        public string Team { get; set; } = "none";

        [XmlAttribute("key")]
        public int Key { get; set; } = -1;

        public string Objectives { get; set; } = string.Empty;

        [XmlElement("Ship")]
        public List<GameUnit> Units { get; set; }

        internal static GameEnginePlayer Clone(GameEnginePlayer p)
        {
            // Copy all primitive types...
            var newP = (GameEnginePlayer)p.MemberwiseClone();

            // And clone all complex types.
            List<GameUnit> newUnits = new List<GameUnit>();
            foreach (var u in p.Units)
            {
                newUnits.Add(GameUnit.Clone(u));
            }

            return newP;
        }
    }
}