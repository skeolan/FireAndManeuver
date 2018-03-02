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

        public override List<GameEvent> ProcessEvent(GameEvent evt)
        {
            this.Result.AddRange(base.ProcessEvent(evt));

            this.TestDummyActorEventReceivedCount++;

            return this.Result;
        }
    }
}
