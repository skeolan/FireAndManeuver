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
        public WeaponAttackEvent(TargetingData targetingData, AttackData attackData, int percentileRoll = AttackEvent.PercentileNotRolled, int exchange = 0, int volley = 0)
            : base(targetingData, percentileRoll: percentileRoll, exchange: exchange, volley: volley)
        {
            this.Description = "Weapon Attack Event";
            this.AttackData = attackData;

            // Just to avoid any confusion
            this.TargetingData.TargetUnitPercentileRoll = percentileRoll;
        }

        public AttackData AttackData { get; set; }

        public override string ToString()
        {
            var baseStr = base.ToString();
            var weaponStr = $"{this.AttackData.Weapon.ToString()}";
            return $"{baseStr} -- {weaponStr}";
        }
    }
}