using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("FM.VolleyOrders")]
    public class VolleyOrders
    {
        [XmlAttribute] public int volley {get; set;}=0;
        [XmlElement("FM.Speed")] public int Speed { get; set; }
        [XmlElement("FM.Evasion")] public int Evasion { get; set; }

        [XmlArray("FM.ManeuveringOrders")]
        [XmlArrayItemAttribute("FM.Maneuver", Type = typeof(ManeuverOrder))]
        public List<ManeuverOrder> _maneuvers { get; set; }

        [XmlArray("FM.FiringOrders")]
        [XmlArrayItemAttribute("FM.Fire", Type = typeof(FireOrder))]
        public List<FireOrder> _firing { get; set; }  = new List<FireOrder> { };

        public List<ManeuverOrder> ManeuveringOrders
        {
            get
            {
                if (_maneuvers == null || _maneuvers.Count == 0)
                    _maneuvers = new List<ManeuverOrder>() { new ManeuverOrder() };

                return _maneuvers.OrderBy(x => x.Priority.ToLowerInvariant() != "primary").ToList();
            }
            set
            {
                _maneuvers = value;
            }
        }

        public List<FireOrder> FiringOrders
        {
            get {
                return _firing.OrderByDescending(x => x.TargetID.ToLowerInvariant() != "pd").OrderByDescending(x => x.Priority.ToLowerInvariant() == "primary").ToList();
            }
            set {
                _firing = value;
            }
        }

        public VolleyOrders()
        {
            Speed = 0;
            Evasion = 0;
            _maneuvers = new List<ManeuverOrder>();
            _firing = new List<FireOrder>();
        }

        public override string ToString()
        {
            var maneuverStrings = string.Join("; ", ManeuveringOrders.Select(m => m.ToString()));
            var firingStrings = string.Join(";", FiringOrders.Select(f => f.ToString()));
            return $"Speed {Speed} - Evasion {Evasion} | Maneuvering: [{maneuverStrings}] | Firing: [{firingStrings}]";
        }

        internal static VolleyOrders Clone(VolleyOrders o)
        {
            throw new NotImplementedException();
        }
    }
}