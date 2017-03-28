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
            get { return string.Format("{0} Drive", this.type); }
            protected set {}
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}/{2} - {3}", base.ToString(), this.initialThrust, this.currentThrust, this.active ? "Active" : "Inactive");
        }
    }

}