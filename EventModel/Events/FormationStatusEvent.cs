// <copyright file="FormationStatusEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class FormationStatusEvent : GameEvent
    {
        public FormationStatusEvent()
        {
            this.Description = "FormationStatusEvent";
        }

        public FormationStatusEvent(string descr)
        {
            this.Description = descr;
        }

        public FormationStatusEvent(int formationId, string formationName)
        {
            this.FormationId = formationId;
            this.FormationName = formationName;
        }

        // TODO: further implementation?
        public int FormationId { get; protected set; } = 0;

        public string FormationName { get; protected set; } = null;
    }
}
