// <copyright file="GamePhase.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    public static partial class Constants
    {
        public enum GamePhase
        {
            ThrustAllocationPhase,
            ManeuveringPhase,
            FiringPhase,
            DamageControlPhase
        }
    }
}
