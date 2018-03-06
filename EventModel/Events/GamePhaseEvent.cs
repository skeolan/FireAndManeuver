// <copyright file="GamePhaseEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.GameModel;

    public abstract class GamePhaseEvent : GameEvent
    {
        public GamePhaseEvent(int volley, int exchange)
            : base("Abstract Game Phase Start Event")
        {
            this.Volley = volley;
            this.Exchange = exchange;
        }

        public Constants.GamePhase GamePhase { get; set; }

        public int Exchange { get; private set; } = 0;

        public int Volley { get; private set; } = 0;
    }
}
