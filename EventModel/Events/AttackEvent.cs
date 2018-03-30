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
            this.TargetingData.SourceFormationUnit = sourceUnit;
            this.TargetingData.TargetFormationUnit = targetUnit;
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

        public AttackEvent(FireOrder fo, GameFormationActor source, int percentileRoll = PercentileNotRolled, int exchange = 0, int volley = 0)
           : base("Attack Event", exchange, volley)
        {
            this.Description = "Attack Event";
            this.TargetingData = new TargetingData(source, fo.TargetID, fo.TargetFormationName, fo.Priority, fo.DiceAssigned, fo.FireType);
            this.UnitAssignmentPercentile = percentileRoll;

            // Just to avoid any confusion
            this.TargetingData.TargetUnitPercentileRoll = percentileRoll;
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, GameFormationActor target, int exchange = 0, int volley = 0, int percentileRoll = PercentileNotRolled)
               : base("Attack Event", exchange, volley)
        {
            this.TargetingData = new TargetingData(source, target, fo.Priority, fo.DiceAssigned, fo.FireType);
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