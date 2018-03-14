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
        }

        public int RollPercentile() => 99;

        public int RollD6() => 6;

        public int RollDie(int sides) => 6;

        public List<int> RollXDY(int numberOfDice, int numberOfSides) => new List<int> { 6, 6 };

        public List<int> RollStandardDice(int numberOfDice) => new List<int> { 6, 6 };

        public List<int> RollExplodingDice(int numberOfDice, int numberOfSides = 6, int explodeMinimum = 6) => new List<int> { 6, 6 };

        public int RollSuccesses(int numberOfDice, int dRM = 0) => (int)(numberOfDice * 1.5);

        public int RollFTSuccesses(int numberOfDice, out List<int> rolls, int dRM = 0, int difficultyLevel = 0)
        {
            Console.WriteLine($"MOCK DICE FTSuccesses die roll : {numberOfDice}D6 with DRM {dRM}");
            rolls = new List<int>();
            return (int)(numberOfDice * 1.5);
        }
    }
}