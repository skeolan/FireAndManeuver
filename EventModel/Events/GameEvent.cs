// <copyright file="GameEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class GameEvent
    {
        public GameEvent(string description = "Abstract base GameEvent", int exchange = 0, int volley = 0)
        {
            this.Description = description;
            this.Exchange = exchange;
            this.Volley = volley;
        }

        public string Description { get; set; }

        public int Exchange { get; set; }

        public int Volley { get; set; }
    }
}
