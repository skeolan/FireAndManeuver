// <copyright file="TestDummyPhaseActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class TestDummyPhaseActor : TestDummyActor, IEventActor
    {
        public int GamePhaseEventDetectedCount { get; protected set; } = 0;

        public int WeaponAttackEventDetectedCount { get; private set; } = 0;

        public override List<GameEvent> ProcessEvent(GameEvent evt)
        {
            Console.WriteLine($"(DummyPhaseActor override ProcessEvent implementation)");
            Console.WriteLine($"{this.GetType().Name} detected a {evt.GetType().Name}");

            // Event routing logic -- I do not love this, but I couldn't figure out a better way to do it
            if (evt is GamePhaseEvent)
            {
                return this.ProcessPhaseEvent(evt as GamePhaseEvent);
            }

            if (evt is WeaponAttackEvent)
            {
                return this.ProcessWeaponAttackEvent(evt as WeaponAttackEvent);
            }

            // Default case - this class does not handle this event
            // Give the base-class a chance to handle it instead
            this.Result.AddRange(base.ProcessEvent(evt));
            return this.Result;
        }

        public List<GameEvent> ProcessPhaseEvent(GamePhaseEvent evt)
        {
            if (evt == null)
            {
                throw new NullReferenceException($"{evt.GetType().Name} parameter '{nameof(evt)}' is null, or is not a {evt.GetType().Name}.");
            }

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a GamePhaseEvent");
            this.GamePhaseEventDetectedCount++;

            // ... but otherwise do nothing
            return this.Result;
        }

        public List<GameEvent> ProcessWeaponAttackEvent(WeaponAttackEvent evt)
        {
            if (evt == null)
            {
                throw new NullReferenceException($"{evt.GetType().Name} parameter '{nameof(evt)}' is null, or is not a {evt.GetType().Name}.");
            }

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a WeaponAttackEvent");
            this.WeaponAttackEventDetectedCount++;

            // ... but otherwise do nothing
            return this.Result;
        }
    }
}
