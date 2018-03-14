// <copyright file="EventActorBase.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using FireAndManeuver.Common;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public abstract class EventActorBase : IEventActor
    {
        public EventActorBase(IServiceProvider services)
        {
            this.DiceUtility = services.GetRequiredService<IDiceUtility>();

            this.Logger = services.GetLogger();
        }

        public int EventReceivedCount { get; protected set; } = 0;

        protected IDiceUtility DiceUtility { get; set; } = null;

        protected ILogger Logger { get; set; }

        protected List<GameEvent> FinalResult { get; set; } = new List<GameEvent>();

        /* Other than special "catch-all" actors, this should NOT be overridden in subclasses: all event routing should happen consistently here */
        public virtual IList<GameEvent> ReceiveEvent(GameEvent evt)
        {
            this.EventReceivedCount++;

            this.FinalResult.Clear(); // Don't let events hang around between returns

            var functionLookup = new Dictionary<Type, Func<GameEvent, IList<GameEvent>>>()
            {
                { typeof(AttackEvent), this.ReceiveAttackEvent },
                { typeof(DamageEvent), this.ReceiveDamageEvent },
                { typeof(FiringPhaseEvent), this.ReceiveFiringPhaseEvent },
                { typeof(FormationDestroyedEvent), this.ReceiveFormationDestroyedEvent },
                { typeof(FormationStatusEvent), this.ReceiveFormationStatusEvent },
                { typeof(GameEvent), this.ReceiveGameEvent },
                { typeof(UnitDamagedEvent), this.ReceiveUnitDamagedEvent },
                { typeof(UnitDestroyedEvent), this.ReceiveUnitDestroyedEvent },
                { typeof(UnitDisabledEvent), this.ReceiveUnitDisabledEvent },
                { typeof(UnitStatusEvent), this.ReceiveUnitStatusEvent },
                { typeof(UnitSystemDestroyedEvent), this.ReceiveUnitSystemDestroyedEvent },
                { typeof(UnitThrustDecreasedEvent), this.ReceiveUnitThrustDecreasedEvent },
                { typeof(GamePhaseEvent), this.ReceiveGamePhaseEvent },
                { typeof(WeaponAttackEvent), this.ReceiveWeaponAttackEvent }
            };

            // May need to throw an exception on null events?
            if (evt != null)
            {
                IList<GameEvent> newEvents;

                if (functionLookup.ContainsKey(evt.GetType()))
                {
                    newEvents = functionLookup[evt.GetType()].Invoke(evt);
                }
                else
                {
                    // Use generic receiver if there is no type-specific receiver defined.
                    newEvents = functionLookup[typeof(GameEvent)].Invoke(evt);
                }

                newEvents.AddTo(this.FinalResult);
            }

            return this.FinalResult;
        }

        protected static ArgumentException ReceiverArgumentMismatch(string argName, Type argType, string methodName, Type expectedType)
        {
            throw new ArgumentException($"{argName} ({argType.Name}) is not a type that method {methodName} can receive: expected {expectedType.Name}");
        }

        /*
         * Following virtual methods are all no-ops (apart from a simple type-check).
         *   Individual IEventActor classes should override the receivers for those
         *   events that they actually need to react to.
         */

        protected virtual IList<GameEvent> ReceiveGameEvent(GameEvent arg)
        {
            var evt = arg as GameEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(GameEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitThrustDecreasedEvent(GameEvent arg)
        {
            var evt = arg as UnitThrustDecreasedEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitThrustDecreasedEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitSystemDestroyedEvent(GameEvent arg)
        {
            var evt = arg as UnitSystemDestroyedEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitSystemDestroyedEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitStatusEvent(GameEvent arg)
        {
            var evt = arg as UnitStatusEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitStatusEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitDisabledEvent(GameEvent arg)
        {
            var evt = arg as UnitDisabledEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitDisabledEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitDestroyedEvent(GameEvent arg)
        {
            var evt = arg as UnitDestroyedEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitDestroyedEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveUnitDamagedEvent(GameEvent arg)
        {
            var evt = arg as UnitDamagedEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(UnitDamagedEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveFormationStatusEvent(GameEvent arg)
        {
            var evt = arg as FormationStatusEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(FormationStatusEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveFormationDestroyedEvent(GameEvent arg)
        {
            var evt = arg as FormationDestroyedEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(FormationDestroyedEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            var evt = arg as FiringPhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(FiringPhaseEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveDamageEvent(GameEvent arg)
        {
            var evt = arg as DamageEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(DamageEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveAttackEvent(GameEvent arg)
        {
            var evt = arg as AttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(AttackEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveWeaponAttackEvent(GameEvent arg)
        {
            var evt = arg as WeaponAttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(WeaponAttackEvent));

            return null;
        }

        protected virtual IList<GameEvent> ReceiveGamePhaseEvent(GameEvent arg)
        {
            GamePhaseEvent evt = arg as GamePhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(GamePhaseEvent));

            return null;
        }
    }
}
