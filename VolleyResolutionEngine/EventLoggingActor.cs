// <copyright file="EventLoggingActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameEngine
{
    using System.Collections.Generic;
    using FireAndManeuver.EventModel;
    using FireAndManeuver.EventModel.EventActors;
    using Microsoft.Extensions.Logging;

    internal class EventLoggingActor : EventActorBase, IEventActor
    {
        private ILogger logger;

        public EventLoggingActor(ILogger logger)
        {
            this.logger = logger;
        }

        public override IList<GameEvent> ReceiveEvent(GameEvent arg)
        {
            this.logger.LogInformation($"[{arg.GetType().Name}] -- '{arg.Description}'");

            return null; // Logger never spawns new events
        }
    }
}