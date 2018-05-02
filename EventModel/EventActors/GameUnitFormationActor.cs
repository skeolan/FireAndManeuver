// <copyright file="GameUnitFormationActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FireAndManeuver.GameModel;
    using FireAndManeuver.GameModel.GameMechanics;
    using Microsoft.Extensions.Logging;

    public class GameUnitFormationActor : EventActorBase, IEventActor
    {
        private GameUnitFormationInfo unitFormationInfo;

        public GameUnitFormationActor(GameUnitFormationInfo unitFormationInfo, IServiceProvider services)
            : base(services)
        {
            this.unitFormationInfo = unitFormationInfo ?? new GameUnitFormationInfo();
            this.UnitId = this.unitFormationInfo.UnitId;
            this.UnitName = this.unitFormationInfo.UnitName;
        }

        public int UnitId { get; internal set; }

        public string UnitName { get; internal set; }

        protected override IList<GameEvent> ReceiveAttackEvent(GameEvent arg)
        {
            var result = new List<GameEvent>();

            var evt = arg as AttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(AttackEvent));

            // Unit can act if:
            // a) Attack is assigned to this Unit's Formation
            // b) Attack has a valid reference to a target Formation
            if (evt.TargetingData == null || evt.TargetingData.Source.FormationId != this.unitFormationInfo.FormationId)
            {
                return null;
            }

            var unit = this.unitFormationInfo.GetUnitReference();
            var applicableWeaponAllocations = new List<GameUnitFireAllocation>();
            foreach (var allocation in unit.FireAllocation.Where(a => a.Volley == evt.Volley))
            {
                if (allocation.Priority.ToLowerInvariant() == evt.TargetingData.FirePriority.ToLowerInvariant())
                {
                    if ((allocation.FireMode ?? TargetingData.DefaultFireType).ToLowerInvariant() == evt.TargetingData.FireType.ToLowerInvariant())
                    {
                        applicableWeaponAllocations.Add(allocation);
                    }
                }
            }

            foreach (var allocation in applicableWeaponAllocations)
            {
                if (allocation.FireConId != 0)
                {
                    var allocatedFireCon = unit.Electronics.Where(s => s.Id == allocation.FireConId).FirstOrDefault()
                        ?? throw new InvalidOperationException("FireAllocation instance has an invalid System ID assigned for its Fire Control");

                    if (allocatedFireCon.Status != UnitSystemStatus.Operational)
                    {
                        var statusMsg = $"[{this.UnitId}]{this.UnitName} fire allocation {allocation.ToString()} has no effect -- "
                                    + $"fire control [{allocatedFireCon.Id}]{allocatedFireCon.SystemName} is {allocatedFireCon.Status.ToString()}";

                        this.Logger.LogInformation(statusMsg);

                        result.Add(new UnitStatusEvent()
                        {
                            Description = $"UnitStatus: [{this.UnitId}]{this.UnitName}",
                            Message = statusMsg,
                            Exchange = evt.Exchange,
                            Volley = evt.Volley
                        });

                        continue;
                    }
                }

                // TODO: Multiple fire allocations with the same FireCon in the same Volley should be illegal.
                TargetingData newTargeting = TargetingData.Clone(evt.TargetingData);
                newTargeting.Target.UnitActor = this;

                newTargeting.TargetUnitPercentileRoll = this.DiceUtility.RollPercentile();

                foreach (var weaponId in allocation.WeaponIDs)
                {
                    var weapon = unit.Weapons.Where(w => w.Id == weaponId).FirstOrDefault();
                    if (weapon.CanFire())
                    {
                        AttackData attackData = new AttackData(weapon, allocation);
                        WeaponAttackEvent weaponAttack = new WeaponAttackEvent(newTargeting, attackData, percentileRoll: newTargeting.TargetUnitPercentileRoll, exchange: evt.Exchange, volley: evt.Volley);
                        result.Add(weaponAttack);
                    }
                }
            }

            return result;
        }

        protected override IList<GameEvent> ReceiveWeaponAttackEvent(GameEvent arg)
        {
            var result = new List<GameEvent>();

            var evt = arg as WeaponAttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(AttackEvent));

            // Unit receives event if:
            // a) WeaponAttack is assigned to this Unit's Formation
            // b) Attack's percentile roll is within this Unit's PercentileRange
            if (evt.TargetingData.Target.FormationId == this.unitFormationInfo.FormationId
                && this.unitFormationInfo.CoversPercentile(evt.UnitAssignmentPercentile))
            {
                var target = evt.TargetingData.Target.UnitActor.unitFormationInfo;
                GameFormation targetFormation = target.GetFormationReference();
                GameUnit targetUnit = target.GetUnitReference();

                GameFormation attackerFormation = this.unitFormationInfo.GetFormationReference();
                GameUnit attackerUnit = this.unitFormationInfo.GetUnitReference();
                int firingRange = evt.TargetingData.FormationRange;

                result.Add(new UnitStatusEvent()
                {
                    Description = $"UnitStatus: [{this.UnitId}]{this.UnitName}",
                    Message = $"[{targetFormation.FormationId}]{targetFormation.FormationName}:[{targetUnit.IdNumeric}]{targetUnit.Name}"
                    + $" covers percentile range {this.unitFormationInfo.PercentileLowerBound}-{this.unitFormationInfo.PercentileUpperBound},"
                    + $" so incoming attack with percentile roll {evt.UnitAssignmentPercentile} targets it.",
                    Exchange = evt.Exchange,
                    Volley = evt.Volley
                });

                WeaponSystem weapon = evt.AttackData.Weapon;

                // TODO: Add ability to override this from AttackData and/or TargetingData?
                Constants.DamageType effectiveDamageType = weapon.GetDamageType();

                int evasionDRM = -1 * targetFormation.GetOrdersForVolley(evt.Volley).EvasionSuccesses;

                int otherDRM = targetFormation.GetDRMVersusWeaponType(weapon.GetType())
                    + targetUnit.GetDRMVersusWeaponType(weapon.GetType());

                var localScreens = targetUnit.GetLocalScreenRating();
                var areaScreens = targetFormation.GetFormationAreaScreenRating();

                ScreenRating totalScreenRating = ScreenRating.Combine(localScreens, areaScreens);

                DamageResult weaponAttackResult = this.ResolveWeaponAttack(weapon, firingRange, totalScreenRating, evasionDRM, otherDRM);

                var statusMsg = $"[{attackerUnit.IdNumeric}]{attackerUnit.Name} fires [{weapon.Id}]{weapon.SystemName} at [{targetUnit.IdNumeric}]{targetUnit.Name}"
                    + $"\n\t\t -- Net modifiers: Range {firingRange}"
                    + $", Screen {weapon.FinalizeScreenValue(totalScreenRating)}"
                    + $", Evasion DRM {weapon.FinalizeEvasionDRM(evasionDRM)}"
                    + $", Other DRM {otherDRM}"
                    + $"\n\t\t -- Rolls {weaponAttackResult.RollString()}"
                    + $"\n\t\t -- Deals {weaponAttackResult.ToString()}";

                result.Add(new UnitStatusEvent()
                {
                    Description = $"UnitStatus: [{this.UnitId}]{this.UnitName}",
                    Message = statusMsg,
                    Exchange = evt.Exchange,
                    Volley = evt.Volley
                });
            }

            return result;
        }

        private DamageResult ResolveWeaponAttack(WeaponSystem weapon, int range, ScreenRating screenRating, int evasion = 0, int otherDRM = 0, Constants.DamageType damageTypeModifier = Constants.DamageType.None, AttackSpecialProperties attackPropertiesModifier = AttackSpecialProperties.None)
        {
            AttackSpecialProperties effectiveAttackProperties = weapon.FinalizeAttackProperties(attackPropertiesModifier);

            // Weapons with a hight TrackRating can ignore Evasion, offsetting up to the full Evasion DRM
            int totalDRM = weapon.FinalizeEvasionDRM(evasion) + otherDRM;

            // Weapons can ignore certain kinds of shields but not others
            int totalScreenRating = weapon.FinalizeScreenValue(screenRating, effectiveAttackProperties);

            Constants.DamageType effectiveDamageType = weapon.FinalizeDamageType(weapon.GetDamageType(), damageTypeModifier, effectiveAttackProperties.HasFlag(AttackSpecialProperties.Overrides_Weapon_DamageType));

            // Roll dice here
            this.Logger.LogInformation($"Attack roll! {weapon.SystemName} -- range {range} | screen {screenRating.ToString()} | net DRM {totalDRM} | {effectiveDamageType.ToString()}-type damage | rating {weapon.Rating}");

            /* "Basic" weapon behavior is to shoot like a non-penetrating beam:
             * 1D per Rating, diminishing with range
             * 1 damage on a 4 unless screened
             * 1 damage on a 5, always
             * 2 damage on a 6, unless double-screened -- then 1
             */
            DamageResult damageMatrix = FullThrustDieRolls.RollFTDamage(this.DiceUtility, weapon.GetAttackDice(), totalDRM, totalScreenRating, effectiveDamageType.HasFlag(Constants.DamageType.Penetrating));

            return damageMatrix;
        }
    }
}