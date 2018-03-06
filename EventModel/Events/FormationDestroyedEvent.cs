// <copyright file="FormationDestroyedEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class FormationDestroyedEvent : FormationStatusEvent
    {
        public FormationDestroyedEvent(int formationId, string formationName)
            : base(formationId, formationName)
        {
            this.Description = $"Formation [{this.FormationId}]{this.FormationName} destroyed!";
        }
    }
}
