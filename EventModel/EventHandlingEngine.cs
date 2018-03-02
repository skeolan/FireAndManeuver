// <copyright file="EventHandlingEngine.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System.Collections.Generic;
    using FireAndManeuver.Common;

    // ... more IGameEvent implementations ...
    public class EventHandlingEngine
    {
        public void ExecuteGamePhase(List<IEventActor> actors, GamePhaseEvent currentPhase, int logVerbosity, int consoleVerbosity)
        {
            Queue<GameEvent> eventQueue = new Queue<GameEvent>();
            eventQueue.Enqueue(currentPhase);

            while (eventQueue.Count > 0)
            {
                GameEvent evt = eventQueue.Dequeue();
                foreach (IEventActor actor in actors)
                {
                    // Could be an asynchronous method as long as eventQueue is a thread-safe queue implementation
                    List<GameEvent> result = actor.ProcessEvent(evt);
                    if (result != null)
                    {
                        eventQueue.EnqueueRange(result);
                    }
                }
            }
        }
    }
}
