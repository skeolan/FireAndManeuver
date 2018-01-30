using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("MainDrive")]
    public class DriveSystem : UnitSystem
    {
        [XmlIgnore] private int? _currentThrust = null;
        [XmlAttribute] public string type { get; set; } = "Standard";
        [XmlAttribute] public int initialThrust { get; set; } = 0;
        
        [XmlAttribute] public int currentThrust { 
            get { return _currentThrust ?? initialThrust; } 
            set { _currentThrust = value; } 
        }
        [XmlAttribute] public bool active { get; set; } = false;
        public DriveSystem()
        {
            SystemName = "Standard Drive System";
        }


        public override string ToString()
        {
            return $"{base.ToString()} - {(this.active ? "Active" : "Inactive")}";
        }
    }

}