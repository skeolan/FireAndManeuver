// <copyright file="DiceNotationUtility.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using OnePlat.DiceNotation;
    using OnePlat.DiceNotation.DiceTerms;
    using OnePlat.DiceNotation.DieRoller;

    public class DiceNotationUtility : IDiceUtility
    {
        private RandomDieRoller roller = new RandomDieRoller();
        private IDice dice = new Dice();

        public int RollD6()
        {
            var r = this.dice.Roll("1d6", this.roller);

            Debug.WriteLine($"RollD6: {r.Value}");

            return (int)r.Value;
        }

        public List<int> RollXDY(int numberOfDice, int numberOfSides)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> RollStandardDice(int numberOfDice)
        {
            return this.dice.Roll($"{numberOfDice}d6", this.roller).Results.Select(r => r.Value);
        }

        public IEnumerable<int> RollExplodingDice(int numberOfDice, int numberOfSides = 6, int explodeMinimum = 6)
        {
            var r = this.dice.Roll($"{numberOfDice}d{numberOfSides}!{explodeMinimum}", this.roller);

            Debug.WriteLine($"RollExplodingDice: ({string.Join(", ", r.Results.Select(roll => roll.Value))})");

            IEnumerable<int> result = r.Results.Select(d => d.Value);

            return result;
        }

        public int RollPercentile()
        {
            var r = this.dice.Roll("1d100", this.roller);

            Debug.WriteLine($"RollPercentile: {r.Value}");

            return r.Value;
        }

        public int RollSuccesses(int numberOfDice, int dRM = 0, int targetNumber = 4, int doubleSuccessNumber = 6)
        {
            var rolls = this.dice.Roll($"{numberOfDice}(d6-{dRM})", this.roller);

            var standardSuccesses = rolls.Results.Where(i => i.Value >= targetNumber).Count();
            var extraSuccesses = rolls.Results.Where(i => i.Value >= doubleSuccessNumber).Count();

            return standardSuccesses + extraSuccesses;
        }
    }
}