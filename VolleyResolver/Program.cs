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

            Console.WriteLine("{0, -20} : {1}", "Race", myUnit.race);
            Console.WriteLine("{0, -20} : {1}", "ClassAbbrev", myUnit.classAbbrev);
            Console.WriteLine("{0, -20} : {1}", "ClassName", myUnit.className);
            Console.WriteLine("{0, -20} : {1}", "ShipClass", myUnit.shipClass);
            Console.WriteLine("{0, -20} : {1}", "Mass", myUnit.mass);
            Console.WriteLine("{0, -20} : {1}", "PointValue", myUnit.pointValue);
            Console.WriteLine("{0, -20} : {1}", "MainDrive", myUnit.mainDrive.ToString());
            Console.WriteLine("{0, -20} : {1}", "FTLDrive", myUnit.ftlDrive);
            Console.WriteLine("{0, -20} : {1}", "Armor", myUnit.armor.ToString());
            Console.WriteLine("{0, -20} : {1}", "Hull", myUnit.hull.ToString());
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
