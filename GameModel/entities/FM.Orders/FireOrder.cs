// <copyright file="FireOrder.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    /// <summary>
    /// Orders for a weapons-fire targeting assignment in a single volley for a single Formation.
    /// </summary>
    [XmlRoot("Fire")]
    public class FireOrder : FormationOrder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireOrder"/> class.
        /// </summary>
        public FireOrder()
        {
        }

        [XmlAttribute("type")]
        public string FireType { get; set; } = "Fire";

        [XmlAttribute("diceAssigned")]
        public int DiceAssigned { get; set; } = 0;

        /// <summary>
        /// Custom string representation of a <see cref="FireOrder" class./>
        /// </summary>
        /// <returns>String representation of the <see cref="FireOrder"/> object.</returns>
        public override string ToString()
        {
            // return $"{priStr} {fcStr} {tgtStr}:{priStr}({WeaponIDs})".Trim();
            var diceAssigned = this.DiceAssigned > 0 ? $"({this.DiceAssigned}D)" : string.Empty;
            return $"{this.FireType}{diceAssigned} - {base.ToString()}";
        }
    }
}