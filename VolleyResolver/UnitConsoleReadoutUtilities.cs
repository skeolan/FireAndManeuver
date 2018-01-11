using System;
using System.Linq;
using System.Collections.Generic;
using FireAndManeuver.GameEngine;
using System.Text.RegularExpressions;

namespace VolleyResolver
{
    public static class UnitConsoleReadoutUtilities
    {

        const bool SUPPRESS_TITLE_LINE_FOR_HULLMATRIX = true;

        public static List<string> generateUnitReadout(Unit myUnit, List<Unit> allUnits = null)
        {
            var outputFormat = "* {0, -20} : {1, -73} *";
            var collectionItemOutputFormat = "* {0, -20} > {1, -73} *";
            var separator = $"* {string.Concat(Enumerable.Repeat("-", 96))} *";
            var boundary = string.Concat(Enumerable.Repeat("*", 100));

            List<string> readout = new List<string>();

            readout.Add("");
            readout.Add(boundary);
            readout.Add(string.Format("* {0, -96} *", myUnit.ToString()));
            readout.Add(separator);
            readout.Add(String.Format(outputFormat, "Status", myUnit.status));
            readout.Add(String.Format(outputFormat, "Armor", myUnit.armor.ToString()));
            readout.Add(String.Format(outputFormat, "Hull", myUnit.hull.ToString()));
            readout.AddRange(printReadoutCollection("Hull", myUnit.hull.HullDisplay(), collectionItemOutputFormat, SUPPRESS_TITLE_LINE_FOR_HULLMATRIX));
            readout.Add(String.Format(outputFormat, "Crew", myUnit.crewQuality));
            readout.Add(separator);
            readout.Add(String.Format(outputFormat, "MainDrive", myUnit.mainDrive.ToString()));
            readout.Add(String.Format(outputFormat, "FTLDrive", myUnit.ftlDrive.ToString()));
            readout.Add(separator);
            readout.AddRange(printReadoutCollection("Electronics", myUnit.electronics, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Defenses", myUnit.defenses, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Holds", myUnit.holds, collectionItemOutputFormat));
            readout.AddRange(printReadoutCollection("Weapons", myUnit.weapons, collectionItemOutputFormat));
            readout.Add(separator);
            readout.Add(String.Format("* {0, -96} *", myUnit.Orders.Count > 0 ? "Orders" : "(No Orders)"));
            foreach (var volleyOrders in myUnit.Orders)
            {
                readout.Add(String.Format("* {0, -96} *", $"  Volley #{volleyOrders.volley}"));
                readout.Add(String.Format("* {0, -96} *", "     " + volleyOrders.ToString().Split('|')[0]));
                readout.AddRange(printOrders("     Maneuvering", volleyOrders.ManeuveringOrders, collectionItemOutputFormat, myUnit, allUnits));
                readout.AddRange(printOrders("     Firing", volleyOrders.FiringOrders, collectionItemOutputFormat, myUnit, allUnits));
            }
            readout.Add(separator);
            readout.Add(String.Format("* {0, -96} *", myUnit.sourceFile));
            readout.Add(boundary);

            return readout;
        }

