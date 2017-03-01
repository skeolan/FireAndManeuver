using System.Collections.Generic;
using System.Xml.Serialization;

namespace FireAndManeuver.Units
{
    [XmlRoot("Ship")]
    public class Unit
    {
        [XmlElement("id")]                  public string id { get; set; }
        [XmlElement("Race")]                public string race { get; set; }
        [XmlElement("ClassAbbrev")]         public string classAbbrev { get; set; }
        [XmlElement("ClassName")]           public string className { get; set; }
        [XmlElement("ShipClass")]           public string shipClass { get; set; }
        [XmlElement("Mass")]                public int mass { get; set; }
        [XmlElement("PointValue")]          public int pointValue { get; set; }
        [XmlElement("MainDrive")]           public DriveSystem mainDrive { get; set; }
        [XmlElement("FTLDrive")]            public FTLDriveSystem ftlDriveSystem { get; set; }
        [XmlIgnore]                         public bool ftlDrive { get { return this.ftlDriveSystem != null; } }
        [XmlElement("Armor")]               public ArmorSystem armor { get; set; }
        [XmlElement("Hull")]                public HullSystem hull { get; set; }

        /* Collections of systems */
        /*
        [XmlElement("Electronics")] public List<ElectronicsSystem> electronics { get; set; }
        [XmlElement("Defenses")] public List<DefenseSystem> defenses { get; set; }
        [XmlElement("Holds")] public List<HoldSystem> holds { get; set; }
        */
        [XmlArray("Weapons")] 
        [XmlArrayItem("PointDefense", Type=typeof(PointDefenseSystem))]
        [XmlArrayItem("BeamBattery", Type=typeof(BeamBatterySystem))]
        [XmlArrayItem("AntiMatterTorpedoLauncher", Type=typeof(AntiMatterTorpedoLauncherSystem))]
        public List<WeaponSystem> weapons { get; set; }

        public Unit()
        {

        }
    }

    public class unitSystem
    {
        [XmlAttribute("xSSD")]
        public int xSSD { get; set; }
        [XmlAttribute("ySSD")]
        public int ySSD { get; set; }
    }

    [XmlRoot("MainDrive")]
    public class DriveSystem : unitSystem
    {
        [XmlAttribute] public string type { get; set; }
        [XmlAttribute] public int initialThrust { get; set; }
        public DriveSystem()
        {
            type="";
            initialThrust=0;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", this.initialThrust, this.type);// == "Standard" ? "" : " ("+this.type+")");
        }
    }

    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : unitSystem
    {
        public FTLDriveSystem(){}
    }

    [XmlRoot("Armor")]
    public class ArmorSystem : unitSystem
    {
        public ArmorSystem() {}

        [XmlAttribute] public int totalArmor { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", this.totalArmor);
        }
    }

    public class HullSystem : unitSystem
    {
        [XmlAttribute] public int value { get; set; }
        [XmlAttribute] public string type { get; set; }
        [XmlAttribute] public int totalHullBoxes { get; set; }
        [XmlAttribute] public int rows { get; set; }

        public HullSystem()
        {

        }

        public override string ToString()
        {
            return string.Format("{0} ({1} rows)", totalHullBoxes, rows);
        }

    }

    public class ElectronicsSystem : unitSystem
    {

    }

    [XmlRoot("FireControl")]
    public class FireControl : ElectronicsSystem
    {

    }

    public class DefenseSystem : unitSystem
    {

    }

    public class HoldSystem : unitSystem
    {

    }

    public abstract class WeaponSystem : unitSystem
    {
        protected string weaponName;
        public WeaponSystem() {
            weaponName =  "BaseWeaponSystem Class";
        }
        
        public override string ToString()
        {
            return string.Format("{0} ({1},{2})", weaponName, xSSD, ySSD);
        }
    }

    public class PointDefenseSystem : WeaponSystem
    {
        public PointDefenseSystem()
        {
            this.weaponName = "Point Defense System";
        }
    }

    public class BeamBatterySystem : WeaponSystem
    {
        public BeamBatterySystem()
        {
            this.weaponName = "Beam Battery";
        }
    }

    public class AntiMatterTorpedoLauncherSystem : WeaponSystem
    {
        public AntiMatterTorpedoLauncherSystem()
        {
            this.weaponName = "Antimatter Torpedo Launcher";
        }
    }
}
