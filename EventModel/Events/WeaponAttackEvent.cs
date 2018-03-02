// <copyright file="WeaponAttackEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;

    public class WeaponAttackEvent : GameEvent
    {
        public WeaponAttackEvent(TargetingData targetingData, AttackData attackData)
            : base("Weapon Attack Event")
        {
            this.TargetingData = targetingData;
            this.AttackData = attackData;
        }

        public TargetingData TargetingData { get; set; }

        public AttackData AttackData { get; set; }
    }
}
