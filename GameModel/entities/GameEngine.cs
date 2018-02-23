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
                f.MaxThrust = newInfo.Select(u => u.MaxThrust).DefaultIfEmpty(0).Max();
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

        public static GameEngine ResolveVolley(GameEngine ge, IConfigurationRoot config, int volley, string sourceFileName)
        {
            ge.Volley = volley;

            // MANEUVER
            ge.ExecuteManeuversForVolley(ge.Volley);

            // FIRE
            ge.ExecuteCombatForVolley(ge.Volley);

            return ge;
        }

        public static void RecordVolleyReport(GameEngine ge, string originalSourceFile)
        {
            var destFileName = Path.GetFileNameWithoutExtension(originalSourceFile) + $".VolleyResults.{ge.Volley}" + Path.GetExtension(originalSourceFile);
            var destFileFullName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                destFileName);
            ge.SourceFile = destFileFullName;
            Console.WriteLine($" - Volley {ge.Volley} interim report saving to:");
            Console.WriteLine($"          {ge.SourceFile}");
            ge.SaveToFile(ge.SourceFile);
        }

        public static void RecordExchangeReport(GameEngine ge, string originalSourceFile)
        {
            var destFileName = Path.GetFileNameWithoutExtension(originalSourceFile) + $".ExchangeResults.{ge.Exchange}" + Path.GetExtension(originalSourceFile);
            var destFileFullName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                destFileName);
            ge.SourceFile = destFileFullName;
            Console.WriteLine($" - Exchange {ge.Exchange} completed report saving to:");
            Console.WriteLine($"          {ge.SourceFile}");
            ge.SaveToFile(ge.SourceFile);
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

        private static int GetManeuverModifier(string priority)
        {
            return priority.ToLowerInvariant() == Constants.PrimaryManeuverPriority.ToLowerInvariant() ? 0 : -1;
        }

        private bool ExecuteCombatForVolley(int currentVolley)
        {
            Console.WriteLine(" - FIRE SEGMENT");

            // TODO -- Point Defense Phase

            // TODO -- Weapon Attack Phase

            // TODO -- Damage and Threshold Checks Phase

            //--And return!
            return true;
        }

        private bool ExecuteManeuversForVolley(int currentVolley)
        {
            Console.WriteLine(" - MANEUVER SEGMENT");

            Dictionary<int, ManeuverSuccessSet> maneuverResultsById = new Dictionary<int, ManeuverSuccessSet>();

            //--Launch Phase (Ordnance, Fighters, Gunboats)
            // TODO

            //--Movement Phase

            // --- Determine Speed and Evasion successes for all Units
            foreach (var f in this.Formations)
            {
                Console.WriteLine($"Roll Speed and Evasion for {f.FormationName}, volley {currentVolley}");
                var formationOrders = f.Orders.Where(o => o.Volley == currentVolley).FirstOrDefault() ?? Constants.DefaultVolleyOrders;

                // Side effect: sets the ManeuverSuccesses and SpeedSuccesses property of the formation's current VolleyOrders.
                var result = f.RollManeuverSpeedAndEvasion(formationOrders, f.FormationId, currentVolley, speedDRM: 0, evasionDRM: 0);
                maneuverResultsById.Add(f.FormationId, result);
            }

            Console.WriteLine();

            // -- Adjudicate all maneuver tests
            foreach (var f in this.Formations)
            {
                this.ExecuteManeuversForFormation(currentVolley, f);
            }

            //--Secondary Movement Phase (Fighters and Gunboats only)
            // TODO
            return true;
        }

        private void ExecuteManeuversForFormation(int currentVolley, GameFormation f)
        {
            var orders = f.Orders.FirstOrDefault(o => o.Volley == currentVolley) ?? Constants.DefaultVolleyOrders;
            Console.WriteLine($"Execute ({orders.ManeuveringOrders.Count}) Maneuvers for [{f.FormationId}]{f.FormationName} -- Speed {orders.SpeedSuccesses}, Evasion {orders.EvasionSuccesses}");

            // Orders should get evaluated in priority order: Primary, then non-default non-primary, then default
            var sortedOrders = orders.ManeuveringOrders.OrderByDescending(o => o.Priority.ToLowerInvariant() == Constants.PrimaryManeuverPriority.ToLowerInvariant() && o.Priority.ToLowerInvariant() != Constants.DefaultManeuverPriority.ToLowerInvariant());

            foreach (var o in sortedOrders)
            {
                var target = this.Formations.Where(t => t.FormationId.ToString() == o.TargetID).FirstOrDefault();

                var speed = Math.Max(0, orders.SpeedSuccesses + GameEngine.GetManeuverModifier(o.Priority));

                if (target == null || o.ManeuverType == Constants.PassiveManeuverType)
                {
                    // Bail early if the maneuver is a passive one, e.g. "Maintain"
                    // Bail early if the maneuver is not against a valid target Id (e.g. "Target 0" default orders or orders against Formations that have been destroyed)
                    Console.WriteLine($"   --- Skip {o.Priority} {o.ManeuverType} ({speed}s) vs [{o.TargetID}]");

                    continue;
                }

                this.ResolveManeuverContest(currentVolley, orders.SpeedSuccesses, o, f, target);
            }
        }

        private void ResolveManeuverContest(int currentVolley, int sourceSpeed, ManeuverOrder sourceManeuver, GameFormation source, GameFormation target)
        {
            (var rangeBetweenFormations, var rangeReciprocal) = this.GetGraphState(source, target);

            this.SyncGraphState(ref rangeBetweenFormations, ref rangeReciprocal, source, target);

            int rangeShift = this.CalculateRangeShift(currentVolley, source, sourceSpeed, sourceManeuver, target);

            rangeBetweenFormations.Value += rangeShift;
            rangeReciprocal.Value += rangeShift;
            Console.WriteLine($"    ---- Range between Formations changes to  [{rangeBetweenFormations.Value}|{rangeReciprocal.Value}]");
        }

        private int CalculateRangeShift(int currentVolley, GameFormation source, int sourceSpeed, ManeuverOrder sourceManeuver, GameFormation target)
        {
            var type = sourceManeuver.ManeuverType;
            var priority = sourceManeuver.Priority;
            var speed = Math.Max(0, sourceSpeed + GameEngine.GetManeuverModifier(sourceManeuver.Priority));

            Console.Write($"   --- Execute [{source.FormationId}]{source.FormationName} : {priority} {type} ({speed}s)");

            var targetOrders = target.Orders
                .Where(tO => tO.Volley == currentVolley)
                .FirstOrDefault() ?? Constants.DefaultVolleyOrders;
            var targetManeuver = targetOrders.ManeuveringOrders
                .Where(tRO => tRO.TargetID == source.FormationId.ToString())
                .FirstOrDefault() ?? Constants.DefaultManeuverOrder;

            var opposingPriority = targetManeuver.Priority;
            var opposingType = targetManeuver.ManeuverType;
            var opposingSpeed = Math.Max(0, targetOrders.SpeedSuccesses + GameEngine.GetManeuverModifier(targetManeuver.Priority));

            Console.WriteLine($" vs {opposingPriority} {opposingType} ({opposingSpeed}s) [{target.FormationId}]{target.FormationName}");

            var margin = speed - opposingSpeed;
            var marginDescription = margin > 0 ? $"wins by {margin}" : $"loses by {Math.Abs(margin)}";
            var rangeShift = 0;
            var successDistance = sourceManeuver.ManeuverType == "Close" ? -Constants.RangeShiftPerSuccess : Constants.RangeShiftPerSuccess;

            var maneuverTypePair = new ValueTuple<string, string>(sourceManeuver.ManeuverType, targetManeuver.ManeuverType);

            // Unopposed auto-successes
            if (maneuverTypePair.Equals(Constants.CloseVersusClose) || maneuverTypePair.Equals(Constants.WithdrawVersusWithdraw))
            {
                // Ignore opponent's Speed successes, they will be resolved separately
                rangeShift = successDistance * speed;
                marginDescription = "wins unopposed";
            }

            // "True" versus test -- if active F got more successes, distance changes; otherwise not.
            if (maneuverTypePair.Equals(Constants.CloseVersusMaintain) || maneuverTypePair.Equals(Constants.CloseVersusWithdraw) || maneuverTypePair.Equals(Constants.WithdrawVersusClose) || maneuverTypePair.Equals(Constants.WithdrawVersusMaintain))
            {
                rangeShift = successDistance * Math.Max(0, margin);

                // Ignore cases where opponent's successes were higher, they will be resolved separately (if not a Maintain already)
            }

            Console.WriteLine($"    ---- {sourceManeuver.ManeuverType} versus {targetManeuver.ManeuverType} {marginDescription}, range change: {rangeShift} MU");

            return rangeShift;
        }

        private Tuple<FormationDistance, FormationDistance> GetGraphState(GameFormation source, GameFormation target)
        {
            var rangeBetweenFormations = this.Distances.Where(d => d.SourceFormationId == source.FormationId && d.TargetFormationId == target.FormationId).FirstOrDefault();
            var rangeReciprocal = this.Distances.Where(d => d.SourceFormationId == target.FormationId && d.TargetFormationId == source.FormationId).FirstOrDefault();

            return new Tuple<FormationDistance, FormationDistance>(rangeBetweenFormations, rangeReciprocal);
        }

        private void SyncGraphState(ref FormationDistance rangeBetweenFormations, ref FormationDistance rangeReciprocal, GameFormation source, GameFormation target)
        {
            // If neither the source -> target nor target -> source distances are in the Distances graph, create them at default starting range
            if (rangeBetweenFormations is null && rangeReciprocal is null)
            {
                var newRange = new FormationDistance(source, target, Constants.DefaultStartingRange);
                var newReciprocal = new FormationDistance(target, source, Constants.DefaultStartingRange);

                this.Distances.Add(newRange);
            }
            else
            {
                // If either source -> target or target -> source is missing but the other is present, add the missing one
                if (rangeBetweenFormations is null)
                {
                    rangeBetweenFormations = new FormationDistance(source, target, rangeReciprocal.Value);
                    this.Distances.Add(rangeBetweenFormations);
                }

                if (rangeReciprocal is null)
                {
                    rangeReciprocal = new FormationDistance(target, source, rangeBetweenFormations.Value);
                    this.Distances.Add(rangeReciprocal);
                }
            }

            // If distances don't match, the graph is in a bad state
            if (rangeBetweenFormations.Value != rangeReciprocal.Value)
            {
                throw new InvalidOperationException("Range between Formations is not symmetrical, something went wrong in updating the distance graph.");
            }
        }
    }
}
