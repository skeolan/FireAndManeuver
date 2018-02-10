// <copyright file="FireOrder.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    /// <summary>
    /// Orders for a single weapons fire execution in a single volley for a single Unit.
    /// </summary>
    [XmlRoot("FM.Fire")]
    public class FireOrder : UnitOrders
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireOrder"/> class.
        /// </summary>
        public FireOrder()
        {
        }

        /// <summary>
        /// Gets or sets the ID of a <see cref="FireControlSystem"/> object assigned to govern this FireOrder.
        /// 0 (no assignment) is acceptable, but forces all assigned <see cref="WeaponSystem"/>s to operate in Point Defense mode.
        /// </summary>
        [XmlAttribute("fireConID")]
        public int FireConID { get; set; } = 0;

        /// <summary>
        /// Gets or sets set of weapon IDs covered by the current order.
        /// </summary>
        [XmlAttribute("weaponIDs")]
        public string WeaponIDs { get; set; } = string.Empty;

        /*
         *[XmlIgnore] private Unit _unit { get; set; }
         */

        /// <summary>
        /// Custom string representation of a <see cref="FireOrder" class./>
        /// </summary>
        /// <returns>String representation of the <see cref="FireOrder"/> object.</returns>
        public override string ToString()
        {
            string fcStr = this.FireConID == 0 ? "FC[*]" : $"FC[{this.FireConID:00}]";
            int wepCt = this.WeaponIDs.Split(',').Length;
            string wepStr = this.WeaponIDs == string.Empty ? string.Empty : $"Weapons[x{wepCt:00}]";

            // return $"{priStr} {fcStr} {tgtStr}:{priStr}({WeaponIDs})".Trim();
            return $"{fcStr, -8} {wepStr} - {base.ToString(), -30}";
        }
    }
}