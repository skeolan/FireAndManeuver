using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class UnitOrders
    {
        [XmlAttribute("target")] public string TargetID {get; set;}="*";
        [XmlAttribute("priority")] public string Priority {get; set;}="secondary";

        public UnitOrders(){}

        public override string ToString()
        {
            string tgtStr = TargetID=="*" ? "Default" : $"Target:[{TargetID}]";
            string priStr = Priority.ToLowerInvariant()=="primary" ? "(Primary)" : "";
            return $"{tgtStr}{priStr}";
        }
    }
}