using System;
using System.Linq;
using System.Collections.Generic;
using FireAndManeuver.GameModel;
using System.Text.RegularExpressions;

namespace FireAndManeuver.Common
{
    public static class ConsoleReadoutUtilities
    {

        const bool SUPPRESS_TITLE_LINE_FOR_HULLMATRIX = true;
        const int READOUT_WIDTH = 100; //90 is the absolute minimum for a reasonable console output, otherwise some lines will blow out the right side.
        const int HANGING_INDENT_WIDTH = 2;
        static string boundary = "".PadRight(READOUT_WIDTH, '*');
        static string separator = $"* {"".PadRight(READOUT_WIDTH-4, '-')} *";
        static string outputFormat = "* {0, -19} : {1, -"+(READOUT_WIDTH - 24 - 2).ToString()+"} *";
        static string collectionItemOutputFormat = "* {0, -19} > {1, -"+(READOUT_WIDTH - 24 - 2).ToString()+"} *";

        public static List<string> generateUnitReadout(GameUnit myUnit, List<GameUnit> allUnits = null)
        {

            List<string> readout = new List<string>();

            readout.Add("");
            readout.Add(boundary);
            readout.Add($"* {myUnit.ToString(), -(READOUT_WIDTH - 4)} *");
            readout.Add(separator);
            readout.Add(String.Format(outputFormat, "Status", myUnit.Status));
            readout.Add(String.Format(outputFormat, "Armor", myUnit.Armor.ToString()));
            readout.Add(String.Format(outputFormat, "Hull", myUnit.Hull.ToString()));
            readout.AddRange(printReadoutCollection("Hull", myUnit.Hull.HullDisplay(), collectionItemOutputFormat, SUPPRESS_TITLE_LINE_FOR_HULLMATRIX));
            readout.Add(String.Format(outputFormat, "Crew", myUnit.CrewQuality));
            readout.Add(separator);
            readout.Add(String.Format(outputFormat, "MainDrive", myUnit.MainDrive.ToString()));
            readout.Add(String.Format(outputFormat, "FTLDrive", myUnit.FtlDrive.ToString()));
            readout.Add(separator);
            readout.AddRange(printReadoutCollection("Electronics", myUnit.Electronics, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Defenses", myUnit.Defenses, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Holds", myUnit.Holds, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Weapons", myUnit.Weapons, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Log", myUnit.Log, collectionItemOutputFormat));
            readout.Add(separator);
            readout.Add($"* {(myUnit.Orders.Count > 0 ? "Orders" : "(No Orders)"), -(READOUT_WIDTH - 4)} *");
            foreach (var volleyOrders in myUnit.Orders)
            {
                readout.Add($"* {$"  Volley #{volleyOrders.volley}", -(READOUT_WIDTH - 4) } *");
                readout.Add($"* {$"     Speed  {volleyOrders.Speed} - Evasion {volleyOrders.Evasion}", -(READOUT_WIDTH - 4)} *");
                readout.AddRange(printOrders("     Maneuvering", volleyOrders.ManeuveringOrders, collectionItemOutputFormat, myUnit, allUnits));
                readout.AddRange(printOrders("     Firing", volleyOrders.FiringOrders, collectionItemOutputFormat, myUnit, allUnits));
            }
            readout.Add(separator);
            readout.Add($"* {("Source: "+myUnit.SourceFile), -(READOUT_WIDTH - 4)} *" );
            readout.Add(boundary);

            return readout;
        }

        public static List<string> generateGameEngineReadout(GameEngine ge)
        {
            List<string> readout = new List<string>();
            var allUnits = new List<GameUnit>();

            readout.Add(" BEGIN READOUT ".PadLeft((READOUT_WIDTH + " BEGIN READOUT ".Length) / 2, '^').PadRight(READOUT_WIDTH, '^'));
            readout.Add("");
            //Game Engine header block
            readout.Add("* Game Info *".PadRight(READOUT_WIDTH, '*'));
            readout.Add("* " + $"Game [{ge.id}] Exchange {ge.exchange} / Volley {ge.volley} -- Combat:{ge.combat}".PadRight(READOUT_WIDTH-4) + " *");
            readout.Add(boundary);
            readout.Add("");

            //Briefing block
            readout.Add("* Briefing *".PadRight(READOUT_WIDTH, '*'));
            readout.AddRange(WrapDecorated(ge.Briefing, READOUT_WIDTH, "* ", " *", HANGING_INDENT_WIDTH));
            readout.Add(boundary);
            readout.Add("");

            //Report block
            readout.Add("* Report *".PadRight(READOUT_WIDTH, '*'));
            readout.AddRange(WrapDecorated(String.IsNullOrEmpty(ge.Report) ? "..." : ge.Report, READOUT_WIDTH, "* ", " *", HANGING_INDENT_WIDTH));
            readout.Add(boundary);
            readout.Add("");

            //Player block
            readout.Add("* Players *".PadRight(READOUT_WIDTH, '*'));
            foreach (var p in ge.Players)
            {
                var pNameString = p.key <= 0 ? p.name : ($"{p.name} [{p.key}]");
                var fixedString = $"* {p.id} -- {p.team.PadRight(16)} -- {pNameString.PadRight(20)} -- {p.email.PadRight(20).Substring(0,20)} -- ";
                var objLength = READOUT_WIDTH - fixedString.Length - 2;
                var playerLine = fixedString + p.Objectives.PadRight(objLength).Substring(0, objLength) + " *";
                readout.Add(playerLine);
            }
            readout.Add(boundary);
            readout.Add("");

            //Units summary block
            foreach (var p in ge.Players)
            {
                readout.Add($"* Player [{p.id}]{p.name} Units ({p.Units.Count}):  *".PadRight(READOUT_WIDTH, '*'));
                foreach (var u in p.Units)
                {
                    readout.AddRange(WrapDecorated(u.ToString(), READOUT_WIDTH, "*   -", " *"));
                    readout.AddRange(WrapDecorated($"Status - {u.Status}", READOUT_WIDTH, "*      -", " *"));
                    allUnits.Add(u);
                }
                readout.Add(boundary);
                readout.Add("");
            }

            //Unit detail blocks
            foreach (var p in ge.Players)
            {
                foreach (var u in p.Units)
                {
                    readout.Add($"* Player [{p.id}]{p.name} Unit Detail: *".PadRight(READOUT_WIDTH, '*'));
                    readout.Add($"* <{u.ToString(), -(READOUT_WIDTH-6)}> *");
                    var unitReadout = generateUnitReadout(u, allUnits);
                    readout.AddRange(unitReadout);
                    readout.Add("");
                }
            }

            readout.Add(" END READOUT ".PadLeft((READOUT_WIDTH + " END READOUT ".Length) / 2, '^').PadRight(READOUT_WIDTH, '^'));

            return readout;
        }

        private static List<string> printReadoutCollection<T>(string collectionName, List<T> coll, string outputFormat, bool suppressTitleLine = false)
        {
            List<string> outputLines = new List<string>();
            switch (coll.Count)
            {
                case 0: { /* print nothing */ break; }
                //case 1: { outputLines.Add(String.Format(outputFormat, collectionName, coll[0])); break; }
                default:
                    {
                        //multiple entries needs a multi-line printout
                        if (!suppressTitleLine) outputLines.AddRange(WrapDecorated($"{collectionName, -16}({coll.Count, 2})", READOUT_WIDTH, "* ", " *"));
                        foreach (var sys in coll)
                        {
                            outputLines.AddRange(WrapDecorated(sys.ToString(), READOUT_WIDTH, "*                      > ", " *", HANGING_INDENT_WIDTH));
                        }
                        break;
                    }
            }

            return outputLines;
        }

        private static List<string> printOrders<T>(string collectionName, List<T> coll, string outputFormat, GameUnit thisUnit = null, List<GameUnit> allUnits = null)
        {
            var output = new List<string>();

            if (coll.Count > 0)
                {
                    output.Add($"* {$"{collectionName,-16}({coll.Count,2})", -(READOUT_WIDTH - 4)} *");
                }


            foreach (var orderT in coll)
            {
                var o = (UnitOrders)(object)orderT;
                string readoutLine = o.ToString();
                readoutLine = readoutLine.Replace("Target:[PD]", "PD");
                if (allUnits != null)
                {
                    var orderTarget = allUnits.Where(u => u.IdNumeric.ToString() == o.TargetID).FirstOrDefault();

                    if (orderTarget != null)
                    {
                        readoutLine = readoutLine.Replace($"Target:[{o.TargetID}]", orderTarget.InstanceName);
                    }
                }

                output.Add(String.Format(outputFormat, "", readoutLine));
                if (thisUnit != null && orderT.GetType() == typeof(FireOrder))
                {
                    foreach (var weaponID in ((FireOrder)o).WeaponIDs.Split(','))
                    {
                        //Fire orders have additional lines of output indicating assigned weapons by ID
                        var weapon = thisUnit.Weapons.Where(w => $"{w.id}" == $"{weaponID}").FirstOrDefault();
                        var weaponText = weapon == null ? $"Weapon [{weaponID:00}]" : weapon.SystemName;
                        var weaponEntry = $"{"",21} - {weaponText,-30} [{weaponID,2}]";
                        output.Add(String.Format(outputFormat, "", weaponEntry));
                    }
                    output.Add(String.Format(outputFormat, "", ""));
                }
            }

            return output;
        }

        public static List<string> WrapDecorated(string text, int margin, string prefix, string suffix, int hangingIndentLength=0)
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
                    end = start + margin;

                lines.Add(prefix + "".PadRight(indent) + text.Substring(start, end - start).PadRight(margin - indent) + suffix);
                start = end + 1;

                indent = hangingIndentLength;
            }

            if (start < text.Length)
                lines.Add(prefix + "".PadRight(indent) + text.Substring(start).PadRight(margin - indent) + suffix);

            return lines;
        }
    }
}
