// <copyright file="DataLoadUtilities.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FireAndManeuver.GameModel;

    public class DataLoadUtilities
    {
        public static List<GameUnit> LoadDesignXML(HashSet<FileInfo> unitXMLFiles)
        {
            var unitSet = new List<GameUnit>();

            // ... load them all up
            foreach (var x in unitXMLFiles)
            {
                try
                {
                    var newU = GameUnit.LoadNewUnit(x.FullName);
                    if (newU != null)
                    {
                        unitSet.Add(newU);
                        string unitInfoShortForm = newU.ToString();
                        Debug.WriteLine("\n{0}\n     -- added from {1,30}", unitInfoShortForm, x.FullName);
                    }
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine($"Unable to load {x.FullName} -- {ex.Message}");
                }
            }

            return unitSet;
        }

        public static HashSet<FileInfo> GetXMLFileList(string[] args, string[] fileSet, string[] dirSet, string defaultXml)
        {
            var fileComparer = new FileInfoFullNameComparer();
            var unitXMLFiles = new HashSet<FileInfo>(fileComparer);

            Console.WriteLine("{0} args, {1} files, {2} dirs", args.Length, fileSet.Length, dirSet.Length);
            if (args.Length + fileSet.Length + dirSet.Length == 0)
            {
                Console.WriteLine("Defaulting to {0}", defaultXml);
                args = new string[] { defaultXml };
            }

            // Add ship files from commandline arguments and config
            List<string> fileArgSet = new List<string>(args);
            fileArgSet.AddRange(fileSet);

            // quietly drop any non-xml files from fileArgSet
            foreach (var x in fileArgSet.Where(x => x.EndsWith("xml")))
            {
                Console.WriteLine("xml: [\"{0}\"]", x);
                if (!string.IsNullOrEmpty(x))
                {
                    AddXmlToSet(unitXMLFiles, new FileInfo(x));
                }
            }

            // and finally all files under *directories* specified in config.
            foreach (var d in dirSet)
            {
                if (!string.IsNullOrEmpty(d))
                {
                    DirectoryInfo dir = new DirectoryInfo(d);
                    Console.WriteLine("dir: [\"{0}\\*\"]", dir.FullName);

                    // specified dir...
                    foreach (var x in dir.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly))
                    {
                        Debug.WriteLine(x.FullName);
                        AddXmlToSet(unitXMLFiles, x);
                    }

                    // ... and all subdirectories
                    foreach (var subD in dir.EnumerateDirectories("*.*", SearchOption.AllDirectories))
                    {
                        Debug.WriteLine("SubDir \"{0}\"", subD.FullName);
                        foreach (var x in subD.EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly))
                        {
                            Debug.WriteLine(x.FullName);
                            AddXmlToSet(unitXMLFiles, x);
                        }
                    }
                }
            }

            return unitXMLFiles;
        }

        private static bool AddXmlToSet(HashSet<FileInfo> unitXMLFiles, FileInfo a)
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
}
