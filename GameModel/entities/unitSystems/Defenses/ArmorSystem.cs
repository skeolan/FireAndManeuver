using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("Armor")]
    public class ArmorSystem : DefenseSystem
    {
        public ArmorSystem() {
            SystemName = "Armor System";
        }

        [XmlAttribute] public string totalArmor { get; set; }
        [XmlAttribute] public string remainingArmor { get; set; }

        //public int[] armorLayers { get { return new List<string>(totalArmor.Split(",")).ForEach() } }

        public override string ToString()
        {
            return string.Format($"{totalArmor, 3} / {remainingArmor ?? totalArmor, 3}");
        }
    }

}