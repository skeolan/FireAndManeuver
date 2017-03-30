using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FM.Maneuver")]
    public class ManeuverOrder
    {
        [XmlAttribute("type")] public string maneuverType {get; set;}="Maintain";
        [XmlAttribute("target")] public string targetID {get; set;}="*";
        [XmlAttribute("priority")] public string priority {get; set;}="secondary";

        public ManeuverOrder(){}

        public override string ToString()
        {
            string tgtStr = targetID=="*" ? "Default" : "Target:"+targetID;
            string priStr = priority.ToLowerInvariant()=="primary" ? "(Primary)" : "";
            return $"{tgtStr}:{maneuverType}{priStr}";
        }
    }
}