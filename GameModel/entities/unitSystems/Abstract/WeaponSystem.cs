// <copyright file="WeaponSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public abstract class WeaponSystem : UnitSystem
    {
        public WeaponSystem()
            : base()
        {
            this.SystemName = "Abstract Weapon System";
        }

        public List<int> Attack(GameUnit target, GameUnit attacker = null)
        {
            List<int> damageMatrix = new List<int>() { 0 };

            return damageMatrix;
        }
    }
}