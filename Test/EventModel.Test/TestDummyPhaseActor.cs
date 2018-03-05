// <copyright file="TestDummyPhaseActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using FireAndManeuver.EventModel.EventActors;

    public class TestDummyPhaseActor : TestDummyActor, IEventActor
    {
        public int GamePhaseEventDetectedCount { get; protected set; } = 0;

        public int WeaponAttackEventDetectedCount { get; private set; } = 0;

        protected override IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            return this.ReceiveGamePhaseEvent(arg);
        }

        protected override IList<GameEvent> ReceiveGamePhaseEvent(GameEvent arg)
        {
            var evt = arg as GamePhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(GamePhaseEvent));

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a GamePhaseEvent");
            this.GamePhaseEventDetectedCount++;

            // ... but otherwise do nothing
            return this.Result;
        }

        protected override IList<GameEvent> ReceiveWeaponAttackEvent(GameEvent arg)
        {
            var evt = arg as WeaponAttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(WeaponAttackEvent));

            // Aha! Non-base event resolution!
            Console.WriteLine($"{this.GetType().Name} handled a {evt.GetType().Name} as a WeaponAttackEvent");
            this.WeaponAttackEventDetectedCount++;

            // ... but otherwise do nothing
            return this.Result;
        }
    }
}
