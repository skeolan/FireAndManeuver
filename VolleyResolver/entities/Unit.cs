using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("Ship")]
    public class Unit
    {
        //Instance properties
        [XmlAttribute("id")] public int idNumeric { get; set; } = 1;
        public string fullID {
            get {
                var raceToken = race.Split(' ')[0];
                return $"{raceToken}-{classAbbrev}-{idNumeric:000}";
            }
            set {
                var raceToken = race.Split(' ')[0];
                string valueNumeric = value.Replace(raceToken, "").Replace(classAbbrev, "").Replace("-", "");
                int newID = idNumeric;
                if( !int.TryParse(value, out newID) | newID <1 ) {
                    throw new ArgumentException($"New ID assignment {value} must be a positive integer value or a string like {race}-{classAbbrev}-###.");
                }

                idNumeric = newID;
            }
        }
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
        [XmlArrayItem("FireControl", Type = typeof(FireControlSystem))]
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

        [XmlArray("FM.Orders")]
        [XmlArrayItem("FM.VolleyOrders", Type=typeof(VolleyOrders))]
        public List<VolleyOrders> Orders { get; set; } = new List<VolleyOrders>();

        public Unit()
        {

        }

        public string InstanceName
        {

            get
            {
                return string.IsNullOrWhiteSpace(name + fullID) ? "" : string.Format("{0} [{1}]", name, fullID);
            }
        }

        public override string ToString()
        {
            //e.g. "Aragorn CA-001 -- Dunedain Ranger-class Heavy Cruiser -- TMF:45 / NPV:500"
            return $"{InstanceName} -- {race} {className}-class {shipClass} -- TMF:{mass} / NPV:{pointValue}";
        }

        public static Unit loadNewUnit(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(Unit));

            FileStream fs;
            Unit myNewUnit = null;

            try
            {
                fs = new FileStream(sourceFile, FileMode.Open);
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

            //If Unit lacks critical features like id's on its systems, add them
            var allSystems = new List<UnitSystem>() {
                myNewUnit.mainDrive,
                myNewUnit.ftlDrive
            };
            allSystems.AddRange(myNewUnit.electronics);
            allSystems.AddRange(myNewUnit.defenses);
            allSystems.AddRange(myNewUnit.holds);
            allSystems.AddRange(myNewUnit.weapons);

            //SortedSet<int> systemIDsTaken = new SortedSet<int>();
            //allSystems.Where(x => x.id != -1).ToList().ForEach( x => systemIDsTaken.Add(x.id) );
            //allSystems.Where( x => x.id != -1).Max( x => x.id);
            //int nextID = systemIDsTaken.LastOrDefault()+1;

            int nextID = Math.Max(allSystems.Max( x => x.id), 0) +1;
            allSystems.Where(x => x.id == -1).ToList().ForEach( x => x.id = nextID++);
            
            return myNewUnit;
        }

    }

}
