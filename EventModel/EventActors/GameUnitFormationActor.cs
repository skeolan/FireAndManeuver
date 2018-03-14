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
            if (evt.TargetingData == null || evt.TargetingData.SourceFormationUnit != null)
            {
                // UnitFormationActors pick up only targeted attack orders with no unit assigned yet...
                return null;
            }

            // ... from their parent Formation
            if (evt.TargetingData.SourceId == this.unitFormationInfo.FormationId)
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
                    // TODO: Check for damage to the governing FireCon -- if it's damaged or destroyed, weapons don't fire.
                    // TODO: Get target unit based on percentile roll once per fire allocation.
                    // TODO: Multiple fire allocations with the same FireCon in the same Volley should be illegal.
                    // TODO: Change this into a new Unit-to-Unit "WeaponAttackEvent" type and spawn one per weapon in the fire allocation.
                    AttackEvent newAttack = new AttackEvent(evt, percentileRoll: this.DiceUtility.RollPercentile(), sourceUnit: this);
                    newAttack.TargetingData.FireConId = allocation.FireConId;
                    newAttack.TargetingData.FireWeapons = allocation.WeaponIDs;
                    this.FinalResult.Add(newAttack);
                }
            }

            return this.FinalResult;
        }

        // TODO: Add a handler for the new "WeaponAttackEvent" that applies the weapon attack to the targeted Unit
    }
}