using System.Collections.Generic;
using System.Xml.Serialization;

namespace FireAndManeuver.Units
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
        [XmlElement("id")] public string id { get; set; }
        [XmlElement("Race")] public string race { get; set; }
        [XmlElement("ClassAbbrev")] public string classAbbrev { get; set; }
        [XmlElement("ClassName")] public string className { get; set; }
        [XmlElement("ShipClass")] public string shipClass { get; set; }
        [XmlElement("Mass")] public int mass { get; set; }
        [XmlElement("PointValue")] public int pointValue { get; set; }
        [XmlElement("MainDrive")] public DriveSystem mainDrive { get; set; } = new DriveSystem();
        [XmlElement("FTLDrive")] public FTLDriveSystem ftlDriveSystem { get; set; }
        [XmlIgnore] public bool ftlDrive { get { return this.ftlDriveSystem != null; } }
        [XmlElement("Armor")] public ArmorSystem armor { get; set; }
        [XmlElement("Hull")] public HullSystem hull { get; set; }
        [XmlIgnore] public string sourceFile {get; set; }

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

        public override string ToString()
        {
            //return string.Format("{0} {1}-class {2} ({3}) -- TMF:{4} / NPV:{5}", this.race, this.className, this.shipClass, this.classAbbrev, this.mass, this.pointValue);
            //e.g. "Ranger CA (Terran Heavy Cruiser) -- TMF:45 / NPV:500"
            return string.Format("{0} {1} ({2} {3}) -- TMF:{4} / NPV:{5}", className, classAbbrev, race, shipClass, mass, pointValue);
        }

    }

    public abstract class unitSystem
    {
        [XmlAttribute("xSSD")] public int xSSD { get; set; }
        [XmlAttribute("ySSD")] public int ySSD { get; set; }
        [XmlIgnore] public string systemName { get; protected set; }

        public unitSystem()
        {
            systemName = "Universal unitSystem class";
        }

        public override string ToString()
        {
            return string.Format("{0} ({1},{2})", systemName, xSSD, ySSD);
        }
    }

    [XmlRoot("MainDrive")]
    public class DriveSystem : unitSystem
    {
        [XmlAttribute] public string type { get; set; }
        [XmlAttribute] public int initialThrust { get; set; }
        public DriveSystem()
        {
            type = "";
            initialThrust = 0;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.initialThrust, this.type);// == "Standard" ? "" : " ("+this.type+")");
        }
    }

    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : unitSystem
    {
        public FTLDriveSystem() { }
    }

    [XmlRoot("Armor")]
    public class ArmorSystem : unitSystem
    {
        public ArmorSystem() { }

        [XmlAttribute] public string totalArmor { get; set; }

        //public int[] armorLayers { get { return new List<string>(totalArmor.Split(",")).ForEach() } }

        public override string ToString()
        {
            return string.Format("{0}", this.totalArmor);
        }
    }

    public class HullSystem : unitSystem
    {
        [XmlAttribute] public int value { get; set; }
        [XmlAttribute] public HullTypeLookup type { get; set; }
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

        private int _totalHullBoxes=-1;

        [XmlIgnore] private decimal hullTypeMultiplier { get { return (int)type / (decimal)100.0; } }
        [XmlAttribute] public int rows { get; set; }

        public HullSystem()
        {
            rows = 4;
        }

        public override string ToString()
        {
            string typeSuffix = type == HullTypeLookup.Custom ? "Custom" : string.Format("{0} [MUx{1}]", System.Enum.GetName(typeof(HullTypeLookup), type), hullTypeMultiplier);
            return string.Format("{0} ({1} rows) {2}", totalHullBoxes, rows, typeSuffix);
        }

    }

    public abstract class ElectronicsSystem : unitSystem
    {

        public override string ToString()
        {
            return string.Format("{0} ({1},{2})", systemName, xSSD, ySSD);
        }
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
            return string.Format("{0} [Total Size {1}]", type, totalSize);
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
            return string.Format("{0} {1}", systemName, arcs);
        }
    }

    public class BeamBatterySystem : ArcWeaponSystem
    {
        [XmlAttribute] public int rating { get; set; }

        public BeamBatterySystem() : base()
        {
            this.systemName = "Beam Battery";
            this.rating = 1;
        }

        public override string ToString()
        {
            return string.Format("Class-{0} {1} {2,-10} ({3,3},{4,3})", rating, systemName, arcs, xSSD, ySSD);
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
