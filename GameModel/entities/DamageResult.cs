// <copyright file="DamageResult.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Collections.Generic;

    public class DamageResult
    {
        public int Standard { get; set; } = 0;

        public int Penetrating { get; set; } = 0;

        public List<int> StandardRolls { get; set; } = new List<int>();

        public List<int> PenetratingRolls { get; set; } = new List<int>();
    }
}