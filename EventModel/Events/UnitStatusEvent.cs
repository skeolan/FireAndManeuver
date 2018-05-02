// <copyright file="UnitStatusEvent.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using FireAndManeuver.Common;

    public class UnitStatusEvent : GameEvent
    {
        public string Message { get; set; }

        public override string ToString()
        {
            return $"E{this.Exchange}.V{this.Volley} -- {this.Message}";
        }
    }
}