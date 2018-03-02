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
        public GameEvent(string description = "Abstract base GameEvent")
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }
}
