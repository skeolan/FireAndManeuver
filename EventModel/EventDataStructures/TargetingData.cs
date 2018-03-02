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

        public IEventActor Source { get; set; } // Typically a Formation

        public IEventActor Target { get; set; } // Typically a Formation

        public GameUnitFormationInfo SourceFormationUnit { get; set; } = null;

        public GameUnitFormationInfo TargetFormationUnit { get; set; } = null;

        public string TargetingPriority { get; set; } = Constants.DefaultAttackPriority;
    }
}
