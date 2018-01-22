using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
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