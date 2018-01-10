using System;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FireAndManeuver.GameEngine;

namespace VolleyResolver
{
    public class Program
    {
        private const string _DefaultXML = ".\\DefaultUnit.xml";

        public static void Main(string[] args)
        {

            
            if (args.Any(a => a.ToLowerInvariant().StartsWith("-help") || a.ToLowerInvariant().StartsWith("-?")))
            {
                //Print help
                Console.WriteLine("Call program with one or more paths to json configs and/or xml unit designs.");
                Console.WriteLine("{0} <\\path\\to\\foo.json> <\\path\\to\\bar.json> <\\path\\to\\design.xml> <\\path\\to\\otherDesign.xml>", "[ProgramName]");
                //Exit early
                return;
            }

            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            foreach (var jsonConfig in args.Where(a => a.EndsWith(".json")))
            {
                configBuilder.AddJsonFile(jsonConfig);
            }
            var config = configBuilder.Build();

            var srcFilesFromArgs = args.Where(x => x.EndsWith(".xml")).ToArray();
            var srcFilesFromConfig = (config["srcFiles"] == null ? new string[0] : config["srcFiles"].Split(','));
            var srcDirsFromConfig = (config["srcDirectories"] == null ? new string[0] : config["srcDirectories"].Split(','));
            var unitXMLFiles = getXMLFileList(srcFilesFromArgs, srcFilesFromConfig, srcDirsFromConfig);
            List<Unit> unitSet = LoadDesignXML(unitXMLFiles);

            //... and list their stats    
            //unitSet.ForEach(myUnit => generateUnitReadout(myUnit).ForEach(l => Console.WriteLine(l) ) );
            foreach (var u in unitSet)
            {
                //Console.WriteLine("\n{0} : \"{1}\"", u.className, u.sourceFile);
                //UnitConsoleReadoutUtilities.generateUnitReadout(u, unitSet).ForEach(l => Console.WriteLine(l));
            }

            GameEngine ge = GameEngine.loadFromXml(@"C:\Games\GitHub\FireAndManeuver.git\Example-GameEngineData\Scenario1_Frat_Attack_2player.xml");
            Console.WriteLine("{0} Unit(s) loaded and displayed successfully.", unitSet.Count);

            Console.WriteLine($"GameEngine [{ge.id}] from {ge.SourceFile} loaded successfully.");
            Console.WriteLine("* Briefing *".PadRight(100, '*'));
            foreach (string BriefingLine in UnitConsoleReadoutUtilities.WrapDecorated(ge.Briefing, 100, "* ", " *"))
            {
                Console.WriteLine(BriefingLine);
            }
            Console.WriteLine("".PadRight(100, '*'));
            Console.WriteLine("");
            Console.WriteLine("* Players *".PadRight(100, '*'));
            foreach (GameEnginePlayer p in ge.Players)
            {
                Console.WriteLine($"* {p.id} -- {p.team.PadRight(16)} -- {p.name.PadRight(20)} -- {p.email.PadRight(20)} -- {p.Objectives.PadRight(23).Substring(0,23)} *");
            }
            Console.WriteLine("".PadRight(100, '*'));
            
        }

        private static List<Unit> LoadDesignXML(HashSet<FileInfo> unitXMLFiles)
        {
            var unitSet = new List<Unit>();

            //... load them all up
            foreach (var x in unitXMLFiles)
            {   
                try {
                var newU = Unit.loadNewUnit(x.FullName);
                if (newU != null)
                {
                    unitSet.Add(newU);
                    string unitInfoShortForm = newU.ToString();
                    Debug.WriteLine("\n{0}\n     -- added from {1,30}", unitInfoShortForm, x.FullName);
                }
                }
                catch (NullReferenceException ex) {
                    Debug.WriteLine($"Unable to load {x.FullName} -- {ex.Message}");
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
                Console.WriteLine("Defaulting to {0}", _DefaultXML);
                args = new string[] { _DefaultXML };
            }

            //Add ship files from commandline arguments and config
            List<string> fileArgSet = new List<string>(args);
            fileArgSet.AddRange(fileSet);

            //quietly drop any non-xml files from fileArgSet
            foreach (var x in fileArgSet.Where(x => x.EndsWith("xml")))
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
