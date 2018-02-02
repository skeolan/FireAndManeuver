using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public int turn { set { exchange = value; } get { return exchange; } } //necessary for compatibility with FTJava "turns"
        public bool shouldSerializeturn() { return false; } //... but our implementation shouldn't produce it

        [XmlAttribute]
        public bool combat { get; set; }
        public string Briefing { get; set; }
        public GameEngineOptions GameOptions { get; set; }
        public string Report { get; set; }

        [XmlElement("Player")]
        public GameEnginePlayer[] Players { get; set; }

        [XmlIgnore]
        public IEnumerable<Unit> AllUnits
        {
            get
            {
                foreach (GameEnginePlayer p in Players)
                {
                    foreach (Unit u in p.Units)
                    {
                        yield return u;
                    }
                }
            }
        }

        public GameEngine()
        {
            exchange = 1;
            volley = 1;
        }


        public static GameEngine LoadFromXml(string sourceFile)
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

        public void SaveToFile(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));
            FileStream fs = new FileStream(sourceFile, FileMode.Create);
            srz.Serialize(fs, this);
        }

        public static GameEngine Clone(GameEngine oldGE)
        {
            var newGE = (GameEngine) oldGE.MemberwiseClone();
            
            //Break connection to source file, if any
            newGE.SourceFile = "";

            //Deep copy of complex types
            newGE.GameOptions = GameEngineOptions.Clone(oldGE.GameOptions);

            List<GameEnginePlayer> newPlayers = new List<GameEnginePlayer>();
            foreach(var P in oldGE.Players)
            {
                newPlayers.Add(GameEnginePlayer.Clone(P));
            }
            newGE.Players = newPlayers.ToArray();
            

            return newGE;
        }


        public static GameEngine ResolveVolleys(GameEngine ge, IConfigurationRoot config, int VolleyMax, string sourceFileName)
        {
            for (var i = ge.volley; i <= VolleyMax; i++)
            {
                Console.WriteLine($"VOLLEY {i}");
                //********
                //MANEUVER
                //********
                ge.ExecuteManeuversForVolley(i);


                //FIRE
                ExecuteCombatForVolley(ge);

                //Record Volley Report
                RecordVolleyReport(ge, sourceFileName);

                ge.volley = (++ge.volley >= VolleyMax) ? VolleyMax : ge.volley;
            }

            Debug.Assert(ge.volley == VolleyMax, $"Volley resolution ran volley count to {ge.volley}, which is higher than {VolleyMax}");

            {
                //Set up for a new Exchange by clearing out this Exchange's scripting
                ge.exchange++;
                ge.volley = 1;

                ge.ClearOrders();
            }

            return ge;
        }

        private static void RecordVolleyReport(GameEngine ge, string originalSourceFile)
        {
            var destFileName = Path.GetFileNameWithoutExtension(originalSourceFile) + $".VolleyResults.{ge.volley}" + Path.GetExtension(originalSourceFile);
            var destFileFullName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                destFileName
                );
            ge.SourceFile = destFileFullName;
            Console.WriteLine($"Volley interim report saving to {ge.SourceFile}...");
            ge.SaveToFile(ge.SourceFile);
        }

        private bool ExecuteManeuversForVolley(int currentVolley)
        {
            Console.WriteLine("MANEUVER SEGMENT");

            //--Launch Phase (Ordnance, Fighters, Gunboats)
            //TODO

            //--Movement Phase
            // -- Roll all maneuver tests
            foreach (var u in AllUnits)
            {
                Console.WriteLine($"  --  Process Maneuver orders for {u.ToString()}");
                var orders = u.Orders.FirstOrDefault(o => o.volley == currentVolley);
                var maneuveringOrders = (orders ?? new VolleyOrders()).ManeuveringOrders;
                foreach (var o in maneuveringOrders)
                {
                    var type = o.ManeuverType;
                    var target = o.TargetID;
                    var priority = o.Priority;

                    Console.WriteLine($"Execute {priority} {type} maneuver against Unit [{target}]");
                }
            }

            // -- Compare each maneuverer's test successes to its target's test successes

            //   -- If a "Close" or "Withdraw" has won, adjust the range accordingly

            //--Secondary Movement Phase (Fighters and Gunboats only)
            //TODO

            return true;
        }
        private static void ExecuteCombatForVolley(GameEngine newGE)
        {
            Console.WriteLine("FIRE SEGMENT");
            //--Point Defense Phase
            //TODO

            //--Weapon Attack Phase

            //--Damage and Threshold Checks Phase
        }

        public void ClearOrders()
        {
            foreach(GameEnginePlayer p in Players)
            {
                foreach(Unit u in p.Units)
                {
                    
                }
            }
        }
    }
}