using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace FireAndManeuver.GameEngine
{
    public enum HullTypeLookup
    {
        Fragile = 10,
        Weak = 20,
        Average = 30,
        Strong = 40,
        Super = 50,
        Custom = 100
    };

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
        public List<unitSystem> holds { get; set; }

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

    public abstract class unitSystem
    {
        [XmlAttribute("id")] public int id { get; set; }
        [XmlAttribute("xSSD")] public int xSSD { get; set; }
        [XmlAttribute("ySSD")] public int ySSD { get; set; }
        [XmlIgnore] public virtual string systemName { get; protected set; }
        [XmlAttribute] public string status { get; set; } = "Operational";

        public unitSystem()
        {
            systemName = "Universal unitSystem class";
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", systemName, status);
        }
    }

    [XmlRoot("MainDrive")]
    public class DriveSystem : unitSystem
    {
        private int? _currentThrust = null;
        [XmlAttribute] public string type { get; set; } = "Standard";
        [XmlAttribute] public int initialThrust { get; set; } = 0;
        
        [XmlAttribute] public int currentThrust { 
            get { return _currentThrust ?? initialThrust; } 
            set { _currentThrust = value; } 
        }
        [XmlAttribute] public bool active { get; set; } = false;
        public DriveSystem()
        {        }

        [XmlIgnore] public override string systemName
        {
            get { return string.Format("{0} Drive", this.type); }
            protected set {}
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}/{2} - {3}", base.ToString(), this.initialThrust, this.currentThrust, this.active ? "Active" : "Inactive");
        }
    }

    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : unitSystem
    {
        [XmlAttribute] public bool active { get; set; } = false;
        public FTLDriveSystem() { systemName = "FTL Drive"; }
        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), this.active ? "Active" : "Inactive");
        }
    }

    [XmlRoot("Armor")]
    public class ArmorSystem : unitSystem
    {
        public ArmorSystem() { }

        [XmlAttribute] public string totalArmor { get; set; }
        [XmlAttribute] public string remainingArmor { get; set; }

        //public int[] armorLayers { get { return new List<string>(totalArmor.Split(",")).ForEach() } }

        public override string ToString()
        {
            string remArmorStr = string.IsNullOrWhiteSpace(remainingArmor) ? "" : String.Format(" ({0} remaining)", remainingArmor) ;
            return string.Format("{0}{1}", this.totalArmor, remArmorStr);
        }
    }

    public class HullSystem : unitSystem
    {
        [XmlAttribute] public int value { get; set; }
        [XmlAttribute] public HullTypeLookup type { get; set; }
        [XmlAttribute("class")] public string hullClass { get; set; }
        [XmlAttribute]
        public int totalHullBoxes
        {
            get
            {
                //if (type == HullTypeLookup.Custom) return _totalHullBoxes;
                if (_totalHullBoxes != -1) return _totalHullBoxes;
                else
                {
                    //Automatically rounds DOWN?
                    return (int)(this.value * this.hullTypeMultiplier);
                }
            }
            set
            {
                this._totalHullBoxes = value;
            }
        }

        private int _totalHullBoxes = -1;
        private int _remainingHullBoxes = -1;

        [XmlIgnore] private decimal hullTypeMultiplier { get { return (int)type / (decimal)100.0; } }
        [XmlAttribute] public int rows { get; set; }
        [XmlAttribute] public int remainingHullBoxes 
        { 
            get
            {
                if(_remainingHullBoxes != -1) return _remainingHullBoxes;
                else return _totalHullBoxes;
            } 
            set
            {
                _remainingHullBoxes = value;
            }
        }

        public HullSystem()
        {
            rows = 4;
        }

        public override string ToString()
        {
            string typeSuffix = type == HullTypeLookup.Custom ? "Custom" : string.Format("{0} [MUx{1}]", System.Enum.GetName(typeof(HullTypeLookup), type), hullTypeMultiplier);
            return string.Format("{0}/{1} ({2} rows) {3} {4}", remainingHullBoxes, totalHullBoxes, rows, typeSuffix, hullClass);
        }

    }

    public abstract class ElectronicsSystem : unitSystem
    {

    }

    [XmlRoot("FireControl")]
    public class FireControl : ElectronicsSystem
    {
        public FireControl()
        {
            systemName = "Fire Control System";
        }
    }

    public class DefenseSystem : unitSystem
    {
        public DefenseSystem()
        {

        }
    }

    public class ScreenSystem : DefenseSystem
    {
        public ScreenSystem()
        {
            systemName = "Screen";
        }
    }

    public class CargoHoldSystem : unitSystem
    {
        [XmlAttribute] public string type;

        [XmlAttribute] public int totalSize;

        public CargoHoldSystem() : base()
        {
            systemName = "Cargo Hold";
        }

        public override string ToString()
        {
            return string.Format("{0} [Total Size {1}]", base.ToString(), totalSize);
        }

    }

    public abstract class WeaponSystem : unitSystem
    {
        public WeaponSystem() : base()
        {
            systemName = "BaseWeaponSystem Class";
        }
    }

    public class PointDefenseSystem : WeaponSystem
    {
        public PointDefenseSystem() : base()
        {
            this.systemName = "Point Defense System";
        }
    }

    public abstract class ArcWeaponSystem : WeaponSystem
    {
        [XmlAttribute] public string arcs { get; set; }
        public ArcWeaponSystem() : base()
        {
            this.systemName = "Arc-Firing Weapon System";
            this.arcs = "(F)";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), arcs);
        }
    }

    public class BeamBatterySystem : ArcWeaponSystem
    {
        [XmlAttribute] public int rating { get; set; }=1;
        [XmlIgnore] public override string systemName
        {
            get { return string.Format("Class-{0} Beam Battery", this.rating); }
            protected set {}
        }

        public BeamBatterySystem() : base()
        {

        }
    }

    public class AntiMatterTorpedoLauncherSystem : WeaponSystem
    {
        public AntiMatterTorpedoLauncherSystem()
        {
            this.systemName = "Antimatter Torpedo Launcher";
        }
    }
}
