// <copyright file="EventLoggingActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using FireAndManeuver.EventModel;
    using Microsoft.Extensions.Logging;

    public class EventLoggingActor : EventActorBase, IEventActor
    {
        public EventLoggingActor(IServiceProvider services)
            : base(services)
        {
            // No-op
        }

        public override IList<GameEvent> ReceiveEvent(GameEvent arg)
        {
            var argString = arg.ToString();
            var argTypeString = arg.GetType().ToString();
            this.Logger.LogInformation($"LOGGER: '{arg.Description}'{(argString == argTypeString ? string.Empty : $" -- {arg.ToString()}")}");

            return null; // Logger never spawns new events
        }
    }
}