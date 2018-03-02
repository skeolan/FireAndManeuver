// <copyright file="TestDummyActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// An IEventActor that never responds to events
    /// </summary>
    public class TestDummyActor : IEventActor
    {
        public TestDummyActor()
        {
            this.Result = new List<GameEvent>();
        }

        public int EventDetectedCount { get; protected set; } = 0;

        protected List<GameEvent> Result { get; set; }

        public virtual List<GameEvent> ProcessEvent(GameEvent evt)
        {
            Console.WriteLine($"(Virtual ProcessEvent implementation)");
            Console.WriteLine($"{this.GetType().Name} detected a {evt.GetType().Name}");
            this.EventDetectedCount++;
            return this.Result;
        }
    }
}
