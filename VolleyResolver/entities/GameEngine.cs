using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("GameEngine")]
    public class GameEngine
    {
        [XmlIgnore]
        public string SourceFile {get; set;}
        [XmlAttribute]
        public int id {get; set;}
        public string Briefing { get; set; }
        public GameEngineOptions GameOptions { get; set; }
        [XmlElement("Player")]
        public GameEnginePlayer[] Players { get; set; }


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