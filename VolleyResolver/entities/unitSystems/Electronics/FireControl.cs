using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FireControl")]
    public class FireControlSystem : ElectronicsSystem
    {
        public FireControlSystem()
        {
            systemName = "Fire Control System";
        }
    }

}