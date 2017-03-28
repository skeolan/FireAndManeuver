using System;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("Armor")]
    public class ArmorSystem : UnitSystem
    {
        public ArmorSystem() { }

        [XmlAttribute] public string totalArmor { get; set; }
        [XmlAttribute] public string remainingArmor { get; set; }

        //public int[] armorLayers { get { return new List<string>(totalArmor.Split(",")).ForEach() } }

        public override string ToString()
        {
            string remArmorStr = string.IsNullOrWhiteSpace(remainingArmor) ? "" : String.Format(" ({0} remaining)", remainingArmor) ;
            return string.Format("{0}{1}", this.totalArmor, remArmorStr);
        }
    }

}