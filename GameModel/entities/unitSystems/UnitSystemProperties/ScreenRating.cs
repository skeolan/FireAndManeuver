// <copyright file="ScreenRating.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;

    public class ScreenRating
    {
        public const int MAXSCREENRATING = 2;

        public ScreenRating(int screenCount = 0, bool screenAdvanced = false)
        {
            this.Value = screenCount;
            this.Advanced = screenAdvanced;
        }

        public int Value { get; set; }

        public bool Advanced { get; set; }

        public static ScreenRating Combine(ScreenRating screenA, ScreenRating screenB)
        {
            return new ScreenRating(Math.Min(MAXSCREENRATING, screenA.Value + screenB.Value), screenA.Advanced || screenB.Advanced);
        }

        public override string ToString()
        {
            string ratingVal = "Unscreened";
            string typeVal = string.Empty;

            if (this.Value == 1)
            {
                ratingVal = "Single";
            }

            if (this.Value > 1)
            {
                ratingVal = "Double";
            }

            if (this.Advanced)
            {
                typeVal = "Advanced";
            }
            else if (this.Value > 0)
            {
                typeVal = "Standard";
            }

            return $"{ratingVal} {typeVal}".Trim();
        }
    }
}