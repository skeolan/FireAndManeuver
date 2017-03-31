using System;
using System.Linq;
using System.Collections.Generic;
using FireAndManeuver.GameEngine;

namespace VolleyResolver
{
    public static class UnitConsoleReadoutUtilities
    {

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

        private static List<string> printReadoutCollection<T>(string collectionName, List<T> coll, string outputFormat)
        {
            List<string> outputLines = new List<string>();
            switch (coll.Count)
            {
                case 0: { /* print nothing */ break; }
                //case 1: { outputLines.Add(String.Format(outputFormat, collectionName, coll[0])); break; }
                default:
                    {
                        //multiple entries needs a multi-line printout
                        outputLines.Add(String.Format("* {0,-16}({1, 2}){2,-76} *", collectionName, coll.Count.ToString(), ""));
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
    }
}
