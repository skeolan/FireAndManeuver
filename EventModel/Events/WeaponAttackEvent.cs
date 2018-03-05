// <copyright file="WeaponAttackEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;

    public class WeaponAttackEvent : AttackEvent
    {
        public WeaponAttackEvent(TargetingData targetingData, AttackData attackData)
            : base(targetingData)
        {
            this.Description = "Weapon Attack Event";
            this.AttackData = attackData;
        }

        public AttackData AttackData { get; set; }
    }
}