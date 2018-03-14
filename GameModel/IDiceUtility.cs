// <copyright file="IDiceUtility.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Collections.Generic;

    public interface IDiceUtility
    {
        /// <summary>
        /// Rolls a single D6 and returns the result.
        /// </summary>
        /// <returns>Value of the rolled D6.</returns>
        int RollD6();

        /// <summary>
        /// Rolls X dice with Y sides each.
        /// </summary>
        /// <param name="numberOfDice">Number of dice to be rolled.</param>
        /// <param name="numberOfSides">Number of sides on each die.</param>
        /// <returns>List of integer die-roll values.</returns>
        List<int> RollXDY(int numberOfDice, int numberOfSides);

        /// <summary>
        /// Rolls a specified number of non-exploding D6s and returns a list containing the results of each die.
        /// </summary>
        /// <param name="numberOfDice">Number of D6s to be rolled.</param>
        /// <returns>List of integer die-roll values.</returns>
        List<int> RollStandardDice(int numberOfDice);

        /// <summary>
        /// Accommodates cases where dice "explode" when above a certain value, resulting in rerolls.
        ///   Rerolls are always the LAST values in the result set, so the caller can easily differentiate
        ///   reroll results from the values on the "original" dice.
        /// </summary>
        /// <param name="numberOfDice">Number of dice to be rolled.</param>
        /// <param name="numberOfSides">Number of sides on each die.</param>
        /// <param name="explodeMinimum">Value which must be rolled in order to trigger an additional roll.</param>
        /// <returns>List of integer die-roll results</returns>
        List<int> RollExplodingDice(int numberOfDice, int numberOfSides = 6, int explodeMinimum = 6);

        int RollPercentile();

        int RollSuccesses(int numberOfDice, int dRM = 0);

        int RollFTSuccesses(int numberOfDice, out List<int> rolls, int dRM = 0, int difficultyLevel = 0);
    }
}