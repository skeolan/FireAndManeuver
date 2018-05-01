// <copyright file="AttackSpecialProperties.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

#pragma warning disable SA1025 // Code must not contain multiple whitespace in a row
namespace FireAndManeuver.GameModel
{
    using System;

    [Flags]
    public enum AttackSpecialProperties
    {
        None                        = 0b00000000,
        Area_Attack                 = 0b00000001, // Affects all Units in the target Formation
        Ignores_Evasion             = 0b00000010,
        Ignores_Standard_Screens    = 0b00000100,
        Ignores_Advanced_Screens    = 0b00001000, // This may not actually apply to any systems in Continuum rules.
        Vulnerable_To_ECM           = 0b00010000,
        Vulnerable_To_PD            = 0b00100000,
        Overrides_Weapon_Properties = 0b01000000,
        Overrides_Weapon_DamageType = 0b10000000
    }
}
#pragma warning restore SA1025 // Code must not contain multiple whitespace in a row
