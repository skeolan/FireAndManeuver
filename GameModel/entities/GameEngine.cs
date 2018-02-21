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

            this.Briefing = string.Empty;
            this.Combat = false;
            this.Distances = new List<FormationDistance>();
            this.Exchange = 1;
            this.Formations = new List<GameFormation>();
            this.GameOptions = new GameEngineOptions();
            this.Id = 0;
            this.Report = string.Empty;
            this.SourceFile = string.Empty;
            this.Players = new List<GameEnginePlayer>();
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
        public GameEngineOptions GameOptions { get; set; }

        [XmlElement("Report")]
        public string Report { get; set; }

        [XmlElement("Player")]
        public List<GameEnginePlayer> Players { get; set; } // TODO: make this a List<GameEnginePlayer>

        [XmlArray("FM.Distances")]
        [XmlArrayItem("Distance")]
        public List<FormationDistance> Distances { get; set; } = new List<FormationDistance>();

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

        [XmlArray("FM.Formations")]
        [XmlArrayItem("Formation")]
        public List<GameFormation> Formations { get; set; } = new List<GameFormation>();

        public static FileStream GetFileStreamFromPath(string sourceFile)
        {
            FileStream fs;

            try
            {
                fs = new FileStream(sourceFile, FileMode.Open);
            }
            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            return fs;
        }

        public static GameEngine LoadFromXml(string sourceFile)
        {
            Stream stream = GetFileStreamFromPath(sourceFile);
            return LoadFromStream(stream);
        }

        public static GameEngine LoadFromStream(Stream sourceXml, string filename = "")
        {
            // Ensure incoming stream is reset to starting position for reading
            sourceXml.Position = 0;

            GameEngine ge = null;
            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));
            TextReader reader = new StreamReader(sourceXml);

            try
            {
                ge = (GameEngine)srz.Deserialize(reader);
                ge.SourceFile = filename;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML input from '{0}' is not a supported Unit design: {1} -- {2}", filename, ex.Message, ex.InnerException.Message ?? string.Empty);

                throw ex;
            }

            // Instantiate references
            var allUnits = ge.AllUnits.ToList<GameUnit>();

            // FormationUnitInfo -> Unit
            foreach (var f in ge.Formations)
            {
                List<GameUnitFormationInfo> newInfo = new List<GameUnitFormationInfo>(f.Units.Count);
                var flagshipFound = false;

                foreach (var fu in f.Units)
                {
                    try
                    {
                        var newFu = new GameUnitFormationInfo(allUnits, fu.UnitId)
                        {
                            IsFormationFlag = fu.IsFormationFlag,
                            HitModifier = fu.HitModifier
                        };
                        newInfo.Add(newFu);
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.Error.WriteLine($"Invalid GameEngine data: GameUnit with Id [{fu.UnitId}] listed as a FormationUnit in Formation [{f.FormationId}] but not found in list of GameUnits: {e.Message}");
                        throw e;
                    }

                    if (fu.IsFormationFlag)
                    {
                        flagshipFound = true;
                    }
                }

                // Default to first Unit in list if not flagship found
                if (!flagshipFound && newInfo.FirstOrDefault() != null)
                {
                    var newFlag = newInfo.First();
                    newFlag.IsFormationFlag = true;
                }

                // Calculate formation-wide max-thrust and per-unit Excess Thrust once all units are dereferenced
                f.Units = newInfo;
                f.MaxThrust = newInfo.Max(u => u.MaxThrust);
                f.Units.ForEach(u => u.ExtraThrust = Math.Max(u.MaxThrust - f.MaxThrust, 0));
            }

            // Distance references -> Formations
            foreach (var d in ge.Distances)
            {
                var sourceRealF = ge.Formations.Where(f => d.SourceFormationId == f.FormationId).First();
                var targetRealF = ge.Formations.Where(f => d.TargetFormationId == f.FormationId).First();

                // All ID References in the Distances collection MUST resolve to a real Formation
                if (sourceRealF == null || targetRealF == null)
                {
                    var invalidIds = new List<string>();
                    if (sourceRealF == null)
                    {
                        invalidIds.Add(d.SourceFormationId.ToString());
                    }

                    if (targetRealF == null)
                    {
                        invalidIds.Add(d.TargetFormationId.ToString());
                    }

                    throw new InvalidDataException($"Distance entry {d.ToString()} lists IDs which are not valid: {string.Join(", ", invalidIds)}");
                }

                d.SourceFormationName = sourceRealF.FormationName;
                d.TargetFormationName = targetRealF.FormationName;
            }

            // Order targets dereference to correct FormationNames by Id
            foreach (var f in ge.Formations)
            {
                foreach (var o in f.Orders)
                {
                    foreach (var mo in o.ManeuveringOrders)
                    {
                        var targetF = ge.Formations.FirstOrDefault(t => t.FormationId.ToString() == mo.TargetID);
                        if (targetF != null)
                        {
                            mo.TargetFormationName = targetF.FormationName;
                        }
                    }

                    foreach (var fo in o.FiringOrders)
                    {
                        var targetF = ge.Formations.FirstOrDefault(t => t.FormationId.ToString() == fo.TargetID);
                        if (targetF != null)
                        {
                            fo.TargetFormationName = targetF.FormationName;
                        }
                    }
                }
            }

            return ge;
        }

        public static GameEngine Clone(GameEngine ge)
        {
            // Cheat!
            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));
            MemoryStream ms = new MemoryStream();
            ge.SaveToStream(ms);

            var newGE = GameEngine.LoadFromStream(ms);

            return newGE;
        }

        public static GameEngine DeepClone(GameEngine oldGE, bool forceImplementation = false)
        {
            if (!forceImplementation)
            {
                throw new NotImplementedException("Finish this method!");
            }

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

            newGE.Players = newPlayers;

            // Formations
            newGE.Formations = new List<GameFormation>(oldGE.Formations.Count);
            foreach (var f in oldGE.Formations)
            {
                newGE.Formations.Add(f.Clone());
            }

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
            FileStream fs = new FileStream(sourceFile, FileMode.Create);
            this.SaveToStream(fs);
        }

        public void SaveToStream(Stream stream)
        {
            TextWriter streamWriter = new StreamWriter(stream);

            XmlSerializer srz = new XmlSerializer(typeof(GameEngine));
            srz.Serialize(streamWriter, this);

            // Reset position so stream is ready for reading
            stream.Position = 0;
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
            Console.WriteLine($"          {ge.SourceFile}");
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

            var speedSuccessesById = new Dictionary<int, int>(this.AllUnits.Count());
            var evasionSuccessesById = new Dictionary<int, int>(this.AllUnits.Count());

            //--Launch Phase (Ordnance, Fighters, Gunboats)
            // TODO

            //--Movement Phase
            foreach (var f in this.Formations)
            {
                Console.WriteLine($"Resolve maneuver for {f.FormationName}, volley {currentVolley}");
                /*
                var result = f.ResolveManeuver(f.Orders.FirstOrDefault(), speedDRM: 0, evasionDRM: 0);

                speedSuccessesById.Add(u.IdNumeric, result.SpeedSuccesses);
                evasionSuccessesById.Add(u.IdNumeric, result.EvasionSuccesses);

                Console.WriteLine($"{u.InstanceName} rolls {result.SpeedSuccesses} for Speed and {result.EvasionSuccesses} for Evasion.");
                */
            }

            // -- Adjudicate all maneuver tests
            foreach (var f in this.Formations)
            {
                Console.WriteLine($"  -- Process Maneuver orders for [{f.FormationId}]{f.FormationName}");
                var orders = f.Orders.FirstOrDefault(o => o.Volley == currentVolley);
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