        internal static List<string> generateGameEngineReadout(GameEngine ge)
        {
            List<string> readout = new List<string>();
            var allUnits = new List<Unit>();

            const int READOUT_WIDTH = 100;

            readout.Add(" BEGIN READOUT ".PadLeft((READOUT_WIDTH + " BEGIN READOUT ".Length) / 2, '^').PadRight(READOUT_WIDTH, '^'));
            readout.Add("");

            //Briefing block
            readout.Add("* Briefing *".PadRight(READOUT_WIDTH, '*'));
            foreach (string BriefingLine in WrapDecorated(ge.Briefing, READOUT_WIDTH, "* ", " *"))
            {
                readout.Add(BriefingLine);
            }
            readout.Add("".PadRight(READOUT_WIDTH, '*'));
            readout.Add("");

            //Player block
            readout.Add("* Players *".PadRight(READOUT_WIDTH, '*'));
            foreach (var p in ge.Players)
            {
                var pNameString = p.key <= 0 ? p.name : ($"{p.name} [{p.key}]");
                readout.Add($"* {p.id} -- {p.team.PadRight(16)} -- {pNameString.PadRight(20)} -- {p.email.PadRight(20).Substring(0,20)} -- {p.Objectives.PadRight(23).Substring(0, 23)} *");
            }
            readout.Add("".PadRight(READOUT_WIDTH, '*'));
            readout.Add("");

            //Units summary block
            foreach (var p in ge.Players)
            {
                readout.Add($"* Player [{p.id}]{p.name} Units ({p.Units.Length}):  *".PadRight(READOUT_WIDTH, '*'));
                foreach (var u in p.Units)
                {
                    readout.AddRange(WrapDecorated(u.ToString(), READOUT_WIDTH, "*   -", " *"));
                    readout.AddRange(WrapDecorated($"Status - {u.status}", READOUT_WIDTH, "*      -", " *"));
                    allUnits.Add(u);
                }
                readout.Add("".PadRight(READOUT_WIDTH, '*'));
                readout.Add("");
            }

            //Unit detail blocks
            foreach (var p in ge.Players)
            {
                foreach (var u in p.Units)
                {
                    readout.Add($"* Player [{p.id}]{p.name} Unit Detail:  *".PadRight(READOUT_WIDTH, '*'));
                    var unitReadout = generateUnitReadout(u, allUnits);
                    readout.AddRange(unitReadout);
                    readout.Add("".PadRight(READOUT_WIDTH, '*'));
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
                        if (!suppressTitleLine) outputLines.Add(String.Format("* {0,-16}({1, 2}){2,-76} *", collectionName, coll.Count.ToString(), ""));
                        foreach (var sys in coll)
                        {
                            outputLines.Add(String.Format(outputFormat, "", sys.ToString()));
                        }
                        break;
                    }
            }

            return outputLines;
        }

        private static List<string> printOrders<T>(string collectionName, List<T> coll, string outputFormat, Unit thisUnit = null, List<Unit> allUnits = null)
        {
            var output = new List<string>();

            if (coll.Count > 0)
                output.Add(String.Format("* {0,-16}({1, 2}){2,-76} *", collectionName, coll.Count.ToString(), ""));

            foreach (var orderT in coll)
            {
                var o = (UnitOrders)(object)orderT;
                string readoutLine = o.ToString();
                readoutLine = readoutLine.Replace("Target:[PD]", "PD");
                if (allUnits != null)
                {
                    var orderTarget = allUnits.Where(u => u.idNumeric.ToString() == o.TargetID).FirstOrDefault();

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
                        var weapon = thisUnit.weapons.Where(w => $"{w.id}" == $"{weaponID}").FirstOrDefault();
                        var weaponText = weapon == null ? $"Weapon [{weaponID:00}]" : weapon.systemName;
                        var weaponEntry = $"{"",21} - {weaponText,-30} [{weaponID,2}]";
                        output.Add(String.Format(outputFormat, "", weaponEntry));
                    }
                    output.Add(String.Format(outputFormat, "", ""));
                }
            }

            return output;
        }

        public static List<string> WrapDecorated(string text, int margin, string prefix, string suffix)
        {
            int start = 0, end;
            var lines = new List<string>();
            text = Regex.Replace(text, @"\s", " ").Trim();

            margin = margin - prefix.Length - suffix.Length;

            while ((end = start + margin) < text.Length)
            {
                while (text[end] != ' ' && end > start)
                    end -= 1;

                if (end == start)
                    end = start + margin;

                lines.Add(prefix + text.Substring(start, end - start).PadRight(margin) + suffix);
                start = end + 1;
            }

            if (start < text.Length)
                lines.Add(prefix + text.Substring(start).PadRight(margin) + suffix);

            return lines;
        }
    }
}
