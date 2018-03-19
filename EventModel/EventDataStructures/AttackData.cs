// <copyright file="AttackData.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;
    using FireAndManeuver.GameModel;

    public class AttackData
    {
        public AttackData(WeaponSystem weapon, GameUnitFireAllocation allocation, AttackSpecialProperties specialProperties = AttackSpecialProperties.None)
        {
            this.DamageType = weapon.GetDamageType();
            this.AttackProperties = weapon.GetAttackProperties() & specialProperties;
            this.DieRolls = new List<int>();
            this.TrackRating = weapon.GetTrackRating();
        }

        public Constants.DamageType DamageType { get; private set; }

        public AttackSpecialProperties AttackProperties { get; private set; }

        public List<int> DieRolls { get; private set; }

        public TrackRating TrackRating { get; private set; }
    }
}
