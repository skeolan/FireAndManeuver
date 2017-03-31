using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FM.Maneuver")]
    public class ManeuverOrder:UnitOrders
    {
        [XmlAttribute("type")] public string ManeuverType {get; set;}="Maintain";        
        public ManeuverOrder(){}

        public override string ToString()
        {        
            return $"{ManeuverType, -8}  - {base.ToString(), -30}";
        }
    }
}