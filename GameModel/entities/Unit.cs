using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("Ship")]
    public class Unit
    {
        private string _damageControlRaw = "";
        private List<UnitSystem> _allSystems;
        
        [XmlAttribute("id")] public int IdNumeric { get; set; } = 1;
        [XmlIgnore] public string FullID {
            get {
                var raceToken = Race.Split(' ')[0];
                return $"{raceToken}-{ClassAbbrev}-{IdNumeric:000}";
            }
            set {
                var raceToken = Race.Split(' ')[0];
                string valueNumeric = value.Replace(raceToken, "").Replace(ClassAbbrev, "").Replace("-", "");
                int newID = IdNumeric;
                if (!int.TryParse(value, out newID) | newID < 1) {
                    throw new ArgumentException($"New ID assignment {value} must be a positive integer value or a string like {Race}-{ClassAbbrev}-###.");
                }

                IdNumeric = newID;
            }
        }
        [XmlElement] public string Name { get; set; } = "";
        [XmlElement] public string Status { get; set; } = "Ok";


        //[XmlElement("Orders")] public string orders { get; set; }
        [XmlElement("DamageControl")] public string DamageControlRaw {
            get
            {
                return _damageControlRaw;
            }
            set
            {
                _damageControlRaw = value;
            } 
        }
        [XmlIgnore] public List<int> DamageControl
        {
            get
            {
                return _damageControlRaw.Split(',').Select(Int32.Parse).ToList();
            }
            set
            {
                _damageControlRaw = String.Join(",", value);
            }
        }

        //[XmlElement] public string Position { get; set; }
        //[XmlElement] public string Heading { get; set; }
        //[XmlElement] public string Speed { get; set; }
        //[XmlElement] public string VectorSpeed { get; set; }
        //[XmlElement] public string Course { get; set; }
        [XmlElement] public string CrewQuality { get; set; } = "Average";


        // Class+instance properties
        [XmlElement] public string Race { get; set; } = "Generic";
        [XmlElement] public string ClassAbbrev { get; set; } = "XX";
        [XmlElement] public string ClassName { get; set; } = "Unit";
        [XmlElement] public string ShipClass { get; set; } = "Unnamed";
        [XmlElement] public int Mass { get; set; } = 0;
        [XmlElement] public int PointValue { get; set; } = 0;
        [XmlElement] public DriveSystem MainDrive { get; set; } = new DriveSystem();
        [XmlElement("FTLDrive")] public FTLDriveSystem FtlDrive { get; set; } = null;
        [XmlElement] public ArmorSystem Armor { get; set; } = null;
        [XmlElement] public HullSystem Hull { get; set; } = null;
        [XmlIgnore] public string SourceFile { get; set; } = null;

        /* Collections of systems */
        [XmlArray]
        [XmlArrayItem("FireControl", Type = typeof(FireControlSystem))]
        public List<ElectronicsSystem> Electronics { get; set; }

        [XmlArray]
        [XmlArrayItemAttribute("Screen", Type = typeof(ScreenSystem))]
        public List<DefenseSystem> Defenses { get; set; }

        [XmlArray]
        [XmlArrayItemAttribute("CargoHold", Type = typeof(CargoHoldSystem))]
        public List<CargoHoldSystem> Holds { get; set; }

        [XmlArray]
        [XmlArrayItem("PointDefense", Type = typeof(PointDefenseSystem))]
        [XmlArrayItem("BeamBattery", Type = typeof(BeamBatterySystem))]
        [XmlArrayItem("AntiMatterTorpedoLauncher", Type = typeof(AntiMatterTorpedoLauncherSystem))]
        public List<WeaponSystem> Weapons { get; set; }

        [XmlIgnore]
        public List<UnitSystem> AllSystems
        {
            get
            {
                if (_allSystems == null || _allSystems.Count == 0)
                    _allSystems = ComposeAllSystemsList();
                return _allSystems;
            }
        }

        private List<UnitSystem> ComposeAllSystemsList()
        {
            var allSystems = new List<UnitSystem>() {
                MainDrive,
                FtlDrive
            };
            allSystems.AddRange(Electronics);
            allSystems.AddRange(Defenses);
            allSystems.AddRange(Holds);
            allSystems.AddRange(Weapons);

            return allSystems;
        }

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
                return string.IsNullOrWhiteSpace(Name + FullID) ? "" : string.Format("{0} [{1}]", Name, FullID);
            }
        }

        public override string ToString()
        {
            //e.g. "Aragorn CA-001 -- Dunedain Ranger-class Heavy Cruiser -- TMF:45 / NPV:500"
            return $"{InstanceName} -- {Race} {ClassName}-class {ShipClass} -- TMF:{Mass} / NPV:{PointValue}";
        }

        public static Unit LoadNewUnit(string sourceFile)
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
                myNewUnit.SourceFile = sourceFile;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? "");
                //throw ex;
            }

            //If Unit lacks critical features like id's on its systems, add them
            var allSystems = myNewUnit.AllSystems;
            
            int nextID = 1;
            if(allSystems.Count > 0) nextID = Math.Max(allSystems.Max( x => x.id), 0) +1;
            allSystems.Where(x => x.id == -1).ToList().ForEach( x => x.id = nextID++);
            
            return myNewUnit;
        }

        internal static Unit Clone(Unit u)
        {
            //Copy all primitive types...
            var newU = (Unit) u.MemberwiseClone();

            //... clone all complex non-collection types...
            newU.Armor = u.Armor.Clone();
            newU.FtlDrive = u.FtlDrive.Clone();
            newU.MainDrive = u.MainDrive.Clone();

            //... initialize all collections...
            newU.Defenses = new List<DefenseSystem>();
            newU.Electronics = new List<ElectronicsSystem>();
            newU.Holds = new List<CargoHoldSystem>();
            newU.Orders = new List<VolleyOrders>();

            //... and fill all collections with copied values / elements.
            //TODO: Make a base SystemCollection class for all system collections to derive from
            //TODO: Implement a 'public T Clone<T>() where T:SystemCollection' method
            //TODO: Get rid of these system-collection foreach loops once the better Clone<T> method exists
            //TODO: Fix your Clone() method -- it doesn't work as implemented below for e.g. PointDefenseSystem : DefenseSystem
            (u.Defenses ?? newU.Defenses).ForEach(d => newU.Defenses.Add(d.Clone()));
            (u.Electronics ?? newU.Electronics).ForEach(e => newU.Electronics.Add(e.Clone()));
            (u.Holds ?? newU.Holds).ForEach(h => newU.Holds.Add(h.Clone()));
            foreach (var o in u.Orders ?? newU.Orders)
            {
                //newU.Orders.Add(VolleyOrders.Clone(o));
            }


            return newU;
        }
    }

}
