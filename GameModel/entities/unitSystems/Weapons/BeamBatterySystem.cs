using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public class BeamBatterySystem : ArcWeaponSystem
    {
        private string _arcs;
        [XmlAttribute] public int rating { get; set; }=1;
        [XmlAttribute] public override string arcs {
            get
            {
                var arcString = rating ==1 ? "(All arcs)" : string.IsNullOrWhiteSpace(_arcs) ? "(F)" : _arcs;
                return arcString;
            }
            set
            {
                _arcs = value;
            }
        }
        [XmlIgnore] public override string systemName
        {
            get { return string.Format("Class-{0} Beam Battery", this.rating); }
            protected set {}
        }

        public BeamBatterySystem() : base()
        {

        }
    }

}