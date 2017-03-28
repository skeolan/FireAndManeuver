using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    public abstract class ArcWeaponSystem : WeaponSystem
    {
        [XmlAttribute] public virtual string arcs { get; set; }
        public ArcWeaponSystem() : base()
        {
            this.systemName = "Arc-Firing Weapon System";
            this.arcs = "(F)";
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", base.ToString(), arcs);
        }
    }

}