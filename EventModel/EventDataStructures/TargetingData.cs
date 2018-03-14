// <copyright file="TargetingData.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;

    public class TargetingData
    {
        public const string DefaultFireType = "Fire";
        public const int DefaultFireConId = 0;

        public TargetingData()
        {
            // defaults!
        }

        public TargetingData(GameFormationActor source, int targetId, string targetName, string priority, int diceAssigned, string fireType)
        {
            this.FireConId = TargetingData.DefaultFireConId;
            this.FireDiceAssigned = diceAssigned;
            this.FirePriority = priority;
            this.FireType = fireType;
            this.FireWeapons = new List<int>();

            this.Source = source;
            this.SourceFormationUnit = null; // Can't be resolved yet.
            this.SourceId = source.GetFormationId();
            this.SourceName = source.GetFormationName();

            this.Target = null; // Can't be resolved yet.
            this.TargetFormationUnit = null; // Can't be resolved yet.
            this.TargetId = targetId;
            this.TargetName = targetName;
        }

        public TargetingData(GameFormationActor source, GameFormationActor target, string priority, int diceAssigned, string fireType)
        {
            this.FireConId = TargetingData.DefaultFireConId;
            this.FireDiceAssigned = diceAssigned;
            this.FirePriority = priority;
            this.FireType = fireType;
            this.FireWeapons = new List<int>();

            this.Source = source;
            this.SourceFormationUnit = null; // Can't be resolved yet
            this.SourceId = source.GetFormationId();
            this.SourceName = source.GetFormationName();

            this.Target = target;
            this.TargetFormationUnit = null; // Can't be resolved yet
            this.TargetId = target.GetFormationId();
            this.TargetName = target.GetFormationName();
        }

        public int FireConId { get; internal set; } = TargetingData.DefaultFireConId;

        public int FireDiceAssigned { get; internal set; } = 0;

        public string FirePriority { get; internal set; } = Constants.DefaultAttackPriority;

        public string FireType { get; internal set; } = TargetingData.DefaultFireType;

        public List<int> FireWeapons { get; internal set; }

        public GameFormationActor Source { get; internal set; } // Typically a Formation

        public GameUnitFormationActor SourceFormationUnit { get; internal set; } = null;

        public int SourceId { get; internal set; } = 0;

        public string SourceName { get; internal set; } = null;

        public GameFormationActor Target { get; internal set; } // Typically a Formation

        public GameUnitFormationActor TargetFormationUnit { get; internal set; } = null;

        public string TargetName { get; internal set; } = null;

        public int TargetId { get; internal set; } = 0;

        public override string ToString()
        {
            string sourceStr = $"[{this.SourceId}]{this.SourceName}";
            string targetStr = $"[{this.TargetId}]{this.TargetName}";
            string diceStr = this.FireDiceAssigned == 0 ? string.Empty : $" ({this.FireDiceAssigned}D)";

            if (this.SourceFormationUnit != null)
            {
                var sfu = this.SourceFormationUnit as GameUnitFormationActor;
                sourceStr += $":[{sfu.UnitId}]{sfu.UnitName}";
            }

            if (this.TargetFormationUnit != null)
            {
                var tfu = this.TargetFormationUnit as GameUnitFormationActor;
                targetStr += $":[{tfu.UnitId}]{tfu.UnitName}";
            }

            return $"Targeting data: [{sourceStr}] -> [{targetStr}] | {this.FirePriority} {this.FireType}{diceStr}";
        }

        internal static TargetingData Clone(TargetingData td)
        {
            return (TargetingData)td.MemberwiseClone();
        }
    }
}
