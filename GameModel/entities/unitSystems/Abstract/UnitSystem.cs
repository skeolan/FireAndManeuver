using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class UnitSystem
    {
        [XmlAttribute("id")] public int id { get; set; } = -1;
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
            string idStr = id == -1 ? "" : string.Format("[{0:00}]", id);
            return $"{idStr,2} - {systemName,-30} - {status,-12}";
        }
    }

}