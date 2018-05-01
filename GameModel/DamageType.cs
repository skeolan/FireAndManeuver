// <copyright file="DamageType.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.ComponentModel;

    public static partial class Constants
    {
#pragma warning disable SA1025 // Code must not contain multiple whitespace in a row
        /// <summary>
        /// Specifies what damage type(s) apply to a particular weapon attack.
        /// </summary>
        /// <remarks>For example, Grasers in the Continuum rules deal Penetrating SAP damage.</remarks>
        [Flags]
        public enum DamageType
        {
            [Description("Not Specified")]
            None        = 0b00000,

            [Description("Standard Damage")]
            Standard    = 0b00001,

            [Description("Penetrating Beam ('B*') Damage")]
            Penetrating = 0b00010,

            [Description("Semi-Armor-Piercing Damage")]
            SAP         = 0b01000,

            [Description("Armor-Piercing Damage")]
            AP          = 0b10000
        }
#pragma warning restore SA1025 // Code must not contain multiple whitespace in a row
    }
}
