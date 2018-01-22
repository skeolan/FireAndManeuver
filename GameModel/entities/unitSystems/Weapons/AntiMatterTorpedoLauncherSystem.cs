namespace FireAndManeuver.GameModel
{

    public class AntiMatterTorpedoLauncherSystem : ArcWeaponSystem
    {
        public AntiMatterTorpedoLauncherSystem()
        {
            this.systemName = "Antimatter Torpedo Launcher";
            //Unless set otherwise, AMTs are 3-arc weapons bearing forward
            this.arcs       = "(FP/F/FS)";
        }
    }

}