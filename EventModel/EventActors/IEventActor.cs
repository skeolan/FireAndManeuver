// <copyright file="IEventActor.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.EventActors
{
    using System.Collections.Generic;

    public interface IEventActor
    {
        List<GameEvent> ProcessEvent(GameEvent evt);
    }
}
