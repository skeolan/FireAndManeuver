using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("Ship")]
    public class Unit
    {
        //Instance properties
        [XmlAttribute("id")] public string id { get; set; } = "001";
        [XmlElement("Name")] public string name { get; set; } = "";
        [XmlElement("Status")] public string status { get; set; } = "Ok";
        //[XmlElement("Orders")] public string orders { get; set; }
        //[XmlElement("DamageControl")] public string damageControl { get; set; }
        //[XmlElement("Position")] public string position { get; set; }
        //[XmlElement("Heading")] public string heading { get; set; }
        //[XmlElement("Speed")] public string speed { get; set; }
        //[XmlElement("VectorSpeed")] public string vectorSpeed { get; set; }
        //[XmlElement("Course")] public string course { get; set; }
        [XmlElement("CrewQuality")] public string crewQuality { get; set; } = "Average";


        // Class+instance properties
        [XmlElement("Race")] public string race { get; set; } = "Generic";
        [XmlElement("ClassAbbrev")] public string classAbbrev { get; set; } = "XX";
        [XmlElement("ClassName")] public string className { get; set; } = "Unit";
        [XmlElement("ShipClass")] public string shipClass { get; set; } = "Unnamed";
        [XmlElement("Mass")] public int mass { get; set; } = 0;
        [XmlElement("PointValue")] public int pointValue { get; set; } = 0;
        [XmlElement("MainDrive")] public DriveSystem mainDrive { get; set; } = new DriveSystem();
        [XmlElement("FTLDrive")] public FTLDriveSystem ftlDrive { get; set; } = null;
        [XmlElement("Armor")] public ArmorSystem armor { get; set; } = null;
        [XmlElement("Hull")] public HullSystem hull { get; set; } = null;
        [XmlIgnore] public string sourceFile { get; set; } = null;

        /* Collections of systems */
        [XmlArray("Electronics")]
        [XmlArrayItem("FireControl", Type = typeof(FireControl))]
        public List<ElectronicsSystem> electronics { get; set; }

        [XmlArray("Defenses")]
        [XmlArrayItemAttribute("Screen", Type = typeof(ScreenSystem))]
        public List<DefenseSystem> defenses { get; set; }

        [XmlArray("Holds")]
        [XmlArrayItemAttribute("CargoHold", Type = typeof(CargoHoldSystem))]
        public List<UnitSystem> holds { get; set; }

        [XmlArray("Weapons")]
        [XmlArrayItem("PointDefense", Type = typeof(PointDefenseSystem))]
        [XmlArrayItem("BeamBattery", Type = typeof(BeamBatterySystem))]
        [XmlArrayItem("AntiMatterTorpedoLauncher", Type = typeof(AntiMatterTorpedoLauncherSystem))]
        public List<WeaponSystem> weapons { get; set; }

        public Unit()
        {

        }

        public string InstanceName
        {

            get
            {
                return string.IsNullOrWhiteSpace(name + id) ? "" : string.Format("{0} {1}-{2}", name, classAbbrev, id);
            }
        }

        public override string ToString()
        {
            //e.g. "Aragorn CA-001 -- Terran Ranger-class Heavy Cruiser -- TMF:45 / NPV:500"
            return string.Format("{0} -- {1} {2}-class {3} -- TMF:{4} / NPV:{5}", InstanceName, race, className, shipClass, mass, pointValue);
        }

        public static Unit loadNewUnit(string sourceFile)
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

}
