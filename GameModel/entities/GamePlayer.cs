// <copyright file="GamePlayer.cs" company="Patrick Maughan">
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
    public class GamePlayer
    {
        public GamePlayer()
        {
            this.Units = new List<GameUnit>();
        }

        [XmlAttribute("id")]
        public string Id { get; set; } = "0";

        [XmlAttribute("name")]
        public string Name { get; set; } = "Anonymous Coward";

        [XmlAttribute("email")]
        public string Email { get; set; } = "none@tempuri.org";

        [XmlAttribute("team")]
        public string Team { get; set; } = "none";

        [XmlAttribute("key")]
        public int Key { get; set; } = 0;

        public string Objectives { get; set; } = string.Empty;

        [XmlElement("Ship")]
        public List<GameUnit> Units { get; set; }

        internal static GamePlayer Clone(GamePlayer p)
        {
            // Copy all primitive types...
            var newP = (GamePlayer)p.MemberwiseClone();

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