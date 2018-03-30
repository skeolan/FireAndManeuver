// <copyright file="AttackSpecialProperties.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;

    [Flags]
    public enum AttackSpecialProperties
    {
        None = 0,
        Area_Attack = 1, // Affects all Units in the target Formation
        Ignores_Evasion = 2,
        Ignores_Screens = 4,
        Vulnerable_To_ECM = 8,
        Vulnerable_To_PD = 16
    }
}
