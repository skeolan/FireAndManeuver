using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public class CargoHoldSystem : UnitSystem
    {
        [XmlAttribute] public string type;

        [XmlAttribute] public int totalSize;

        public CargoHoldSystem() : base()
        {
            SystemName = "Cargo Hold System";
        }

        public override string ToString()
        {
            return string.Format("{0} [Total Size {1}]", base.ToString(), totalSize);
        }

    }

}