// <copyright file="ManeuverSuccessSet.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    public class ManeuverSuccessSet
    {
        public ManeuverSuccessSet()
        {
        }

        public int FormationId { get; set; }

        public int Volley { get; set; }

        public int SpeedSuccesses { get; set; }

        public int EvasionSuccesses { get; set; }
    }
}