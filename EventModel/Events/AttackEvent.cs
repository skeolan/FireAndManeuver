// <copyright file="AttackEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;

    public class AttackEvent : GameEvent
    {
        public const int PercentileNotRolled = -1;

        public AttackEvent()
            : base("Attack Event")
        {
            // ...
        }

        public AttackEvent(AttackEvent evt, int percentileRoll = AttackEvent.PercentileNotRolled, GameUnitFormationActor sourceUnit = null, GameUnitFormationActor targetUnit = null)
            : base(evt.Description)
        {
            // Perform a deep copy
            this.Exchange = evt.Exchange;
            this.Volley = evt.Volley;
            this.TargetingData = TargetingData.Clone(evt.TargetingData);
            this.TargetingData.Source.UnitActor = sourceUnit;
            this.TargetingData.Target.UnitActor = targetUnit;
            this.UnitAssignmentPercentile = percentileRoll;

            // Just to avoid any confusion
            this.TargetingData.TargetUnitPercentileRoll = percentileRoll;
        }

        public AttackEvent(TargetingData targetingData, int percentileRoll = AttackEvent.PercentileNotRolled, int exchange = 0, int volley = 0)
            : base("Attack Event", exchange, volley)
        {
            this.TargetingData = TargetingData.Clone(targetingData);
            this.UnitAssignmentPercentile = percentileRoll;

            // Just to avoid any confusion
            this.TargetingData.TargetUnitPercentileRoll = percentileRoll;
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, FormationDistanceGraph distanceGraph, int percentileRoll = PercentileNotRolled, int exchange = 0, int volley = 0)
           : base("Attack Event", exchange, volley)
        {
            var distance = distanceGraph.GetOrEstablishDistance(source.GetFormationId(), fo.TargetID);

            this.Description = "Attack Event";
            this.TargetingData = new TargetingData(source, distance, fo.Priority, fo.DiceAssigned, fo.FireType);
            this.UnitAssignmentPercentile = percentileRoll;

            // Just to avoid any confusion
            this.TargetingData.TargetUnitPercentileRoll = percentileRoll;
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, GameFormationActor target, FormationDistanceGraph distanceGraph, int exchange = 0, int volley = 0, int percentileRoll = PercentileNotRolled)
               : base("Attack Event", exchange, volley)
        {
            var distance = distanceGraph.GetOrEstablishDistance(source.GetFormationId(), target.GetFormationId());

            if (target.GetFormationId() != fo.TargetID)
            {
                throw new InvalidOperationException(
                    $"FireOrder specified target formation [{fo.TargetID}]{fo.TargetFormationName} " +
                    $"but target Formation argument in AttackEvent is [{target.GetFormationId()}]{target.GetFormationName()}");
            }

            this.TargetingData = new TargetingData(source, target, distance, fo.Priority, fo.DiceAssigned, fo.FireType);
            this.UnitAssignmentPercentile = percentileRoll;
        }

        public TargetingData TargetingData { get; protected set; }

        public int UnitAssignmentPercentile { get; protected set; }

        public override string ToString()
        {
            return $"E{this.Exchange} V{this.Volley} -- {this.Description}: {this.TargetingData.ToString().Replace("Targeting data:", string.Empty).Trim()}";
        }
    }
}