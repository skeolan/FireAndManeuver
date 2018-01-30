using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{

    public class AntiMatterTorpedoLauncherSystem : ArcWeaponSystem
    {

        public AntiMatterTorpedoLauncherSystem()
        {
            SystemName = "Antimatter Torpedo Launcher";
            //Unless set otherwise, AMTs are 3-arc weapons bearing forward
            arcs       = "(FP/F/FS)";
        }
    }

}