// <copyright file="EventActorBase.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public abstract class EventActorBase : IEventActor
    {
        public int EventReceivedCount { get; protected set; } = 0;

        protected List<GameEvent> Result { get; set; } = new List<GameEvent>();

        public virtual List<GameEvent> ProcessEvent(GameEvent evt)
        {
            this.EventReceivedCount++;

            if (evt is GamePhaseEvent)
            {
                this.ProcessGamePhaseEvent(evt as GamePhaseEvent);
            }

            if (evt is WeaponAttackEvent)
            {
                this.ProcessWeaponAttackEvent(evt as WeaponAttackEvent);
            }

            return this.Result;
        }

        protected virtual void ProcessWeaponAttackEvent(WeaponAttackEvent weaponAttackEvent)
        {
            // no-op
        }

        protected virtual void ProcessGamePhaseEvent(GamePhaseEvent evt)
        {
            // no-op
        }
    }
}
