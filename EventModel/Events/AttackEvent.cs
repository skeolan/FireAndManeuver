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
            : base("Attack Event")
        {
            // ...
        }

        public AttackEvent(TargetingData targetingData, int exchange = 0, int volley = 0)
            : base("Attack Event", exchange, volley)
        {
            this.TargetingData = targetingData;
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, int exchange = 0, int volley = 0)
           : base("Attack Event", exchange, volley)
        {
            this.Description = "Attack Event";
            this.TargetingData = new TargetingData(source, fo.TargetID, fo.TargetFormationName, fo.Priority, fo.DiceAssigned, fo.FireType);
        }

        public AttackEvent(FireOrder fo, GameFormationActor source, GameFormationActor target, int exchange = 0, int volley = 0)
               : base("Attack Event", exchange, volley)
        {
            this.TargetingData = new TargetingData(source, target, fo.Priority, fo.DiceAssigned, fo.FireType);
        }

        public TargetingData TargetingData { get; protected set; }

        public override string ToString()
        {
            return $"E{this.Exchange} V{this.Volley} -- {this.Description}: {this.TargetingData.ToString()}";
        }
    }
}