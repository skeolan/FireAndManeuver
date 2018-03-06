// <copyright file="TestDummyActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.EventModel.EventActors;

    /// <summary>
    /// An IEventActor that never responds to events
    /// </summary>
    public class TestDummyActor : EventActorBase, IEventActor
    {
        public int TestDummyActorEventReceivedCount { get; protected set; } = 0;

        protected override IList<GameEvent> ReceiveGameEvent(GameEvent evt)
        {
            var result = base.ReceiveGameEvent(evt);

            this.TestDummyActorEventReceivedCount++;

            return result;
        }
    }
}
