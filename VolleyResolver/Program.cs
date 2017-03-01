using System;
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

            var myUnit = loadNewUnit("..\\Example-ShipData\\UNSC_DD_Lake.xml");

            var outputFormat = "{0, -20} : {1}";

            Console.WriteLine(outputFormat, "Race", myUnit.race);
            Console.WriteLine(outputFormat, "ClassAbbrev", myUnit.classAbbrev);
            Console.WriteLine(outputFormat, "ClassName", myUnit.className);
            Console.WriteLine(outputFormat, "ShipClass", myUnit.shipClass);
            Console.WriteLine(outputFormat, "Mass", myUnit.mass);
            Console.WriteLine(outputFormat, "PointValue", myUnit.pointValue);
            Console.WriteLine(outputFormat, "MainDrive", myUnit.mainDrive.ToString());
            Console.WriteLine(outputFormat, "FTLDrive", myUnit.ftlDrive);
            Console.WriteLine(outputFormat, "Armor", myUnit.armor.ToString());
            Console.WriteLine(outputFormat, "Hull", myUnit.hull.ToString());
            Console.WriteLine("");
            Console.WriteLine(outputFormat, "Weapons ("+myUnit.weapons.Count.ToString()+")", "");
            foreach (var ws in myUnit.weapons)
            {
                Console.WriteLine(outputFormat, "", ws.ToString() );
            }
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
