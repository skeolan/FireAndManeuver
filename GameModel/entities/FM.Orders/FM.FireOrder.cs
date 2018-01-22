using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("FM.Fire")]
    public class FireOrder:UnitOrders
    {
        [XmlAttribute("fireConID")] public int FireConID {get; set;}=0;
        [XmlAttribute("weaponIDs")] public string WeaponIDs {get; set;}="";
        //[XmlIgnore] private Unit _unit { get; set; }

        public FireOrder(){}

        public override string ToString()
        {
            string fcStr  = FireConID == 0 ? "FC[*]" : $"FC[{FireConID:00}]";
            int wepCt = WeaponIDs.Split(',').Length;
            string wepStr = WeaponIDs == "" ? "" : $"Weapons[x{wepCt:00}]";
            //return $"{priStr} {fcStr} {tgtStr}:{priStr}({WeaponIDs})".Trim();

            return $"{fcStr, -8} {wepStr} - {base.ToString(), -30}";
        }
    }
}