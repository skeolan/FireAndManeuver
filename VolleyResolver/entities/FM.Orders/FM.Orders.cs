using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FM.Orders")]
    public class Orders
    {
        [XmlElement("FM.Speed")] public int Speed {get; set;}
        [XmlElement("FM.Evasion")] public int Evasion {get; set;}
        
        [XmlArray("FM.Maneuvers")]
        [XmlArrayItemAttribute("FM.Maneuver", Type = typeof(ManeuverOrder))] 
        public List<ManeuverOrder> _maneuvers {get; set; }
       
        public List<ManeuverOrder> Maneuvers { 
            get 
            { 
                if(_maneuvers==null || _maneuvers.Count == 0)
                    _maneuvers = new List<ManeuverOrder>() { new ManeuverOrder() };

                return _maneuvers.OrderBy(x => x.priority.ToLowerInvariant() != "primary").ToList();
            }
            set 
            {
                _maneuvers = value;
            }
        }

        public Orders()
        {
            this.Speed = 0;
            this.Evasion = 0;
            this._maneuvers = new List<ManeuverOrder>();
        }

        public override string ToString()
        {
            //string maneuverStrings = "...";
            var maneuverStrings = string.Join("; ", Maneuvers.Select( m => m.ToString()) );
            return $"Speed {Speed} - Evasion {Evasion} - target(s): [{maneuverStrings}]";
        }
    }
}