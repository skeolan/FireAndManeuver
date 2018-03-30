// <copyright file="EventHandlingEngine.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel
{
    using System.Collections.Generic;
    using FireAndManeuver.Common;
    using FireAndManeuver.EventModel.EventActors;

    // ... more IGameEvent implementations ...
    public class EventHandlingEngine
    {
        public void ExecuteGamePhase(IList<IEventActor> actors, GamePhaseEvent currentPhase, int logVerbosity, int consoleVerbosity)
        {
            // TODO: reimplement this using the "Actor Model via TPL Dataflow" approach from that blog post?
            Queue<GameEvent> eventQueue = new Queue<GameEvent>();
            eventQueue.Enqueue(currentPhase);

            while (eventQueue.Count > 0)
            {
                GameEvent evt = eventQueue.Dequeue();
                foreach (IEventActor actor in actors)
                {
                    var isLogger = actor is EventLoggingActor;

                    // Could be an asynchronous method as long as eventQueue is a thread-safe queue implementation
                    IList<GameEvent> result = actor.ReceiveEvent(evt);
                    if (result != null && result.Count > 0)
                    {
                        eventQueue.EnqueueRange(result);
                    }
                }
            }
        }
    }
}
