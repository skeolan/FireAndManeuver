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

        public TargetingData(GameFormationActor source, FormationDistance distance, string priority, int diceAssigned, string fireType)
        {
            this.FireDiceAssigned = diceAssigned;
            this.FirePriority = priority;
            this.FireType = fireType;

            this.Source = new TargetingProfile(source);

            this.Target = new TargetingProfile(distance.TargetFormationId, distance.TargetFormationName);

            this.FormationRange = distance.Value;
        }

        public TargetingData(GameFormationActor source, GameFormationActor target, FormationDistance distance, string priority, int diceAssigned, string fireType)
            : this(source, distance, priority, diceAssigned, fireType)
        {
            this.Target.FormationActor = target;
        }

        public int FireDiceAssigned { get; internal set; } = 0;

        public string FirePriority { get; internal set; } = Constants.DefaultAttackPriority;

        public string FireType { get; internal set; } = TargetingData.DefaultFireType;

        public TargetingProfile Source { get; set; }

        public TargetingProfile Target { get; set; }

        public int TargetUnitPercentileRoll { get; internal set; } = AttackEvent.PercentileNotRolled;

        public int FormationRange { get; internal set; } = Constants.DefaultStartingRange;

        public override string ToString()
        {
            string sourceStr = $"[{this.Source.FormationId}]{this.Source.FormationName}";
            string targetStr = $"[{this.Target.FormationId}]{this.Target.FormationName}";
            string diceStr = this.FireDiceAssigned == 0 ? string.Empty : $" ({this.FireDiceAssigned}D)";
            string percentileStr = this.TargetUnitPercentileRoll == -1 ? string.Empty : $" -- Rolled {this.TargetUnitPercentileRoll} to hit";

            if (this.Source.UnitActor != null)
            {
                var sfu = this.Source.UnitActor;
                sourceStr += $":[{sfu.UnitId}]{sfu.UnitName}";
            }

            if (this.Target.UnitActor != null)
            {
                var tfu = this.Target.UnitActor;
                targetStr += $":[{tfu.UnitId}]{tfu.UnitName}";
            }

            return $"Targeting data: [{sourceStr}] -> [{targetStr}] | {this.FirePriority} {this.FireType}{diceStr}{percentileStr}";
        }

        internal static TargetingData Clone(TargetingData td)
        {
            var newTD = (TargetingData)td.MemberwiseClone();

            newTD.Source = TargetingProfile.Clone(td.Source);
            newTD.Target = TargetingProfile.Clone(td.Target);

            return newTD;
        }
    }
}
