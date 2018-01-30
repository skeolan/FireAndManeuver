using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public abstract class ElectronicsSystem : UnitSystem
    {
        public ElectronicsSystem()
        {
            SystemName = "Abstract Electronics System";
        }
    }

}