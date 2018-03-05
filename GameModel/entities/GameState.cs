// <copyright file="GameState.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("GameEngine")]
    public class GameState
    {
        public GameState()
        {
            this.Briefing = string.Empty;
            this.Combat = false;
            this.Distances = new List<FormationDistance>();
            this.Exchange = 1;
            this.Formations = new List<GameFormation>();
            this.DistanceGraph = new FormationDistanceGraph(this.Formations, this.Distances);
            this.GameOptions = new GameOptions();
            this.Id = 0;
            this.Report = string.Empty;
            this.SourceFile = string.Empty;
            this.Players = new List<GamePlayer>();
            this.Turn = 1;
            this.Volley = 1;
        }

        [XmlIgnore]
        public string SourceFile { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("FM.Exchange")]
        public int Exchange { get; set; }

        [XmlAttribute("FM.Volley")]
        public int Volley { get; set; }

        [XmlAttribute("turn")]
        public int Turn
        {
            get { return this.Exchange; }
            set { this.Exchange = value; }
        } // necessary for compatibility with FTJava "turns"

        [XmlAttribute("combat")]
        public bool Combat { get; set; }

        [XmlElement("Briefing")]
        public string Briefing { get; set; }

        [XmlElement("GameOptions")]
        public GameOptions GameOptions { get; set; }

        [XmlElement("Report")]
        public string Report { get; set; }

        [XmlElement("Player")]
        public List<GamePlayer> Players { get; set; } // TODO: make this a List<GameEnginePlayer>

        [XmlArray("FM.Distances")]
        [XmlArrayItem("Distance")]
        public List<FormationDistance> Distances { get; set; } = new List<FormationDistance>();

        [XmlIgnore]
        public IEnumerable<GameUnit> AllUnits
        {
            get
            {
                foreach (GamePlayer p in this.Players)
                {
                    foreach (GameUnit u in p.Units)
                    {
                        yield return u;
                    }
                }
            }
        }

        [XmlArray("FM.Formations")]
        [XmlArrayItem("Formation")]
        public List<GameFormation> Formations { get; set; } = new List<GameFormation>();

        [XmlIgnore]
        public FormationDistanceGraph DistanceGraph { get; set; }

        // i.e. to allow new orders to be input at the start of a new Exchange
        public void ClearOrders()
        {
            foreach (GamePlayer p in this.Players)
            {
                foreach (GameUnit u in p.Units)
                {
                    // TODO: Implement order-clearing for all Units
                }
            }

            foreach (GameFormation f in this.Formations)
            {
                // TODO: Implement order-clearing for all Formations
            }
        }
    }
}
