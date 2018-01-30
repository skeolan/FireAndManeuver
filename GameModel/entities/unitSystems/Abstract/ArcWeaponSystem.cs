using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class ArcWeaponSystem : WeaponSystem
    {

        [XmlAttribute] public virtual string arcs { get; set; }
        public ArcWeaponSystem() : base()
        {
            SystemName = "Abstract Arc-Firing Weapon System";
            arcs = "(F)";
        }

        public override string ToString()
        {
            return $"{base.ToString()} - {arcs}";
        }
    }

}