using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FireAndManeuver.Units;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var myUnit = loadNewUnit(args.Length > 0 ? args[0] : "..\\Example-ShipData\\UNSC_DD_Lake.xml");

            foreach(var readoutLine in generateUnitReadout(myUnit))
            {
                Console.WriteLine(readoutLine);
            }
        }

        private static List<string> generateUnitReadout(Unit myUnit)
        {
            var outputFormat = "{0, -20} : {1}";

            List<string> readout = new List<string>();

            readout.Add("Hull Types:");
            foreach (var ht in Enum.GetNames( typeof(HullTypeLookup) ) )
            {
                readout.Add( String.Format("     {0,-10} - {1}", ht, (int)Enum.Parse(typeof(HullTypeLookup), ht) ) );
            }

            readout.Add("");
            readout.Add("Unit:");
            readout.Add("-----");
            readout.Add(String.Format(outputFormat, "Race", myUnit.race) );
            readout.Add(String.Format(outputFormat, "ClassAbbrev", myUnit.classAbbrev) );
            readout.Add(String.Format(outputFormat, "ClassName", myUnit.className) );
            readout.Add(String.Format(outputFormat, "ShipClass", myUnit.shipClass) );
            readout.Add(String.Format(outputFormat, "Mass", myUnit.mass) );
            readout.Add(String.Format(outputFormat, "PointValue", myUnit.pointValue) );
            readout.Add(String.Format(outputFormat, "MainDrive", myUnit.mainDrive.ToString()) );
            readout.Add(String.Format(outputFormat, "FTLDrive", myUnit.ftlDrive) );
            readout.Add(String.Format(outputFormat, "Armor", myUnit.armor.ToString()) );
            readout.Add(String.Format(outputFormat, "Hull", myUnit.hull.ToString()) );
            readout.Add("");
            readout.AddRange(printSystemCollection("Electronics", myUnit.electronics, outputFormat) );
            readout.AddRange(printSystemCollection("Defenses", myUnit.defenses, outputFormat) );
            readout.AddRange(printSystemCollection("Holds", myUnit.holds, outputFormat) );
            readout.AddRange(printSystemCollection("Weapons", myUnit.weapons, outputFormat) );

            return readout;
        }

        private static List<string> printSystemCollection<T>(string collectionName, List<T> coll, string outputFormat)
        {
            List<string> outputLines=new List<string>();
            switch(coll.Count) {
                case 0: { /* print nothing */ break; }
                case 1: { outputLines.Add(String.Format(outputFormat, collectionName, coll[0]) );  break; }
                default: { 
                    //multiple entries needs a multi-line printout
                    outputLines.Add(String.Format("{0,-12}({1, 2}) -----", collectionName, coll.Count.ToString()) );
                    foreach (var sys in coll)
                    {
                        outputLines.Add(String.Format(outputFormat, "", sys.ToString() ) );
                    }
                    outputLines.Add("");
                    break;
                 }
            }

            return outputLines;
        }

        private static Unit loadNewUnit(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(Unit));

            FileStream fs;
            
            try {
                fs = new FileStream(sourceFile, FileMode.Open);
                Console.WriteLine("Loaded XML {0} successfully", sourceFile);
            } 
            catch(FileNotFoundException ex) {
                throw ex;
            }

            var myNewUnit = (Unit) srz.Deserialize(fs);

            return myNewUnit;
        }
    }
}
