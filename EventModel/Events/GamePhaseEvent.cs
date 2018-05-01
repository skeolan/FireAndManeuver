// <copyright file="GamePhaseEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.GameModel;

    public abstract class GamePhaseEvent : GameEvent
    {
        public GamePhaseEvent(int volley, int exchange, FormationDistanceGraph distanceGraph)
            : base("Abstract Game Phase Start Event")
        {
            this.Volley = volley;
            this.Exchange = exchange;
            this.DistanceGraph = distanceGraph;
        }

        public FormationDistanceGraph DistanceGraph { get; set; }

        public Constants.GamePhase GamePhase { get; set; }
    }
}
