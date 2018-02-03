using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("Player")]
    public class GameEnginePlayer
    {
        [XmlAttribute()]
        public string id {get; set;} = "-1";
        [XmlAttribute()]
        public string name {get; set;} = "Anonymous Coward";
        [XmlAttribute()]
        public string email {get; set;} = "none@tempuri.org";
        [XmlAttribute()]
        public string team {get; set;} = "none";
        [XmlAttribute()]
        public int key {get; set;} = -1;
        public string Objectives {get; set;} = "";
        [XmlElement("Ship")]
        public List<GameUnit> Units { get; set; }

        public GameEnginePlayer()
        {
            Units = new List<GameUnit>();
        }

        internal static GameEnginePlayer Clone(GameEnginePlayer p)
        {
            //Copy all primitive types...
            var newP = (GameEnginePlayer)p.MemberwiseClone();

            //And clone all complex types.
            List<GameUnit> newUnits = new List<GameUnit>();
            foreach (var u in p.Units)
            {
                newUnits.Add(GameUnit.Clone(u));
            }

            return newP;
        }
    }
}