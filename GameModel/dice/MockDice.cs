// <copyright file="MockDice.cs" company="Patrick Maughan">
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
    using OnePlat.DiceNotation.DieRoller;

    public class MockDice : IDiceUtility
    {
        // Constructor
        public MockDice()
        {
            // N/A
        }

        public int RollD6() => 6;

        public List<int> RollXDY(int numberOfDice, int numberOfSides) => new List<int> { 6, 6 };

        public IEnumerable<int> RollStandardDice(int numberOfDice)
        {
            for (int i = 0; i < numberOfDice; i++)
            {
                yield return this.RollD6();
            }
        }

        public IEnumerable<int> RollExplodingDice(int numberOfDice, int numberOfSides, int explodeMinimum)
        {
            var result = new List<int>();

            for (int i = 0; i < numberOfDice; i++)
            {
                result.Add(explodeMinimum);
                result.Add(explodeMinimum - 1);
            }

            return result;
        }

        public int RollPercentile() => 99;

        public int RollSuccesses(int numberOfDice, int dRM = 0, int targetNumber = 4, int doubleSuccessNumber = 6)
            => numberOfDice;
    }
}