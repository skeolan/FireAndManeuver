// <copyright file="FullThrustDieRolls.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel.GameMechanics
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    public static class FullThrustDieRolls
    {
        public static DamageResult RollFTDamage(IDiceUtility roller, int numberOfDice, int drm = 0, int targetScreenRating = 0, bool dealPenetrating = false, int recursionDepth = 0)
        {
            var indent = string.Empty.PadRight(4 * recursionDepth);
            var penetrationCount = 0;
            var damageResult = new DamageResult();

            var diceRolled = roller.RollStandardDice(numberOfDice);
            var rolls = diceRolled.ToList().OrderByDescending(v => v);

            Console.Write($"{indent}[Rolled {numberOfDice}] dice");
            Console.Write($" with DRM {drm}, screen {targetScreenRating}{(dealPenetrating ? ", penetrating" : ", non-penetrating")}");
            Console.WriteLine($": {string.Join(",", rolls)}");

            foreach (var roll in rolls)
            {
                var dieDamage = CountSuccessesOnRoll(roll, drm, targetScreenRating);
                damageResult.StandardRolls.Add(roll);

                if (dieDamage > 0)
                {
                    Console.Write($"{indent}Roll of {roll} deals {dieDamage} damage");

                    damageResult.Standard += dieDamage;

                    if (roll >= Constants.MinRollForDoubleSuccess && dealPenetrating)
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
                var penetrationFollowup = FullThrustDieRolls.RollFTDamage(roller, penetrationCount, 0, 0, true, recursionDepth + 1);
                Console.WriteLine($"{indent}]");
                damageResult.Penetrating += penetrationFollowup.Standard;

                // Penetrating damage could ITSELF penetrate, but it all rolls up to just one count of penetrating damage
                // TODO: revisit this; what about the case of multiple layers of armor + multiple penetrating recursions?
                damageResult.Penetrating += penetrationFollowup.Penetrating;

                damageResult.PenetratingRolls.AddRange(penetrationFollowup.StandardRolls);
                damageResult.PenetratingRolls.AddRange(penetrationFollowup.PenetratingRolls);
            }

            return damageResult;
        }

        public static int RollFTSuccesses(IDiceUtility roller, int numberOfDice, out IEnumerable<int> rolls, int drm = 0, int difficultyLevel = 0)
        {
            if (numberOfDice == 0)
            {
                rolls = new List<int>();
                return 0; // No successes possible if no dice rolled!
            }

            int successes = 0;
            string expression = $"{numberOfDice}D6";
            rolls = roller.RollStandardDice(numberOfDice).OrderByDescending(i => i);

            foreach (int roll in rolls)
            {
                successes += CountSuccessesOnRoll(roll, drm, difficultyLevel);
            }

            return successes;
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
