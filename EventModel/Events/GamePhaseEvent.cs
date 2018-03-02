// <copyright file="GamePhaseEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.GameModel;

    public abstract class GamePhaseEvent : GameEvent
    {
        public GamePhaseEvent()
            : base("Abstract Game Phase Start Event")
        {
            // No-op
        }

        public Constants.GamePhase GamePhase { get; set; }
    }
}
