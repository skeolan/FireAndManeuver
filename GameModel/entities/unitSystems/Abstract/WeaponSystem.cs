// <copyright file="WeaponSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public abstract class WeaponSystem : UnitSystem
    {
        public WeaponSystem()
            : base()
        {
            this.SystemName = "Abstract Weapon System";
        }

        [XmlAttribute("rating")]
        public int Rating { get; set; } = 1;

        public virtual Constants.DamageType GetDamageType()
        {
            return Constants.DamageType.Standard;
        }

        public int GetAttackDice()
        {
            return this.Rating;
        }

        public virtual AttackSpecialProperties GetAttackProperties()
        {
            return AttackSpecialProperties.None;
        }

        public virtual TrackRating GetTrackRating()
        {
            // Since the abstract default is a "non-arc" weapon, treat as if it fires into all arcs.
            return TrackRating.FiveOrSixArc;
        }

        public virtual bool CanFire()
        {
            return this.Status == UnitSystemStatus.Operational;
        }

        public virtual AttackSpecialProperties FinalizeAttackProperties(AttackSpecialProperties attackPropertiesModifier = AttackSpecialProperties.None)
        {
            if (attackPropertiesModifier.HasFlag(AttackSpecialProperties.Overrides_Weapon_Properties))
            {
                return attackPropertiesModifier;
            }
            else
            {
                return this.GetAttackProperties() & attackPropertiesModifier;
            }
        }

        public virtual Constants.DamageType FinalizeDamageType(Constants.DamageType weaponDamageType, Constants.DamageType damageTypeModifier = Constants.DamageType.None, bool modifierOverridesWeaponDamageType = false)
        {
            if (modifierOverridesWeaponDamageType)
            {
                return damageTypeModifier;
            }
            else
            {
                return weaponDamageType | damageTypeModifier;
            }
        }

        public virtual int FinalizeScreenValue(ScreenRating screen, AttackSpecialProperties effectiveAttackProperties = AttackSpecialProperties.None)
        {
            if (effectiveAttackProperties.HasFlag(AttackSpecialProperties.Ignores_Advanced_Screens))
            {
                return 0; // Ignores any screens
            }

            if (!screen.Advanced && effectiveAttackProperties.HasFlag(AttackSpecialProperties.Ignores_Standard_Screens))
            {
                return 0; // Ignores standard screens
            }

            // else
            return screen.Value;
        }

        public virtual int FinalizeEvasionDRM(int evasionDRM)
        {
            return Math.Min(0, evasionDRM + (int)this.GetTrackRating());
        }
    }
}