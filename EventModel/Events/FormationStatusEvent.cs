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
        public FormationStatusEvent(string descr = "FormationStatusEvent", int exchange = 0, int volley = 0, int formationId = 0, string formationName = null)
        {
            this.Description = descr;
            this.Exchange = exchange;
            this.Volley = volley;

            this.FormationId = formationId;
            this.FormationName = formationName;
        }

        // TODO: further implementation?
        public int FormationId { get; protected set; } = 0;

        public string FormationName { get; protected set; } = null;
    }
}
