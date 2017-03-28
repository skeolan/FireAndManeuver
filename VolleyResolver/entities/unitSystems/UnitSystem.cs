using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    public abstract class UnitSystem
    {
        [XmlAttribute("id")] public int id { get; set; }
        [XmlAttribute("xSSD")] public int xSSD { get; set; }
        [XmlAttribute("ySSD")] public int ySSD { get; set; }
        [XmlIgnore] public virtual string systemName { get; protected set; }
        [XmlAttribute] public string status { get; set; } = "Operational";

        public UnitSystem()
        {
            systemName = "Universal unitSystem class";
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", systemName, status);
        }
    }

}