using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public class PointDefenseSystem : WeaponSystem
    {
        [XmlIgnore] public new string SystemName { get; private set; } = "Point Defense System";
        public PointDefenseSystem() : base()
        {
            SystemName = "Point Defense System";
        }
    }

}