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
        public AttackEvent()
            : base()
        {
            this.Description = "Attack Event";
        }

        public AttackEvent(TargetingData targetingData)
        {
            this.TargetingData = targetingData;
        }

        public AttackEvent(FireOrder fo, GameFormationActor source)
        {
            this.TargetingData = new TargetingData(source, fo.TargetID, fo.TargetFormationName, fo.Priority, fo.DiceAssigned, fo.FireType);
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, GameFormationActor target)
        {
            this.TargetingData = new TargetingData(source, target, fo.Priority, fo.DiceAssigned, fo.FireType);
        }

        public TargetingData TargetingData { get; protected set; }
    }
}