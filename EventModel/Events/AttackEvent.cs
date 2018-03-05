// <copyright file="AttackEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;

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

        public TargetingData TargetingData { get; protected set; }
    }
}