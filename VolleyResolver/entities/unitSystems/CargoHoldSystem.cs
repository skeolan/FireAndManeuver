using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    public class CargoHoldSystem : UnitSystem
    {
        [XmlAttribute] public string type;

        [XmlAttribute] public int totalSize;

        public CargoHoldSystem() : base()
        {
            systemName = "Cargo Hold";
        }

        public override string ToString()
        {
            return string.Format("{0} [Total Size {1}]", base.ToString(), totalSize);
        }

    }

}