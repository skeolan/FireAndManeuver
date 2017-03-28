using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("FireControl")]
    public class FireControl : ElectronicsSystem
    {
        public FireControl()
        {
            systemName = "Fire Control System";
        }
    }

}