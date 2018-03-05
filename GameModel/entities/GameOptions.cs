// <copyright file="GameOptions.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("GameOptions")]
    public class GameOptions
    {
        public const int RangeBandDefault = 6;
        public const int DefaultStartingRange = 60;

        internal static GameOptions Clone(GameOptions gameOptions)
        {
            // Clone with all primitive types set to the same value
            GameOptions newOpt = (GameOptions)gameOptions.MemberwiseClone();

            // No non-static, non-primitive options supported yet!
            return newOpt;
        }
    }
}