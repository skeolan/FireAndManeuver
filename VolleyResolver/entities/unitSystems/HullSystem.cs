using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    public class HullSystem : UnitSystem
    {
        [XmlAttribute] public int value { get; set; }
        [XmlAttribute] public HullTypeLookup type { get; set; }
        [XmlAttribute("class")] public string hullClass { get; set; }
        [XmlAttribute]
        public int totalHullBoxes
        {
            get
            {
                //if (type == HullTypeLookup.Custom) return _totalHullBoxes;
                if (_totalHullBoxes != -1) return _totalHullBoxes;
                else
                {
                    //Automatically rounds DOWN?
                    return (int)(this.value * this.hullTypeMultiplier);
                }
            }
            set
            {
                this._totalHullBoxes = value;
            }
        }

        private int _totalHullBoxes = -1;
        private int _remainingHullBoxes = -1;

        [XmlIgnore] private decimal hullTypeMultiplier { get { return (int)type / (decimal)100.0; } }
        [XmlAttribute] public int rows { get; set; }
        [XmlAttribute] public int remainingHullBoxes 
        { 
            get
            {
                if(_remainingHullBoxes != -1) return _remainingHullBoxes;
                else return _totalHullBoxes;
            } 
            set
            {
                _remainingHullBoxes = value;
            }
        }

        public HullSystem()
        {
            rows = 4;
        }

        public override string ToString()
        {
            string typeSuffix = type == HullTypeLookup.Custom ? "Custom" : string.Format("{0} [MUx{1}]", System.Enum.GetName(typeof(HullTypeLookup), type), hullTypeMultiplier);
            return string.Format("{0}/{1} ({2} rows) {3} {4}", remainingHullBoxes, totalHullBoxes, rows, typeSuffix, hullClass);
        }

    }

}