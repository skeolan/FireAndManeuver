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
            this.FinalResult.Clear();

            var evt = arg as AttackEvent ??
                throw ReceiverArgumentMismatch(nameof(arg), arg.GetType(), MethodBase.GetCurrentMethod().Name, typeof(AttackEvent));

            // If Attack is assigned to this Unit's Formation, but not to another Unit
            if (evt.TargetingData != null && evt.TargetingData.SourceId == this.unitFormationInfo.FormationId && evt.TargetingData.SourceFormationUnit == null)
            {
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
                        if (allocatedFireCon.Status != UnitSystem.StatusOperational)
                        {
                            continue;
                        }
                    }

                    // TODO: Multiple fire allocations with the same FireCon in the same Volley should be illegal.
                    TargetingData newTargeting = TargetingData.Clone(evt.TargetingData);
                    newTargeting.TargetFormationUnitId = evt.TargetingData.Target.GetUnitByPercentile(this.DiceUtility.RollPercentile()).UnitId;

                    foreach (var weaponId in allocation.WeaponIDs)
                    {
                        var weapon = unit.Weapons.Where(w => w.Id == weaponId).FirstOrDefault();
                        AttackData attackData = new AttackData(weapon, allocation);
                        WeaponAttackEvent weaponAttack = new WeaponAttackEvent(newTargeting, attackData);
                        this.FinalResult.Add(weaponAttack);
                    }
                }
            }

            return this.FinalResult;
        }

        // TODO: Add a handler for the new "WeaponAttackEvent" that applies the weapon attack to the targeted Unit
    }
}