using System.Collections.Generic;

namespace FireAndManeuver.GameEngine
{
    public abstract class WeaponSystem : UnitSystem
    {
        public WeaponSystem() : base()
        {
            systemName = "BaseWeaponSystem Class";
        }

        public List<int> Attack(Unit target, Unit attacker=null)
        {
            List<int> damageMatrix = new List<int>() {0};

            return damageMatrix;
        }
    }

}