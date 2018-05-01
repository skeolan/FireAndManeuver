// <copyright file="GameFormationActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Logging;

    public class GameFormationActor : EventActorBase, IEventActor
    {
        private GameFormation formation;
        private int formationId;
        private string formationName;

        public GameFormationActor(GameFormation f, IServiceProvider services)
            : base(services)
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

        public int GetUnitIdByPercentile(int unitAssignmentPercentile)
        {
            StringBuilder logMsg = new StringBuilder();

            decimal percentRemaining = (decimal)unitAssignmentPercentile;

            var targetedUnit = this.formation.Units.FirstOrDefault();

            foreach (var unit in this.formation.Units)
            {
                // This unit "takes up" the next N points of the percentage spread
                // ... which either makes it the target, or accounts for N points of
                // ... the roll.
                decimal hitShare = this.formation.GetHitChancePercentage(unit.UnitId);
                percentRemaining -= hitShare;

                logMsg.Append($"{hitShare:N0} of rolled {unitAssignmentPercentile} goes to [{unit.UnitId}]{unit.UnitName}, {percentRemaining:N0} remaining");

                if (percentRemaining <= 0)
                {
                    logMsg.AppendLine(" -- target found!");
                    targetedUnit = unit;
                    break; // target found, don't continue looping
                }

                logMsg.AppendLine();
            }

            return targetedUnit.UnitId;
        }

        protected override IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            var evt = arg as FiringPhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(FiringPhaseEvent));

            var fireOrders = this.formation.Orders.Where(o => o.Volley == evt.Volley).Select(o => o.FiringOrders).FirstOrDefault() ?? new List<FireOrder>();

            // Don't roll percentile yet, GameUnits roll a percentile for the AttackEvent(s) they produce instead
            IEnumerable<AttackEvent> attackOrders = fireOrders.Where(fo => fo.TargetID != 0).Select(fo => new AttackEvent(fo, this, distanceGraph: evt.DistanceGraph, percentileRoll: AttackEvent.PercentileNotRolled, exchange: evt.Exchange, volley: evt.Volley));

            StringBuilder statusMsg = new StringBuilder();

            statusMsg.AppendLine($"FormationActor for [{this.formation.FormationId}]{this.formation.FormationName}" +
                    $" received {arg.GetType().Name} - E{arg.Exchange}V{arg.Volley}:" +
                    $" found {fireOrders.Count} FireOrders for current volley.");
            foreach (var o in attackOrders)
            {
                statusMsg.AppendLine("\t- " + o.ToString());
            }

            var result = new List<GameEvent>
            {
                new FormationStatusEvent(
                    statusMsg.ToString(),
                    arg.Exchange,
                    arg.Volley,
                    this.formationId,
                    this.formationName)
            };

            result.AddRange(attackOrders);

            return result;
        }
    }
}