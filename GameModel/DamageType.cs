// <copyright file="DamageType.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.ComponentModel;

    public static partial class Constants
    {
        /// <summary>
        /// Specifies what damage type(s) apply to a particular weapon attack.
        /// </summary>
        /// <remarks>For example, Grasers in the Continuum rules deal Penetrating SAP damage.</remarks>
        [Flags]
        public enum DamageType
        {
            [Description("Standard Damage")]
            Standard = 0,

            [Description("Penetrating Beam ('B*') Damage")]
            Penetrating = 1,

            [Description("Semi-Armor-Piercing Damage")]
            SAP = 2,

            [Description("Armor-Piercing Damage")]
            AP = 4
        }
    }
}
