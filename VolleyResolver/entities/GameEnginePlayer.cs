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
        public string id {get; set;}
        [XmlAttribute()]
        public string name {get; set;}
        [XmlAttribute()]
        public string email {get; set;}
        [XmlAttribute()]
        public string team {get; set;}
        public string Objectives {get; set;}
        [XmlElement("Ship")]
        public Unit[] Units { get; set; }

    }
}