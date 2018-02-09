// <copyright file="AntiMatterTorpedoLauncherSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class AntiMatterTorpedoLauncherSystem : ArcWeaponSystem
    {
        public AntiMatterTorpedoLauncherSystem()
        {
            this.SystemName = "Antimatter Torpedo Launcher";

            // Unless set otherwise, AMTs are 3-arc weapons bearing forward
            this.Arcs = "(FP/F/FS)";
        }
    }
}