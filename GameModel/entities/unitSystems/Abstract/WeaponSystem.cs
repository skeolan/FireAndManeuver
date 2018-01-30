using System.Xml.Serialization;
using System.Collections.Generic;

namespace FireAndManeuver.GameModel
{
    public abstract class WeaponSystem : UnitSystem
    {
        public WeaponSystem() : base()
        {
            SystemName = "Abstract Weapon System";
        }

        public List<int> Attack(Unit target, Unit attacker=null)
        {
            List<int> damageMatrix = new List<int>() {0};

            return damageMatrix;
        }
    }

}