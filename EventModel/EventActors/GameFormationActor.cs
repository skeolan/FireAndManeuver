// <copyright file="GameFormationActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FireAndManeuver.GameModel;

    public class GameFormationActor : EventActorBase, IEventActor
    {
        private GameFormation formation;
        private int formationId;
        private string formationName;

        public GameFormationActor(GameFormation f)
        {
            this.formation = f;
            this.formationId = f.FormationId;
            this.formationName = f.FormationName;
        }

        public int GetFormationId()
        {
            return this.formationId;
        }

        public string GetFormationName()
        {
            return this.formationName;
        }

        /* Specific event handler overrides go here */
        public GameFormation GetFormation()
        {
            return this.formation;
        }

        protected override IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            var evt = arg as FiringPhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(GameEvent));

            var fireOrders = this.formation.Orders.Where(o => o.Volley == evt.Volley).Select(o => o.FiringOrders).FirstOrDefault() ?? new List<FireOrder>();

            var result = new List<GameEvent>
            {
                new FormationStatusEvent(
                    $"FormationActor for [{this.formation.FormationId}]{this.formation.FormationName}" +
                    $" received {arg.GetType().Name} - E{arg.Exchange}V{arg.Volley}:" +
                    $" found {fireOrders.Count} FireOrders for current volley.",
                    arg.Exchange,
                    arg.Volley,
                    this.formationId,
                    this.formationName)
            };

            IEnumerable<AttackEvent> attackOrders = fireOrders.Select(fo => new AttackEvent(fo, this, arg.Exchange, arg.Volley));

            result.AddRange(attackOrders);

            return result;
        }
    }
}