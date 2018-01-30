using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("FTLDrive")]
    public class FTLDriveSystem : UnitSystem
    {
        [XmlAttribute] public bool active { get; set; } = false;
        public FTLDriveSystem()
        {
            SystemName = "FTL Drive System";
        }
        public override string ToString()
        {
            return string.Format("{0} - {1}", base.ToString(), this.active ? "Active" : "Inactive");
        }
    }

}