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
            : base(
                  formationId: formationId,
                  formationName: formationName,
                  message: $"Formation [{formationId}]{formationName} destroyed!")
        {
            // All about that base
        }
    }
}
