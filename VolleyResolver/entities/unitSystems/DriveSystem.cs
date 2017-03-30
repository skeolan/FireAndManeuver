using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("MainDrive")]
    public class DriveSystem : UnitSystem
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
            get { return $"{this.type} Drive {this.currentThrust}/{this.initialThrust}"; }
            protected set {}
        }

        public override string ToString()
        {
            return $"{base.ToString()} - {(this.active ? "Active" : "Inactive")}";
        }
    }

}