// <copyright file="FiringPhaseEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.GameModel;

    public class FiringPhaseEvent : GamePhaseEvent
    {
        public FiringPhaseEvent(int volley, int exchange)
            : base(volley, exchange)
        {
            this.GamePhase = Constants.GamePhase.FiringPhase;
            this.Description = $"Firing Phase Begun for E{exchange} V{volley}";
        }
    }
}
