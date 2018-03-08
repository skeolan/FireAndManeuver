// <copyright file="GameUnitFormationActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    public class GameUnitFormationActor : EventActorBase, IEventActor
    {
        public string UnitId { get; internal set; }

        public string UnitName { get; internal set; }
    }
}