// <copyright file="TargetingProfile.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;

    public class TargetingProfile
    {
        public TargetingProfile(int formationId = 0, string formationName = "", int unitId = 0, string unitName = "")
        {
            this.FormationActor = null;
            this.FormationId = formationId;
            this.FormationName = formationName;

            this.UnitActor = null;
            this.UnitId = unitId;
            this.UnitName = unitName;
        }

        public TargetingProfile(GameFormationActor actor, int unitId = 0, string unitName = "")
            : this(actor.GetFormationId(), actor.GetFormationName(), unitId, unitName)
        {
            this.FormationActor = actor;
        }

        public TargetingProfile(GameFormationActor actor, GameUnitFormationActor unitActor)
            : this(actor.GetFormationId(), actor.GetFormationName(), unitActor.UnitId, unitActor.UnitName)
        {
            this.FormationActor = actor;
            this.UnitActor = unitActor;
        }

        public GameFormationActor FormationActor { get; set; }

        public int FormationId { get; set; }

        public string FormationName { get; set; }

        public GameUnitFormationActor UnitActor { get; set; }

        public int UnitId { get; set; }

        public string UnitName { get; set; }

        internal static TargetingProfile Clone(TargetingProfile source)
        {
            TargetingProfile newP = (TargetingProfile)source.MemberwiseClone();

            // Preserve reference properties if present
            newP.FormationActor = source.FormationActor;
            newP.UnitActor = source.UnitActor;

            return newP;
        }
    }
}
