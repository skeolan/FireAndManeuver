// <copyright file="GameUnit.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("Ship")]
    public class GameUnit
    {
        private string damageControlRaw = string.Empty;
        private List<UnitSystem> allSystems;

        public GameUnit()
        {
        }

        [XmlAttribute("id")]
        public int IdNumeric { get; set; } = 1;

        [XmlIgnore]
        public string FullID
        {
            get
            {
                var raceToken = this.Race.Split(' ')[0];
                return $"{raceToken}-{this.ClassAbbrev}-{this.IdNumeric:000}";
            }

            set
            {
                var raceToken = this.Race.Split(' ')[0];
                string valueNumeric = value.Replace(raceToken, string.Empty).Replace(this.ClassAbbrev, string.Empty).Replace("-", string.Empty);
                int newID = this.IdNumeric;
                if (!int.TryParse(value, out newID) | newID < 1)
                {
                    throw new ArgumentException($"New ID assignment {value} must be a positive integer value or a string like {this.Race}-{this.ClassAbbrev}-###.");
                }

                this.IdNumeric = newID;
            }
        }

        [XmlElement]
        public string Name { get; set; } = string.Empty;

        [XmlElement]
        public string Status { get; set; } = "Ok";

        // [XmlElement("Orders")] public string orders { get; set; }
        [XmlElement("DamageControl")]
        public string DamageControlRaw
        {
            get
            {
                return this.damageControlRaw;
            }

            set
            {
                this.damageControlRaw = value;
            }
        }

        [XmlIgnore]
        public List<int> DamageControl
        {
            get
            {
                return this.damageControlRaw.Split(',').Select(int.Parse).ToList();
            }

            set
            {
                this.damageControlRaw = string.Join(",", value);
            }
        }

        // [XmlElement] public string Position { get; set; }
        // [XmlElement] public string Heading { get; set; }
        // [XmlElement] public string Speed { get; set; }
        // [XmlElement] public string VectorSpeed { get; set; }
        // [XmlElement] public string Course { get; set; }
        [XmlElement]
        public string CrewQuality { get; set; } = "Average";

        [XmlElement]
        public string Race { get; set; } = "Generic";

        [XmlElement]
        public string ClassAbbrev { get; set; } = "XX";

        [XmlElement]
        public string ClassName { get; set; } = "Unit";

        [XmlElement]
        public string ShipClass { get; set; } = "Unnamed";

        [XmlElement]
        public int Mass { get; set; } = 0;

        [XmlElement]
        public int PointValue { get; set; } = 0;

        [XmlElement]
        public DriveSystem MainDrive { get; set; } = new DriveSystem();

        [XmlElement("FTLDrive")]
        public FTLDriveSystem FtlDrive { get; set; } = null;

        [XmlElement]
        public ArmorSystem Armor { get; set; } = null;

        [XmlElement]
        public HullSystem Hull { get; set; } = null;

        [XmlIgnore]
        public string SourceFile { get; set; } = null;

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

        [XmlArray]
        [XmlArrayItem("Record", Type = typeof(GameUnitRecord))]
        public List<GameUnitRecord> Log { get; set; }

        [XmlIgnore]
        public List<UnitSystem> AllSystems
        {
            get
            {
                if (this.allSystems == null || this.allSystems.Count == 0)
                {
                    this.allSystems = this.ComposeAllSystemsList();
                }

                return this.allSystems;
            }
        }

        [XmlArray("FM.Orders")]
        [XmlArrayItem("FM.VolleyOrders", Type = typeof(VolleyOrders))]
        public List<VolleyOrders> Orders { get; set; } = new List<VolleyOrders>();

        public string InstanceName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.Name + this.FullID) ? string.Empty : string.Format("{0} [{1}]", this.Name, this.FullID);
            }
        }

        // Non-Property methods
        public static GameUnit LoadNewUnit(string sourceFile)
        {
            XmlSerializer srz = new XmlSerializer(typeof(GameUnit));

            FileStream fs;
            GameUnit myNewUnit = null;

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
                myNewUnit = (GameUnit)srz.Deserialize(fs);
                myNewUnit.SourceFile = sourceFile;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine("XML {0} is not a supported Unit design: {1} -- {2}", sourceFile, ex.Message, ex.InnerException.Message ?? string.Empty);

                // throw ex;
            }

            // If Unit lacks critical features like id's on its systems, add them
            var allSystems = myNewUnit.AllSystems;

            int nextID = 1;
            if (allSystems.Count > 0)
            {
                nextID = Math.Max(allSystems.Max(x => x.Id), 0) + 1;
            }

            allSystems.Where(x => x.Id == -1).ToList().ForEach(x => x.Id = nextID++);

            return myNewUnit;
        }

        public override string ToString()
        {
            // e.g. "Aragorn CA-001 -- Dunedain Ranger-class Heavy Cruiser -- TMF:45 / NPV:500"
            return $"{this.InstanceName} -- {this.Race} {this.ClassName}-class {this.ShipClass} -- TMF:{this.Mass} / NPV:{this.PointValue}";
        }

        public ManeuverResult ResolveManeuver(VolleyOrders orders, int speedDRM = 0, int evasionDRM = 0)
        {
            Console.WriteLine($"Resolving maneuver for {this.InstanceName}");

            int speedSuccesses = 0;
            int evasionSuccesses = 0;
            var roller = new DiceNotationUtility() as IDiceUtility;

            // Roll for Speed
            speedSuccesses = roller.RollFTSuccesses(orders.Speed);

            // Roll for Evasion
            evasionSuccesses = roller.RollFTSuccesses(orders.Evasion);

            return new ManeuverResult() { SpeedSuccesses = speedSuccesses, EvasionSuccesses = evasionSuccesses };
        }

        internal static GameUnit Clone(GameUnit u)
        {
            // Copy all primitive types...
            var newU = (GameUnit)u.MemberwiseClone();

            // ... clone all complex non-collection types...
            newU.Armor = u.Armor.Clone();
            newU.FtlDrive = u.FtlDrive.Clone();
            newU.MainDrive = u.MainDrive.Clone();

            // ... initialize all collections...
            newU.Defenses = new List<DefenseSystem>();
            newU.Electronics = new List<ElectronicsSystem>();
            newU.Holds = new List<CargoHoldSystem>();
            newU.Orders = new List<VolleyOrders>();

            // ... and fill all collections with copied values / elements.
            // TODO: Make a base SystemCollection class for all system collections to derive from
            // TODO: Implement a 'public T Clone<T>() where T:SystemCollection' method
            // TODO: Get rid of these system-collection foreach loops once the better Clone<T> method exists
            // TODO: Fix your Clone() method -- it doesn't work as implemented below for e.g. PointDefenseSystem : DefenseSystem
            (u.Defenses ?? newU.Defenses).ForEach(d => newU.Defenses.Add(d.Clone()));
            (u.Electronics ?? newU.Electronics).ForEach(e => newU.Electronics.Add(e.Clone()));
            (u.Holds ?? newU.Holds).ForEach(h => newU.Holds.Add(h.Clone()));
            foreach (var o in u.Orders ?? newU.Orders)
            {
                // newU.Orders.Add(VolleyOrders.Clone(o));
            }

            return newU;
        }

        private List<UnitSystem> ComposeAllSystemsList()
        {
            var allSystems = new List<UnitSystem>()
            {
                this.MainDrive,
                this.FtlDrive
            };
            allSystems.AddRange(this.Electronics);
            allSystems.AddRange(this.Defenses);
            allSystems.AddRange(this.Holds);
            allSystems.AddRange(this.Weapons);

            return allSystems;
        }
    }
}
