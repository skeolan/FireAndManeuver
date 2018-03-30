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
            if (evt.TargetingData == null)
            {
                this.Logger.LogInformation($"[{this.UnitId}]{this.UnitName} unable to act on AttackEvent with incomplete TargetingData.");
                return null;
            }

            if (evt.TargetingData.SourceId != this.unitFormationInfo.FormationId)
            {
                this.Logger.LogInformation($"[{this.UnitId}]{this.UnitName} is not part of this AttackEvent's SourceFormation.");
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
                    if (allocatedFireCon.Status != UnitSystem.StatusOperational)
                    {
                        continue;
                    }
                }

                // TODO: Multiple fire allocations with the same FireCon in the same Volley should be illegal.
                TargetingData newTargeting = TargetingData.Clone(evt.TargetingData);
                newTargeting.SourceFormationUnit = this;

                newTargeting.TargetUnitPercentileRoll = this.DiceUtility.RollPercentile();

                foreach (var weaponId in allocation.WeaponIDs)
                {
                    var weapon = unit.Weapons.Where(w => w.Id == weaponId).FirstOrDefault();
                    AttackData attackData = new AttackData(weapon, allocation);
                    WeaponAttackEvent weaponAttack = new WeaponAttackEvent(newTargeting, attackData, percentileRoll: newTargeting.TargetUnitPercentileRoll, exchange: evt.Exchange, volley: evt.Volley);
                    result.Add(weaponAttack);
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
            if (evt.TargetingData.TargetId == this.unitFormationInfo.FormationId
                && this.unitFormationInfo.CoversPercentile(evt.UnitAssignmentPercentile))
            {
                GameFormation formation = this.unitFormationInfo.GetFormationReference();
                GameUnit unit = this.unitFormationInfo.GetUnitReference();

                result.Add(new UnitStatusEvent()
                    {
                        Description = $"[{formation.FormationId}]{formation.FormationName}:[{unit.IdNumeric}]{unit.Name} covers percentile range {this.unitFormationInfo.PercentileLowerBound}-{this.unitFormationInfo.PercentileUpperBound}, so incoming attack with percentile roll {evt.UnitAssignmentPercentile} targets it.",
                        Exchange = evt.Exchange,
                        Volley = evt.Volley
                    });
            }

            return result;
        }
        }
    }