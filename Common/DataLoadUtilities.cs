﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FireAndManeuver.GameModel;
using System.Linq;

namespace FireAndManeuver.Common
{
    public class DataLoadUtilities
    {
        public static List<Unit> LoadDesignXML(HashSet<FileInfo> unitXMLFiles)
        {
            var unitSet = new List<Unit>();

            //... load them all up
            foreach (var x in unitXMLFiles)
            {
                try
                {
                    var newU = Unit.LoadNewUnit(x.FullName);
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

        public static HashSet<FileInfo> getXMLFileList(string[] args, string[] fileSet, string[] dirSet, string _DefaultXML)
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
