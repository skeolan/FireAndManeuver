using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("GameEngine")]
    public class GameEngine
    {
        [XmlIgnore]
        public string SourceFile { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlElement("FM:Exchange")]
        public int exchange { get; set; }
        [XmlElement("FM:Volley")]
        public int volley { get; set; }
        [XmlAttribute]
        public int turn { set { exchange = value; } private get { return exchange; } } //necessary for compatibility with FTJava "turns"
        public bool shouldSerializeturn() { return false; } //... but our implementation shouldn't produce it

        [XmlAttribute]
        public bool combat { get; set; }
        public string Briefing { get; set; }
        public GameEngineOptions GameOptions { get; set; }
        public string Report {get; set;}

        [XmlElement("Player")]
        public GameEnginePlayer[] Players { get; set; }

        public GameEngine()
        {
            exchange = 1;
            volley = 1;
        }


        public static GameEngine loadFromXml(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));

            FileStream fs;
            GameEngine ge = null;

            try
            {
                fs = new FileStream(sourceFile, FileMode.Open);
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            try
            {
                ge = (GameEngine)srz.Deserialize(fs);
                ge.SourceFile = sourceFile;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? "");
                //throw ex;
            }

            return ge;
        }
    }
}