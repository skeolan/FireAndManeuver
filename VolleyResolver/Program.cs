using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
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
                .AddJsonFile("shipDataSources-small.json")
                .Build();

            var srcFilesFromConfig = (config["srcFiles"]       == null ? new string[0] : config["srcFiles"]      .Split(',') );
            var srcDirsFromConfig  = (config["srcDirectories"] == null ? new string[0] : config["srcDirectories"].Split(',') );
            var unitXMLFiles = getXMLFileList(args, srcFilesFromConfig, srcDirsFromConfig);
            List<Unit> unitSet = loadUnitsFromXMLFiles(unitXMLFiles);

            //... and list their stats    
            //unitSet.ForEach(myUnit => generateUnitReadout(myUnit).ForEach(l => Console.WriteLine(l) ) );
            foreach (var u in unitSet)
            {
                Console.WriteLine("\n{0} : \"{1}\"", u.className, u.sourceFile);
                generateUnitReadout(u).ForEach(l => Console.WriteLine(l));
            }

            Console.WriteLine("{0} Unit(s) loaded and displayed successfully.", unitSet.Count);
        }

        private static List<Unit> loadUnitsFromXMLFiles(HashSet<FileInfo> unitXMLFiles)
        {
            var unitSet = new List<Unit>();

            //... load them all up
            foreach (var x in unitXMLFiles)
            {
                var newU = loadNewUnit(x);
                if (newU != null)
                {
                    unitSet.Add(newU);
                    string unitInfoShortForm = newU.ToString();
                    Debug.WriteLine("\n{0}\n     -- added from {1,30}", unitInfoShortForm, x.FullName);
                }
            }

            return unitSet;
        }

        private static HashSet<FileInfo> getXMLFileList(string[] args, string[] fileSet, string[] dirSet)
        {

            var fileComparer = new FileInfoFullNameComparer();
            var unitXMLFiles = new HashSet<FileInfo>(fileComparer);

            Console.WriteLine("{0} args, {1} files, {2} dirs", args.Length, fileSet.Length, dirSet.Length);
            if (args.Length + fileSet.Length + dirSet.Length == 0) 
            {
                var defaultXML = "..\\Example-ShipData\\UNSC_DD_Lake.xml";
                Console.WriteLine("Defaulting to {0}", defaultXML);
                args = new string[] { defaultXML };
            }

            //Add ship files from commandline arguments and config
            List<string> fileArgSet = new List<string>(args);
            fileArgSet.AddRange(fileSet);
            foreach (var x in fileArgSet)
            {
                Console.WriteLine("xml: [\"{0}\"]", x);
                if (!string.IsNullOrEmpty(x))
                {
                    addXMLToSet(unitXMLFiles, new FileInfo(x));
                }
            }

            //and finally all files under *directories* specified in config.
            foreach (var d in dirSet)
            {
                if (!string.IsNullOrEmpty(d))
                {

                    DirectoryInfo dir = new DirectoryInfo(d);
                    Console.WriteLine("dir: [\"{0}\\*\"]", dir.FullName);

                    //specified dir...
                    foreach (var x in dir.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly))
                    {
                        Debug.WriteLine(x.FullName);
                        addXMLToSet(unitXMLFiles, x);
                    }

                    //... and all subdirectories
                    foreach (var subD in dir.EnumerateDirectories("*.*", SearchOption.AllDirectories))
                    {

                        Debug.WriteLine("SubDir \"{0}\"", subD.FullName);
                        foreach (var x in subD.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly))
                        {
                            Debug.WriteLine(x.FullName);
                            addXMLToSet(unitXMLFiles, x);
                        }
                    }
                }
            }
            return unitXMLFiles;
        }

        private static bool addXMLToSet(HashSet<FileInfo> unitXMLFiles, FileInfo a)
        {
            bool success = unitXMLFiles.Add(a);
            if (success)
            {
                Debug.WriteLine("{0}", a.Name);
            }
            else
            {
                Debug.WriteLine("!!!! Duplicate - {0}", a.FullName);
            }

            return success;
        }

        private static List<string> generateUnitReadout(Unit myUnit)
        {
            var outputFormat = "{0, -20} : {1}";

            List<string> readout = new List<string>();

            readout.Add(myUnit.ToString());
            readout.Add("--------------------------------------------------------------");
            readout.Add(String.Format(outputFormat, "MainDrive", myUnit.mainDrive.ToString()));
            readout.Add(String.Format(outputFormat, "FTLDrive", myUnit.ftlDrive));
            readout.Add(String.Format(outputFormat, "Armor", myUnit.armor.ToString()));
            readout.Add(String.Format(outputFormat, "Hull", myUnit.hull.ToString()));
            readout.Add("");
            readout.AddRange(printSystemCollection("Electronics", myUnit.electronics, outputFormat));
            readout.AddRange(printSystemCollection("Defenses", myUnit.defenses, outputFormat));
            readout.AddRange(printSystemCollection("Holds", myUnit.holds, outputFormat));
            readout.AddRange(printSystemCollection("Weapons", myUnit.weapons, outputFormat));
            readout.Add("--------------------------------------------------------------");

            return readout;
        }

        private static List<string> printSystemCollection<T>(string collectionName, List<T> coll, string outputFormat)
        {
            List<string> outputLines = new List<string>();
            switch (coll.Count)
            {
                case 0: { /* print nothing */ break; }
                case 1: { outputLines.Add(String.Format(outputFormat, collectionName, coll[0])); break; }
                default:
                    {
                        //multiple entries needs a multi-line printout
                        outputLines.Add(String.Format("{0,-12}({1, 2}) -----", collectionName, coll.Count.ToString()));
                        foreach (var sys in coll)
                        {
                            outputLines.Add(String.Format(outputFormat, "", sys.ToString()));
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
            catch (FileNotFoundException ex)
            {
                throw ex;
            }

            try
            {
                myNewUnit = (Unit)srz.Deserialize(fs);
                myNewUnit.sourceFile = sourceFile;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? "");
                //throw ex;
            }

            return myNewUnit;
        }
    }

    public class FileInfoFullNameComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo f1, FileInfo f2)
        {
            if (f1 == null || f2 == null)
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
