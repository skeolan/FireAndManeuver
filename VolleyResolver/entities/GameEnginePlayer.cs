using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
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
        public List<Unit> Units { get; set; }

        public GameEnginePlayer()
        {
            Units = new List<Unit>();
        }

    }
}