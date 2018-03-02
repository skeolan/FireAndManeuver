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
        public AttackData()
        {
            this.DamageType = Constants.DamageType.Standard;
            this.AttackProperties = Constants.AttackSpecialProperties.None;
            this.DieRolls = new List<int>();
            this.TrackRating = 0;
        }

        public Constants.DamageType DamageType { get; private set; }

        public Constants.AttackSpecialProperties AttackProperties { get; private set; }

        public List<int> DieRolls { get; private set; }

        public int TrackRating { get; private set; }
    }
}
