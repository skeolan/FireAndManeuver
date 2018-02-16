// <copyright file="ManeuverOrder.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    [XmlRoot("Maneuver")]
    public class ManeuverOrder : UnitOrders
    {
        public ManeuverOrder()
        {
        }

        [XmlAttribute("type")]
        public string ManeuverType { get; set; } = "Maintain";

        public override string ToString() => $"{this.ManeuverType}  - {base.ToString()}";
    }
}