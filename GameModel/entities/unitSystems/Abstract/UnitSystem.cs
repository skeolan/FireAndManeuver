using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class UnitSystem
    {
        [XmlIgnore] public string SystemName { get; protected set; } = "Abstract base Unit System";

        [XmlAttribute("id")] public int id { get; set; } = -1;
        [XmlAttribute("xSSD")] public int xSSD { get; set; }
        [XmlAttribute("ySSD")] public int ySSD { get; set; }
        [XmlAttribute] public string status { get; set; } = "Operational";

        public UnitSystem()
        {

        }

        public override string ToString()
        {
            string idStr = this.id == -1 ? "" : string.Format("[{0:00}]", this.id);
            return $"{idStr,2} - {this.SystemName,-30} - {this.status,-12}";
        }

        //As long as all properties are primitive type, no need to override this for derived classes
        public virtual dynamic Clone()
        {
            return (dynamic)this.MemberwiseClone();
        }
    }

}