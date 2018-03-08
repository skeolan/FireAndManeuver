// <copyright file="TargetingData.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;

    public class TargetingData
    {
        public TargetingData()
        {
            // defaults!
        }

        public TargetingData(GameFormationActor source, int targetId, string targetName, string priority, int diceAssigned, string fireType)
        {
            this.SourceId = source.GetFormationId();
            this.SourceName = source.GetFormationName();
            this.TargetId = targetId;
            this.TargetName = targetName;
            this.Source = source;
            this.TargetingPriority = priority;
            this.DiceAssigned = diceAssigned;
            this.FireType = fireType;

            // Leaves this.Target null since it can't be resolved yet; this is intentional.
        }

        public TargetingData(GameFormationActor source, GameFormationActor target, string priority, int diceAssigned, string fireType)
        {
            this.SourceId = source.GetFormationId();
            this.SourceName = source.GetFormationName();
            this.Source = source;

            this.TargetId = target.GetFormationId();
            this.TargetName = target.GetFormationName();
            this.Target = target;

            this.TargetingPriority = priority;
            this.DiceAssigned = diceAssigned;
            this.FireType = fireType;
        }

        public string TargetingPriority { get; internal set; } = Constants.DefaultAttackPriority;

        public int DiceAssigned { get; internal set; } = 0;

        public string FireType { get; internal set; } = "Fire";

        public int SourceId { get; internal set; } = 0;

        public GameFormationActor Source { get; internal set; } // Typically a Formation

        public string SourceName { get; internal set; } = null;

        public GameUnitFormationActor SourceFormationUnit { get; internal set; } = null;

        public int TargetId { get; internal set; } = 0;

        public GameFormationActor Target { get; internal set; } // Typically a Formation

        public string TargetName { get; internal set; } = null;

        public GameUnitFormationActor TargetFormationUnit { get; internal set; } = null;

        public override string ToString()
        {
            string sourceStr = $"[{this.SourceId}]{this.SourceName}";
            string targetStr = $"[{this.TargetId}]{this.TargetName}";
            string diceStr = this.DiceAssigned == 0 ? string.Empty : $" ({this.DiceAssigned}D)";

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

            return $"Targeting data: [{sourceStr}] -> [{targetStr}] | {this.TargetingPriority} {this.FireType}{diceStr}";
        }
    }
}
