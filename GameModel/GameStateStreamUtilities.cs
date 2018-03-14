// <copyright file="GameStateStreamUtilities.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
using System;
using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class GameStateStreamUtilities
    {
        public static GameState CloneGameState(GameState gameState)
        {
            // Cheat!
            XmlSerializer srz = new XmlSerializer(typeof(GameState));
            MemoryStream ms = new MemoryStream();
            GameStateStreamUtilities.SaveToStream(ms, gameState);

            var newGE = GameStateStreamUtilities.LoadFromStream(ms);

            return newGE;
        }

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

        public static GameState LoadFromXml(string sourceFile)
        {
            Stream stream = GetFileStreamFromPath(sourceFile);
            return LoadFromStream(stream);
        }

        public static GameState LoadFromStream(Stream sourceXml, string filename = "")
        {
            // Ensure incoming stream is reset to starting position for reading
            sourceXml.Position = 0;

            GameState ge = null;
            XmlSerializer srz = new XmlSerializer(typeof(GameState));
            TextReader reader = new StreamReader(sourceXml);

            try
            {
                ge = (GameState)srz.Deserialize(reader);
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
                        var newFu = new GameUnitFormationInfo(allUnits, fu.UnitId, f)
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
                        var targetF = ge.Formations.FirstOrDefault(t => t.FormationId == mo.TargetID);
                        if (targetF != null)
                        {
                            mo.TargetFormationName = targetF.FormationName;
                        }
                    }

                    foreach (var fo in o.FiringOrders)
                    {
                        var targetF = ge.Formations.FirstOrDefault(t => t.FormationId == fo.TargetID);
                        if (targetF != null)
                        {
                            fo.TargetFormationName = targetF.FormationName;
                        }
                    }
                }
            }

            return ge;
        }

        public static void SaveToFile(string sourceFile, GameState gameState)
        {
            FileInfo target = new FileInfo(sourceFile);
            if (!target.Directory.Exists)
            {
                target.Directory.Create();
            }

            FileStream fs = new FileStream(sourceFile, FileMode.Create);

            SaveToStream(fs, gameState);
        }

        public static void SaveToStream(Stream stream, GameState gameState)
        {
            TextWriter streamWriter = new StreamWriter(stream);

            XmlSerializer srz = new XmlSerializer(typeof(GameState));
            srz.Serialize(streamWriter, gameState);

            // Reset position so stream is ready for reading
            stream.Position = 0;
        }
    }
}
