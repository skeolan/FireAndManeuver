// <copyright file="ConsoleReadoutUtilities.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.Common.ConsoleUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FireAndManeuver.GameModel;

    public static class ConsoleReadoutUtilities
    {
        private const bool SuppressTitleLineForHullMatrix = true;
        private const int ReadoutWidth = 150; // 90 is the absolute minimum for a reasonable console output, otherwise some lines will blow out the right side.
        private const int HangingIndentWidth = 2;
        private static string boundary = string.Empty.PadRight(ReadoutWidth, '*');
        private static string separator = $"* {string.Empty.PadRight(ReadoutWidth - 4, '-')} *";
        private static string outputFormat = "* {0, -19} : {1, -" + (ReadoutWidth - 24 - 2).ToString() + "} *";
        private static string collectionItemOutputFormat = "* {0, -19} > {1, -" + (ReadoutWidth - 24 - 2).ToString() + "} *";
        private static string itemLeadString = "*                      > ";

        public static List<string> GenerateUnitReadout(GameUnit myUnit, List<GameUnit> allUnits = null)
        {
            List<string> readout = new List<string>()
            {
            };

            readout.Add(string.Empty);
            readout.Add(boundary);
            readout.Add($"* {myUnit.ToString(), -(ReadoutWidth - 4)} *");

            readout.Add(separator);

            readout.Add(string.Format(outputFormat, "Status", myUnit.Status));
            readout.Add(string.Format(outputFormat, "Screen Rating", myUnit.GetLocalScreenRating()));
            readout.Add(string.Format(outputFormat, "Area Screen Rating", myUnit.GetAreaScreenRating()));
            readout.Add(string.Format(outputFormat, "Armor", myUnit.Armor.ToString()));
            readout.Add(string.Format(outputFormat, "Hull", myUnit.Hull.ToString()));
            readout.AddRange(PrintReadoutCollection("Hull", myUnit.Hull.HullDisplay(), SuppressTitleLineForHullMatrix));
            readout.Add(string.Format(outputFormat, "Crew", myUnit.CrewQuality));

            readout.Add(separator);

            readout.Add(string.Format(outputFormat, "MainDrive", myUnit.MainDrive.ToString()));
            readout.Add(string.Format(outputFormat, "FTLDrive", myUnit.FtlDrive.ToString()));

            readout.Add(separator);

            readout.AddRange(PrintReadoutCollection("Electronics", myUnit.Electronics));
            readout.AddRange(PrintReadoutCollection("Defenses", myUnit.Defenses));
            readout.AddRange(PrintReadoutCollection("Holds", myUnit.Holds));
            readout.AddRange(PrintReadoutCollection("Weapons", myUnit.Weapons));
            readout.AddRange(PrintReadoutCollection("Log", myUnit.Log));

            readout.Add(separator);
            readout.Add($"* {"Source: " + myUnit.SourceFile, -(ReadoutWidth - 4)} *");
            readout.Add(boundary);

            return readout;
        }

        public static List<string> GenerateGameEngineReadout(GameState ge, bool printUnitDetails = false)
        {
            List<string> readout = new List<string>();
            var allUnits = ge.AllUnits.ToList<GameUnit>();

            readout.Add(" BEGIN READOUT ".PadLeft((ReadoutWidth + " BEGIN READOUT ".Length) / 2, '^').PadRight(ReadoutWidth, '^'));
            readout.Add(string.Empty);

            // Game Engine header block
            readout.Add("* Game Info *".PadRight(ReadoutWidth, '*'));
            readout.Add("* " + $"Game [{ge.Id}] Exchange {ge.Exchange} / Volley {ge.Volley} -- Combat:{ge.Combat}".PadRight(ReadoutWidth - 4) + " *");
            readout.Add(boundary);
            readout.Add(string.Empty);

            // Briefing block
            readout.Add("* Briefing *".PadRight(ReadoutWidth, '*'));
            readout.AddRange(WrapDecorated(ge.Briefing, ReadoutWidth, "* ", " *", HangingIndentWidth));
            readout.Add(boundary);
            readout.Add(string.Empty);

            // Report block
            readout.Add("* Report *".PadRight(ReadoutWidth, '*'));
            readout.AddRange(WrapDecorated(string.IsNullOrEmpty(ge.Report) ? "..." : ge.Report, ReadoutWidth, "* ", " *", HangingIndentWidth));
            readout.Add(boundary);
            readout.Add(string.Empty);

            if (printUnitDetails)
            {
                // Unit detail blocks
                foreach (var p in ge.Players)
                {
                    foreach (var u in p.Units)
                    {
                        readout.Add($"* Player [{p.Id}]{p.Name} Unit Detail: *".PadRight(ReadoutWidth, '*'));
                        readout.Add($"* <{u.ToString(), -(ReadoutWidth - 6)}> *");
                        var unitReadout = GenerateUnitReadout(u, allUnits);
                        readout.AddRange(unitReadout);
                        readout.Add(string.Empty);
                    }
                }

                readout.Add(boundary);
            }

            // Formation readout block
            foreach (var f in ge.Formations)
            {
                readout.Add($"* Formation [{f.FormationId}]{f.FormationName} detail: *".PadRight(ReadoutWidth, '*'));
                readout.AddRange(GenerateFormationReadout(f, allUnits, ge.Players));
                readout.Add(boundary);
            }

            // Distance readout block
            {
                readout.Add($"* Formation distances: *".PadRight(ReadoutWidth, '*'));
                readout.AddRange(GenerateDistanceReadout(ge.Distances));
                readout.Add(boundary);
            }

            // Orders readout block
            foreach (var f in ge.Formations)
            {
                readout.Add($"* Formation orders for {f.FormationName}: *".PadRight(ReadoutWidth, '*'));
                readout.AddRange(GenerateOrdersReadout(f, allUnits, ge.Formations));
                readout.Add(boundary);
            }

            readout.Add(" END READOUT ".PadLeft((ReadoutWidth + " END READOUT ".Length) / 2, '^').PadRight(ReadoutWidth, '^'));

            return readout;
        }

        public static List<string> GenerateDistanceReadout(List<FormationDistance> input)
        {
            var readout = new List<string>();
            var distances = new List<FormationDistance>(input);
            var maxNameLength = input.Max(f => Math.Max(f.SourceFormationName.Length, f.TargetFormationName.Length));

            while (distances.Count > 0)
            {
                var node = distances.First();
                var mirrorNode = distances.Where(n => node.SourceFormationId == n.TargetFormationId && node.TargetFormationId == n.SourceFormationId).FirstOrDefault();

                distances.Remove(node);
                if (mirrorNode != null)
                {
                    distances.Remove(mirrorNode);
                }

                readout.Add($"[{node.SourceFormationId}]{node.SourceFormationName.PadRight(maxNameLength)} <- {node.Value, 3} MU -> [{node.TargetFormationId}] {node.TargetFormationName.PadRight(maxNameLength)}");
            }

            return readout;
        }

        public static List<string> WrapDecorated(string text, int margin, string prefix, string suffix, int hangingIndentLength = 0)
        {
            int start = 0, end;
            var lines = new List<string>();
            text = Regex.Replace(text, @"\s", " ").Trim();

            margin = margin - prefix.Length - suffix.Length;
            int indent = 0;

            while ((end = start + margin - indent) < text.Length)
            {
                while (text[end] != ' ' && end > start)
                {
                    end -= 1;
                    var candidate = text.Substring(start, end - start);
                }

                if (end == start)
                {
                    end = start + margin;
                }

                lines.Add(prefix + string.Empty.PadRight(indent) + text.Substring(start, end - start).PadRight(margin - indent) + suffix);
                start = end + 1;

                indent = hangingIndentLength;
            }

            if (start < text.Length)
            {
                lines.Add(prefix + string.Empty.PadRight(indent) + text.Substring(start).PadRight(margin - indent) + suffix);
            }

            return lines;
        }

        private static List<string> PrintReadoutCollection<T>(string collectionName, List<T> coll, bool suppressTitleLine = false)
        {
            List<string> outputLines = new List<string>();
            switch (coll.Count)
            {
                case 0:
                    { /* print nothing */
                        break;
                    }

                default:
                    {
                        // multiple entries needs a multi-line printout
                        if (!suppressTitleLine)
                        {
                            outputLines.AddRange(WrapDecorated($"{collectionName, -16}({coll.Count, 2})", ReadoutWidth, "* ", " *"));
                        }

                        foreach (var sys in coll)
                        {
                            outputLines.AddRange(WrapDecorated(sys.ToString(), ReadoutWidth, "*                      > ", " *", HangingIndentWidth));
                        }

                        break;
                    }
            }

            return outputLines;
        }

        private static List<string> PrintOrders<T>(string collectionName, List<T> coll, string outputFormat, GameUnit thisUnit = null, List<GameUnit> allUnits = null)
        {
            var output = new List<string>();

            if (coll.Count > 0)
            {
                output.Add($"* {$"{collectionName, -16}({coll.Count, 2})", -(ReadoutWidth - 4)} *");
            }

            foreach (var orderT in coll)
            {
                var o = (FormationOrder)(object)orderT;
                string readoutLine = o.ToString();
                readoutLine = readoutLine.Replace("Target:[PD]", "PD");
                if (allUnits != null)
                {
                    var orderTarget = allUnits.Where(u => u.IdNumeric == o.TargetID).FirstOrDefault();

                    if (orderTarget != null)
                    {
                        readoutLine = readoutLine.Replace($"Target:[{o.TargetID}]", orderTarget.InstanceName);
                    }
                }

                output.Add(string.Format(outputFormat, string.Empty, readoutLine));
                if (thisUnit != null && orderT.GetType() == typeof(FireOrder))
                {
                    /*foreach (var weaponID in ((FireOrder)o).WeaponIDs.Split(','))
                    {
                        // Fire orders have additional lines of output indicating assigned weapons by ID
                        var weapon = thisUnit.Weapons.Where(w => $"{w.Id}" == $"{weaponID}").FirstOrDefault();
                        var weaponText = weapon == null ? $"Weapon [{weaponID:00}]" : weapon.SystemName;
                        var weaponEntry = $"{string.Empty, 21} - {weaponText, -30} [{weaponID, 2}]";
                        output.Add(string.Format(outputFormat, string.Empty, weaponEntry));
                    }*/

                    output.Add(string.Format(outputFormat, string.Empty, string.Empty));
                }
            }

            return output;
        }

        private static List<string> GenerateOrdersReadout(GameFormation f, List<GameUnit> allUnits, List<GameFormation> formations)
        {
            List<string> readout = new List<string>();
            foreach (var volleyOrder in f.Orders)
            {
                readout.AddRange(WrapDecorated(volleyOrder.ToString(), ReadoutWidth, "* ", " *", 5));
                readout.AddRange(PrintReadoutCollection(string.Empty, volleyOrder.ManeuveringOrders, true));

                foreach (var fire in volleyOrder.FiringOrders)
                {
                    var leadString = itemLeadString;
                    var fireOrderString = ToTitleCase($"{fire.FireType} Order: '{fire.Priority}' -- Target: [{fire.TargetID}] {fire.TargetFormationName}{(fire.DiceAssigned > 0 ? $" ({fire.DiceAssigned}D)" : string.Empty)}");
                    readout.AddRange(WrapDecorated(fireOrderString, ReadoutWidth, itemLeadString, " *", 5));
                    foreach (var unit in f.Units)
                    {
                        var unitForOrder = allUnits.FirstOrDefault(x => x.IdNumeric == unit.UnitId);
                        if (unitForOrder != null)
                        {
                            var unitOrderFireAllocations = unitForOrder.FireAllocation.Where(fireAlloc => (fireAlloc.Volley == volleyOrder.Volley || fireAlloc.Volley == 0) && fireAlloc.Priority.ToLowerInvariant() == fire.Priority.ToLowerInvariant());
                            foreach (var allocation in unitOrderFireAllocations)
                            {
                                var fc = unitForOrder.AllSystems.FirstOrDefault(afc => afc.Id == allocation.FireConId) ?? new FireControlSystem() { Status = UnitSystemStatus.Operational };
                                var line = new List<string>() { $">     {unit.UnitName} -- FC({fc.StatusString})" };
                                foreach (var w in allocation.WeaponIDs)
                                {
                                    var wep = unitForOrder.Weapons.First(uw => uw.Id == w);
                                    line.Add($">>     {wep.ToString()}");
                                }

                                readout.AddRange(PrintReadoutCollection(string.Empty, line, true));
                            }
                        }
                    }
                }
            }

            return readout;
        }

        private static List<string> GenerateFormationReadout(GameFormation f, List<GameUnit> allUnits, List<GamePlayer> players)
        {
            var readout = new List<string>();

            var player = players.Where(p => p.Id == f.PlayerId.ToString()).FirstOrDefault() ?? new GamePlayer() { Name = "Unknown Player" };

            var playerStrings = new List<string>()
            {
                $"Player {player.Id}",
                player.Team,
                (player.Key <= 0 ? player.Name : $"{player.Name} [{player.Key}]"),
                player.Email,
                string.IsNullOrWhiteSpace(player.Objectives) ? null : $"\"{player.Objectives}\""
            }.Where(s => !string.IsNullOrWhiteSpace(s));
            readout.AddRange(WrapDecorated(string.Join(" -- ", playerStrings), ReadoutWidth, "* ", " *", 5));

            readout.AddRange(WrapDecorated($"Area Screen Rating '{f.GetFormationAreaScreenRating()}'", ReadoutWidth, "* ", " *"));
            readout.AddRange(WrapDecorated($"MaxThrust '{f.MaxThrust}'", ReadoutWidth, "* ", " *"));
            readout.AddRange(WrapDecorated($"Units ({f.Units.Count})", ReadoutWidth, "* ", " *"));
            foreach (var u in f.Units)
            {
                var uR = allUnits.Where(ur => ur.IdNumeric == u.UnitId).First();
                var unitString = $"- {uR.ToString()} ({uR.Status}) -- Thrust {uR.GetCurrentThrust()}";
                if (uR.GetCurrentThrust() > f.MaxThrust)
                {
                    unitString += $" ({f.MaxThrust}+{u.ExtraThrust})";
                }

                if (u.IsFormationFlag)
                {
                    unitString += $" -- Flagship";
                }

                unitString += $" -- HitChance {f.GetHitChancePercentage(u.UnitId):0.##}%";

                readout.AddRange(WrapDecorated(unitString, ReadoutWidth, "*   ", " *", 5));

                readout.AddRange(PrintReadoutCollection("Fire Allocations", uR.FireAllocation));
            }

            return readout;
        }

        private static string ToTitleCase(this string s)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }
    }
}
