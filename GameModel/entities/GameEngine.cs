// <copyright file="GameEngine.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;
    using Microsoft.Extensions.Configuration;

    [XmlRoot("GameEngine")]
    public class GameEngine
    {
        public GameEngine()
        {
            this.Exchange = 1;
            this.Volley = 1;
            this.Turn = 1;
        }

        [XmlIgnore]
        public string SourceFile { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlElement("FM:Exchange")]
        public int Exchange { get; set; }

        [XmlElement("FM:Volley")]
        public int Volley { get; set; }

        [XmlAttribute("turn")]
        public int Turn
        {
            get { return this.Exchange; }
            set { this.Exchange = value; }
        } // necessary for compatibility with FTJava "turns"

        [XmlAttribute("combat")]
        public bool Combat { get; set; }

        public string Briefing { get; set; }

        public GameEngineOptions GameOptions { get; set; }

        public string Report { get; set; }

        [XmlElement("Player")]
        public GameEnginePlayer[] Players { get; set; }

        [XmlIgnore]
        public IEnumerable<GameUnit> AllUnits
        {
            get
            {
                foreach (GameEnginePlayer p in this.Players)
                {
                    foreach (GameUnit u in p.Units)
                    {
                        yield return u;
                    }
                }
            }
        }

        public static bool ShouldSerializeTurn()
        {
            return false;
        } // ... but our implementation shouldn't produce it

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
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? string.Empty);

                // throw ex;
            }

            return ge;
        }

        public static GameEngine Clone(GameEngine oldGE)
        {
            var newGE = (GameEngine)oldGE.MemberwiseClone();

            // Break connection to source file, if any
            newGE.SourceFile = string.Empty;

            // Deep copy of complex types
            newGE.GameOptions = GameEngineOptions.Clone(oldGE.GameOptions);

            List<GameEnginePlayer> newPlayers = new List<GameEnginePlayer>();
            foreach (var p in oldGE.Players)
            {
                newPlayers.Add(GameEnginePlayer.Clone(p));
            }

            newGE.Players = newPlayers.ToArray();

            return newGE;
        }

        public static GameEngine ResolveVolleys(GameEngine ge, IConfigurationRoot config, int volleyMax, string sourceFileName)
        {
            for (; ge.Volley <= volleyMax; ge.Volley++)
            {
                Console.WriteLine();
                Console.WriteLine($"VOLLEY {ge.Volley}");
                Console.WriteLine(string.Empty.PadRight(100, '*'));

                // ********
                // MANEUVER
                // ********
                ge.ExecuteManeuversForVolley(ge.Volley);

                // FIRE
                ge.ExecuteCombatForVolley(ge.Volley);

                // Record Volley Report
                RecordVolleyReport(ge, sourceFileName);

                Console.WriteLine(string.Empty.PadRight(100, '*'));
            }

            {
                // Set up for a new Exchange by clearing out this Exchange's scripting
                ge.Exchange++;
                ge.Volley = 1;

                ge.ClearOrders();
            }

            return ge;
        }

        public void SaveToFile(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));
            FileStream fs = new FileStream(sourceFile, FileMode.Create);
            srz.Serialize(fs, this);
        }

        public void ClearOrders()
        {
            foreach (GameEnginePlayer p in this.Players)
            {
                foreach (GameUnit u in p.Units)
                {
                }
            }
        }

        private static void RecordVolleyReport(GameEngine ge, string originalSourceFile)
        {
            var destFileName = Path.GetFileNameWithoutExtension(originalSourceFile) + $".VolleyResults.{ge.Volley}" + Path.GetExtension(originalSourceFile);
            var destFileFullName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                destFileName);
            ge.SourceFile = destFileFullName;
            Console.WriteLine($" - Volley interim report saving to:");
            Console.WriteLine($"          {ge.SourceFile}...");
            ge.SaveToFile(ge.SourceFile);
        }

        private bool ExecuteCombatForVolley(int currentVolley)
        {
            Console.WriteLine(" - FIRE SEGMENT");

            //--Point Defense Phase
            // TODO

            //--Weapon Attack Phase
            // TODO

            //--Damage and Threshold Checks Phase
            // TODO

            //--And return!
            return true;
        }

        private bool ExecuteManeuversForVolley(int currentVolley)
        {
            Console.WriteLine(" - MANEUVER SEGMENT");

            //--Launch Phase (Ordnance, Fighters, Gunboats)
            // TODO

            //--Movement Phase
            // -- Roll all maneuver tests
            foreach (var u in this.AllUnits)
            {
                Console.WriteLine($"  -- Process Maneuver orders for {u.ToString()}");
                var orders = u.Orders.FirstOrDefault(o => o.Volley == currentVolley);
                var maneuveringOrders = (orders ?? new VolleyOrders()).ManeuveringOrders;
                foreach (var o in maneuveringOrders)
                {
                    var type = o.ManeuverType;
                    var target = o.TargetID;
                    var priority = o.Priority;

                    Console.WriteLine($"   --- Execute {priority} {type} maneuver against Unit [{target}]");
                }
            }

            // -- Compare each maneuverer's test successes to its target's test successes

            // -- If a "Close" or "Withdraw" has won, adjust the range accordingly

            //--Secondary Movement Phase (Fighters and Gunboats only)
            // TODO
            return true;
        }
    }
}