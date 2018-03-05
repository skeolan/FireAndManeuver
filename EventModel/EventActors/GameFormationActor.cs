// <copyright file="GameFormationActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using FireAndManeuver.GameModel;

    public class GameFormationActor : EventActorBase, IEventActor
    {
        private GameFormation formation;

        public GameFormationActor(GameFormation f)
        {
            this.formation = f;
        }

        /* Specific event handler overrides go here */
        public GameFormation GetFormation()
        {
            return this.formation;
        }

        protected override IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            Console.WriteLine($"FormationActor for [{this.formation.FormationId}]{this.formation.FormationName} received {arg.GetType()} - {arg.Description}, taking appropriate actions...");
            return base.ReceiveFiringPhaseEvent(arg);
        }
    }
}