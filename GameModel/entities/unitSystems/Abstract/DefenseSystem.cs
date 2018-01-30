using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class DefenseSystem : UnitSystem
    {
        public DefenseSystem()
        {
            SystemName = "Abstract Defense System";
        }
    }

}