// <copyright file="PulseTorpedoSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Xml.Serialization;

    public class PulseTorpedoSystem : ArcWeaponSystem
    {
        public PulseTorpedoSystem()
        {
            this.SystemName = "Pulse Torpedo System";

            // Unless set otherwise, PulseTorpss are 1-arc weapons bearing forward
            this.Arcs = "(F)";
        }
    }
}