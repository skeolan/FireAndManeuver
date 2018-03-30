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
            this.Weapon = weapon;
            this.DamageType = weapon.GetDamageType();
            this.AttackProperties = weapon.GetAttackProperties() & specialProperties;
            this.DieRolls = new List<int>();
            this.TrackRating = weapon.GetTrackRating();
        }

        public WeaponSystem Weapon { get; private set; }

        public Constants.DamageType DamageType { get; private set; }

        public AttackSpecialProperties AttackProperties { get; private set; }

        public List<int> DieRolls { get; private set; }

        public TrackRating TrackRating { get; private set; }

        internal static AttackData Clone(AttackData instance)
        {
            var newInstance = (AttackData)instance.MemberwiseClone();

            newInstance.DieRolls = new List<int>();
            newInstance.DieRolls.AddRange(instance.DieRolls);
            newInstance.Weapon = instance.Weapon.Clone();

            return newInstance;
        }
    }
}
