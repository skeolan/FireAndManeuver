// <copyright file="ArmorSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    [XmlRoot("Armor")]
    public class ArmorSystem : DefenseSystem
    {
        public ArmorSystem()
        {
            this.SystemName = "Armor System";
        }

        [XmlAttribute("totalArmor")]
        public string TotalArmor { get; set; }

        [XmlAttribute("remainingArmor")]
        public string RemainingArmor { get; set; }

        // public int[] armorLayers { get { return new List<string>(totalArmor.Split(",")).ForEach() } }
        public override string ToString()
        {
            return string.Format($"{this.TotalArmor, 3} / {this.RemainingArmor ?? this.TotalArmor, 3}");
        }
    }
}