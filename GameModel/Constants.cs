// <copyright file="Constants.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Set of known constant values for the Game Model
    /// TODO: Combine this with the GameModelOptions class.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Die roll value that meets minimum for success in a typical roll (in FT Continuum, 4).
        /// </summary>
        public const int MinRollForSuccess = 4;

        /// <summary>
        /// Unmodified die roll value that counts as a double-hit or double-success (in FT Continuum, a "natural" 6).
        /// </summary>
        public const int MinRollForDoubleSuccess = 6;
    }
}
