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

        public List<int> RollExplodingDice(int numberOfDice, int numberOfSides = 6, int explodeMinimum = 6)
        {
            var r = this.dice.Roll("1d6", this.roller);

            Debug.WriteLine($"RollExplodingDice: {r.Value}");

            List<int> result = r.Results.Select(d => d.Value).ToList();

            return result;
        }

        public List<int> RollStandardDice(int numberOfDice)
        {
            throw new NotImplementedException();
        }

        public int RollFTSuccesses(int numberOfDice, int drm = 0, int difficultyLevel = 0)
        {
            if (numberOfDice == 0)
            {
                return 0; // No successes possible if no dice rolled!
            }

            var successes = 0;
            var diceRolled = this.dice.Roll($"{numberOfDice}D6", this.roller);
            Console.WriteLine($"DiceRoller library FTSuccesses die roll [{diceRolled.DiceExpression}]: {string.Join(",", diceRolled.Results.Select(r => r.Value))}");

            foreach (var roll in diceRolled.Results)
            {
                successes += CountSuccessesOnRoll(roll.Value, drm, difficultyLevel);
            }

            return successes;
        }

        public DamageResult RollFTDamage(int numberOfDice, int drm = 0, int targetScreenRating = 0, bool dealPenetrating = false, int recursionDepth = 0)
        {
            var indent = string.Empty.PadRight(4 * recursionDepth);
            var penetrationCount = 0;
            var damageResult = new DamageResult();

            var diceRolled = this.dice.Roll($"{numberOfDice}D6", this.roller);
            var rolls = diceRolled.Results.OrderByDescending(v => v.Value);

            Console.Write($"{indent}DiceRoller library RollFTDamage die roll [{diceRolled.DiceExpression}]");
            Console.Write($" with DRM {drm}, screen {targetScreenRating}, penetration {dealPenetrating}");
            Console.WriteLine($": {string.Join(",", rolls.Select(r => r.Value))}");

            foreach (var roll in rolls)
            {
                var dieDamage = CountSuccessesOnRoll(roll.Value, drm, targetScreenRating);
                damageResult.StandardRolls.Add(roll.Value);

                if (dieDamage > 0)
                {
                    Console.Write($"{indent}Roll of {roll.Value} deals {dieDamage} damage");

                    damageResult.Standard += dieDamage;

                    if (roll.Value >= Constants.MinRollForDoubleSuccess && dealPenetrating)
                    {
                        penetrationCount++;
                    }

                    Console.WriteLine();
                }

                // Exit early if dieDamage == 0, since dice are sorted in descending order?
            }

            if (penetrationCount > 0)
            {
                Console.WriteLine($"{indent}Rolling {penetrationCount} penetrating dice: [");

                // On a "natural" max roll, deal recursive, shield-ignoring, DRM-ignoring, penetrating followup damage
                var penetrationFollowup = this.RollFTDamage(penetrationCount, 0, 0, true, recursionDepth + 1);
                Console.WriteLine($"{indent}]");
                damageResult.Penetrating += penetrationFollowup.Standard;

                // Penetrating damage could ITSELF penetrate, but it all rolls up to just one count of penetrating damage
                damageResult.Penetrating += penetrationFollowup.Penetrating;

                damageResult.PenetratingRolls.AddRange(penetrationFollowup.StandardRolls);
                damageResult.PenetratingRolls.AddRange(penetrationFollowup.PenetratingRolls);
            }

            return damageResult;
        }

        public int RollSuccesses(int numberOfDice, int dRM = 0)
        {
            throw new NotImplementedException();
        }

        public List<int> RollXDY(int numberOfDice, int numberOfSides)
        {
            throw new NotImplementedException();
        }

        private static int CountSuccessesOnRoll(int value, int drm, int difficultyLevel)
        {
            int successes = 0;

            // After DRM, 4+ counts for 1 success,
            // except in cases of "screen level 1" or equivalent "ignore 4's" situations
            if (value + drm >= Constants.MinRollForSuccess + (difficultyLevel > 0 ? 1 : 0))
            {
                successes += 1;
            }

            // Natural 6 counts for 2 successes, unless DRM causes it to fail
            // except in "screen level 2" or equivalent "ignore double-success on 6's" situations
            if (value >= Constants.MinRollForDoubleSuccess && value + drm >= Constants.MinRollForSuccess && difficultyLevel < 2)
            {
                successes += 1;
            }

            return successes;
        }
    }
}