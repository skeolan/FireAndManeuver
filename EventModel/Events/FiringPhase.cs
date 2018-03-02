// <copyright file="FiringPhase.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.GameModel;

    public class FiringPhase : GamePhaseEvent
    {
        public FiringPhase()
            : base()
        {
            this.GamePhase = Constants.GamePhase.FiringPhase;
            this.Description = "Firing Phase Begun";
        }
    }
}
