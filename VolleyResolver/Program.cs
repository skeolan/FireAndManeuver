using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FireAndManeuver.Units;

namespace VolleyResolver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("shipDataSources.json")
                .Build();

            var dirSet = config["srcDirectories"].Split(',');
            var fileSet = config["srcFiles"].Split(',');
            var unitSet = new List<Unit>();
            var fileComparer = new FileInfoFullNameComparer();
            var unitXMLFiles = new HashSet<FileInfo>(fileComparer);
            
            if(args.Length+fileSet.Length+dirSet.Length == 0) args = new string[] {"..\\Example-ShipData\\UNSC_DD_Lake.xml"};

            //Add ship files from commandline arguments and config
            List<string> fileArgSet = new List<string>(args);
            fileArgSet.AddRange(fileSet);
            foreach(var x in  fileArgSet)
            {
                var a = new FileInfo(x);
                if( unitXMLFiles.Add(a) )
                {
                    Console.WriteLine("Added - {0}", a.Name);
                }
                else
                {
                    Console.WriteLine("!!!! Duplicate - {0}", a.FullName);
                }
            }
            Console.WriteLine("{0} Unit XML(s) loaded from commandline + config - {1} unique XMLs", args.Length, unitXMLFiles.Count);

            //and finally all files under *directories* specified in config.
            int nonDirXMLs = unitXMLFiles.Count;
            int dirXMLCount = 0;
            foreach (var d in dirSet)
            {
                Console.WriteLine("srcDirectory: [\"{0}\"]", d);
                DirectoryInfo dir = new DirectoryInfo(d);
                foreach (var subD in dir.EnumerateDirectories("*.*", SearchOption.AllDirectories))
                {

                    Console.WriteLine("\"{0}\"", subD.FullName);
                    var dirXMLs = subD.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly);
                    foreach (var x in dirXMLs)
                    {
                        var u = x;
                        if (unitXMLFiles.Add(u))
                        {
                            dirXMLCount++;
                            Console.WriteLine("     Added - {0}", u.Name);
                        }
                        else
                        {
                            Console.WriteLine("     !!!! Duplicate - {0}", u.Name);
                        }
                    }
                }
            }
            Console.WriteLine("{0} Unit XML(s) added from directory config - {1} unique XMLs", dirXMLCount, unitXMLFiles.Count);

            
            //... load them all up
            foreach (var x in unitXMLFiles)
            { 
                var newU = loadNewUnit(x);
                if(newU != null) 
                {
                    unitSet.Add( newU );
                }
            }
            
            //... and list their stats    
            //unitSet.ForEach(myUnit => generateUnitReadout(myUnit).ForEach(l => Console.WriteLine(l) ) );

            Console.WriteLine("{0} Unit(s) loaded and displayed successfully.", unitSet.Count);
        }

        private static List<string> generateUnitReadout(Unit myUnit)
        {
            var outputFormat = "{0, -20} : {1}";

            List<string> readout = new List<string>();

            readout.Add("");
            readout.Add("Unit:");
            readout.Add("--------------------------------------------------------------");
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
            readout.Add("--------------------------------------------------------------");

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
                    break;
                 }
            }

            return outputLines;
        }

        private static Unit loadNewUnit(FileInfo f)
        {
            return loadNewUnit(f.FullName);
        }
        private static Unit loadNewUnit(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(Unit));

            FileStream fs;
            Unit myNewUnit = null;
            
            try 
            {
                fs = new FileStream(sourceFile, FileMode.Open);
                //Console.WriteLine("Loaded XML {0} successfully", sourceFile);
            } 
            catch(FileNotFoundException ex) {
                throw ex;
            }
            


            try 
            {
                myNewUnit = (Unit) srz.Deserialize(fs);
            }
            catch(InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? "");
                //throw ex;
            }

            return myNewUnit;
        }
    }

    public class FileInfoFullNameComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals (FileInfo f1, FileInfo f2)
        {   
            if (f1 == null || f2==null)
            {
                return false;
            }
            
            return f1.FullName.Equals(f2.FullName, StringComparison.CurrentCultureIgnoreCase);
        }
        
        public int GetHashCode(FileInfo fi)
        {
            return fi.FullName.GetHashCode();
        }
    }
}
