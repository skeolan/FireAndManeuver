// <copyright file="TestDummyPhaseActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.EventModel.EventActors;

    public class TestDummyPhaseActor : TestDummyActor, IEventActor
    {
        public int GamePhaseEventDetectedCount { get; protected set; } = 0;

        public int WeaponAttackEventDetectedCount { get; private set; } = 0;

        protected override void ProcessGamePhaseEvent(GamePhaseEvent evt)
        {
            if (evt == null)
            {
                throw new NullReferenceException($"{evt.GetType().Name} parameter '{nameof(evt)}' is null, or is not a {evt.GetType().Name}.");
            }

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a GamePhaseEvent");
            this.GamePhaseEventDetectedCount++;

            // ... but otherwise do nothing
        }

        protected override void ProcessWeaponAttackEvent(WeaponAttackEvent evt)
        {
            if (evt == null)
            {
                throw new NullReferenceException($"{evt.GetType().Name} parameter '{nameof(evt)}' is null, or is not a {evt.GetType().Name}.");
            }

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a WeaponAttackEvent");
            this.WeaponAttackEventDetectedCount++;

            // ... but otherwise do nothing
        }
    }
}
