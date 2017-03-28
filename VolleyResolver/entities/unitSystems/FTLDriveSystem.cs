using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : UnitSystem
    {
        [XmlAttribute] public bool active { get; set; } = false;
        public FTLDriveSystem() { systemName = "FTL Drive"; }
        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), this.active ? "Active" : "Inactive");
        }
    }

}