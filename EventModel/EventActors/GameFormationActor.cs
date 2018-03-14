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

        protected override IList<GameEvent> ReceiveFiringPhaseEvent(GameEvent arg)
        {
            var evt = arg as FiringPhaseEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(FiringPhaseEvent));

            var fireOrders = this.formation.Orders.Where(o => o.Volley == evt.Volley).Select(o => o.FiringOrders).FirstOrDefault() ?? new List<FireOrder>();

            // Don't roll percentile yet, GameUnits roll a percentile for the AttackEvent(s) they produce instead
            IEnumerable<AttackEvent> attackOrders = fireOrders.Select(fo => new AttackEvent(fo, this, arg.Exchange, arg.Volley, AttackEvent.PercentileNotRolled));

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

        protected override IList<GameEvent> ReceiveAttackEvent(GameEvent arg)
        {
            var evt = arg as AttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(AttackEvent));
            var result = new List<GameEvent>();

            /* Formations respond to an attack event IFF
             * 1. Formation is the target of the event (ID match)
             * 2. Attack event has populated *all of* these fields:
             *    a) a source Formation ID
             *    b) a source Unit ID
             *    c) a percentile roll for Unit targeting
             * ... otherwise, you'll presumably see this event again
             * later, with that information populated.
             */

            TargetingData tD = evt.TargetingData;
            int percentile = evt.UnitAssignmentPercentile;

            if (
                tD.TargetId == this.formationId
                && tD.SourceId != 0
                && tD.SourceFormationUnit != null
                && tD.SourceFormationUnit.UnitId != 0
                && evt.UnitAssignmentPercentile != AttackEvent.PercentileNotRolled)
            {
                // Event has found its target Formation and is ready to execute!
                // Route it to the corresponding Unit by percentile roll
                GameUnitFormationInfo targetUnit = this.GetUnitByPercentile(evt.UnitAssignmentPercentile);

                this.Logger.LogInformation($"AttackEvent received by [{this.formationId}]{this.formationName} " +
                    $"with roll {evt.UnitAssignmentPercentile} assigned to unit [{targetUnit.UnitId}]{targetUnit.UnitName}");

                return null;
            }

            // Else
            return result;
        }

        private GameUnitFormationInfo GetUnitByPercentile(int unitAssignmentPercentile)
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

            return targetedUnit;
        }
    }
}