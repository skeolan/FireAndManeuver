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

        public Constants.DamageType DamageType { get; set; } = Constants.DamageType.Standard;

        public override string ToString()
        {
            var standardStr = $"{this.Standard}";
            var penetratingStr = this.Penetrating > 0 ? $"+{this.Penetrating}p" : string.Empty;

            return $"{standardStr}{penetratingStr} {this.DamageType.ToString()} Damage";
        }

        public string RollString()
        {
            var standardRollStr = $"({string.Join(", ", this.StandardRolls)})";
            var penetratingRollStr = this.PenetratingRolls.Count > 0 ? $"+({string.Join(", ", this.PenetratingRolls)})" : string.Empty;

            return $"{standardRollStr}{penetratingRollStr}";
        }
    }
}