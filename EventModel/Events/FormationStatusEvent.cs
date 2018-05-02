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
        public FormationStatusEvent(string message = "Default FormationStatusEvent", int exchange = 0, int volley = 0, int formationId = 0, string formationName = null)
        {
            this.Message = message;
            this.Exchange = exchange;
            this.Volley = volley;

            this.FormationId = formationId;
            this.FormationName = formationName;
            this.Description = $"FormationStatus";
        }

        // TODO: further implementation?
        public int FormationId { get; protected set; } = 0;

        public string FormationName { get; protected set; } = null;

        public string Message { get; protected set; } = null;

        public override string ToString()
        {
            return $"[{this.FormationId}]{this.FormationName} -- {this.Message}";
        }
    }
}
