// <copyright file="GameEngineOptions.cs" company="Patrick Maughan">
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
    public class GameEngineOptions
    {
        public const int RangeBandDefault = 6;

        internal static GameEngineOptions Clone(GameEngineOptions gameOptions)
        {
            // Clone with all primitive types set to the same value
            GameEngineOptions newOpt = (GameEngineOptions)gameOptions.MemberwiseClone();

            // No non-static, non-primitive options supported yet!
            return newOpt;
        }
    }
}