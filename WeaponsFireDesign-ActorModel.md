# Design for Game Phase Resolution via the Actor Model

## Weapons Fire Phase Example
`GameEngine` calls `eventEngine.ExecuteGamePhase(FiringPhase)` to kick off resolution of the firing phase; eventEngine pushes the "Firing Phase Started" event onto an event queue. When the queue is empty (i.e. the last event has been dequeued and processed by all Actors without any new events firing), the phase is complete.

The EventEngine dequeues each Event from the queue in turn, and instructs all Actors to react (or choose not to react) to the event by changing their internal state and/or adding new events to the back of the queue:

* `Formation` actors react to the `FiringPhase` event by constructing a set of `AttackEvent`s based on their FireOrders and emitting each one onto the queue.
* `Formation` actors react to `AttackEvent`s (IFF the event targets them) by determining which of their constituent unit(s) should be affected and emitting a `DamageEvent` targeting each one.
* `Unit` actors react to `DamageEvent`s that target them by adjusting their internal state accordingly and then emit events as appropriate: `UnitDamaged`, `UnitDisabled`, `UnitSystemDestroyed`, `UnitThrustDecreased`, `UnitDestroyed`, etc.
* `Formation` actors react to `UnitThrustDecreased`, `UnitDisabled`, `UnitDestroyed`, etc. events by adjusting their internal state accordingly.
  * Special case: if `UnitDestroyed` event reduces a `Formation`'s unit set to zero, the `Formation` emits a `FormationDestroyed` event.
* `GameEngine` is itself an Actor because it needs to respond to `FormationDestroyed` and `UnitDestroyed` events by updating its `Players`, `AllFormations` and `AllUnits` collections.
* The `DistanceGraph` actor reacts to `FormationDestroyed` events by removing that `Formation`'s node from the graph and deleting any edges that touched it.
* The `Logger` actor reacts to every event that enters the queue by logging it, and never emits any events.
* The `ConsoleDisplayEngine` actor reacts to some events by printing event information and/or summary reports to stdout, often with snazzy formatting.

## Interfaces and event-queue loop
```C#
public interface IEventActor
{
    public List<IGameEvent> ProcessEvent(IGameEvent, Queue<IGameEvent>);
}

public interface IGameEvent
{   
    public IEventActor source;
    public IEventActor target;
    
    public string Description;
    
    // Not sure what other properties might be universally present in IGameEvent
}

public class EventHandlingEngine
{
    public void ExecuteGamePhase(List<IEventActor> actors, GamePhaseEvent currentPhase, int logVerbosity, int consoleVerbosity)
    {
        Queue<IGameEvent> eventQueue = new Queue<IGameEvent>() { currentPhase };

        while(!eventQueue.IsEmpty())
        {
            IGameEvent evt = eventQueue.Dequeue();
            foreach(IEventActor actor in actors)
            {
                // Could be an asynchronous method as long as eventQueue is a thread-safe queue implementation
                List<IGameEvent> result = actor.ProcessEvent(evt, eventQueue)
                if (result != null) eventQueue.EnqueueRange(result); 
            }
        }
    }
}
```

## Linking EventHandlingEngine up to the GameEngine state and pushing Actors into the execution loop
```C#
public class GameEngine
{

    // ...

    // "Phase kickoff" events initialized elsewhere
    List<IGameEvent> GamePhases = new List<IGameEvent> { ThrustAllocationPhase, ManeuveringPhase, FiringPhase, DamageControlPhase };
    
    var eventEngine = new EventHandlingEngine();
    
    var actors = new List<IEventActor>();      
    actors.Add(this);                    
    actors.Add(new Logger(gameEngine, logVerbosity);
    actors.Add(new ConsoleDisplayEngine(gameEngine, consoleVerbosity));
    actors.Add(this.DistanceGraph);
    actors.AddRange(this.AllFormations); 
    actors.AddRange(this.AllUnits);      
        
    foreach (phase in GamePhases)
    {
        eventEngine.ExecuteGamePhase(actors, phase, Constants.VERBOSITY_DEBUG, Constants.VERBOSITY_CONSOLE_DEFAULT)
    }
    
    // ...
}
```

## IGameEvent Implementations
```C#
public class GameEventBase : IGameEvent
{
    public IEventActor source;
    public IEventActor target;
    
    public string Description;
    
    public GameEventBase(IEventActor source, IEventActor target, string description)
    {
        Source = source;
        Target = target;
        Description = description;
    }
}

public class FiringPhase : GameEventBase, IGameEvent
{
    public FiringPhase() : base (null, null, "Firing Phase Begun") 
    { };
}

public class AttackEvent : GameEventBase, IGameEvent
{
    public WeaponTypeEnum WeaponType;
    public OrderedList<DieRoll> DieRolls;
    public int TrackRating;
    
    public IEventActor sourceUnit;
    public IEventActor targetUnit; // Usually null or ignored
    
    
    public List<AttackSpecialPropertyEnum> SpecialProperties;
    
    public AttackEvent(IEventActor sourceUnit, IEventActor sourceFormation, IEventActor targetFormation, ) : base (...)
    {
        // constructor constructs
    }
}

// ... more IGameEvent implementations ...
```

## IEventActor Implementations
```C#
public class GameFormation : IEventActor
{

    // ...
    
    public List<IGameEvent> ProcessEvent(IGameEvent evt, Queue<IGameEvent> evtQueue)
    {
        if(evt is FiringPhase)
        {
            List<AttackEvent> attacks = this.GetAttackEventsFromFireOrders();
            return attacks;
        }
        if(evt is AttackEvent && (AttackEvent) evt.TargetId == this.Id)
        {
            // ... handle incoming attack, enqueue new DamageEvents as appropriate back onto evtQueue
            return;
        }
        // ... other event handling
        
        // events for which there is no handler do not get any action from this Actor.
        
        return null;
    }
}

public class GameUnit : IEventActor
{
    // ... Different implementation of ProcessEvent ...
}    
    
    
// ... other Actors ...
```
