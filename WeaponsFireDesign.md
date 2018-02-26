1. `GameEngine` evaluates `FireOrders` on each `Formation`. 
    - `Formation`s initiate weapons fire by all `Unit`s participating in a given `FireOrder`, potentially with global modifiers (penalty for splitting fire, etc.)
    - This will yield a list of `FirePackets`, each consisting of:
      - A `WeaponType`
      - A set of `dieRolls`
      - `TrackRating` (i.e. how many points of Evasion this packet can ignore, based on number of arcs the weapon mount covers and any other weapon / Unit properties)
      - The target `Formation`
      - The originating `Formation` and `Unit` (it's important to know who killed whom!)
      - The range X
      - A `damageType` `{Standard, Penetrating, AP, Semi-AP}`
      - Maybe a few other flag properties 
        - `PDVulnerable`
        - `IgnoresShield` 
        - `IgnoresEvasion` (as distinct from `TrackRating` which just mitigates it)
      - (if appropriate) the ID of the specific `Unit` it's meant to target (most `FirePacket`s just target Formations, though.)
      - If I later add some exotic new weapon that has to be handled in a special way, I can add more properties to these FirePackets.  Weird cases I can think of that might need to be handled:
        - Fire will arrive in a future turn (This one would be tough!)
        - Fire is from an area-of-effect weapon
        - Fire deals double damage if target's cloaking device is active
        - Fire deals no damage, but instead directly disables or destroys a specified target System IFF it would otherwise deal 1 or more damage
        - Fire is highly accurate and targets a specific `Unit` within the `Formation`, or a specific `System` of a specific `Unit`.
    - Return the List<FirePacket> back to the `GameEngine`.
    
1. GameEngine stores all of these lists in an associative array, `{ FormationId -> FirePacket[], FormationId -> FirePacket[]}`
    >`Formation` fA contains ships s1 and s2; `Formation` fB contains `Unit`s s3 and s4.  fA has a `FireOrder` targeting fB, and vice versa. The `FormationDistance` between them is 12 MU.
    >
    >fA composes a list of `FirePacket`s from s1 and s2:
    >- s1 has a few weapons assigned to this `FireOrder`, so invoke the FireAt(fB) method of each weapon via the IWeaponSystem interface, each yielding a FirePacket:
    >   - Class-3 Beam* attack `{s1, BeamP, [6, 6, 4], Track 1, fB, 12, Penetrating}` (note that this likely triggers penetrating damage, but we don't roll for it yet -- it might not even hit!).
    >   - Class-2 Beam* attack `{s1, BeamP, [3, 2], Track 2, fB, 12, Penetrating}`
    >   - Pulse Torpedo attack `{s1, PulseTorpedo, [5], Track 0, fB, 12, Semi-AP}`
    >- s2 has no weapons assigned to this FireOrder - it's on dedicated Point Defense duty, so it yields no FirePackets.
    >
    >fB composes a list of `FirePacket`s from s3 and s4:
    >- s3 and s4 are unarmed freighters, so fB has no FireOrders.
    >
    >`GameEngine` routes `FirePacket`s to their respective targets:
    >
    >```
    >{
    >"formationA" : [],
    >"formationB" : 
    >  [
    >    {s1, "BeamP",        [6, 6, 4], "Track 1", "formationB", 12, "Penetrating"},
    >    {s1, "BeamP",        [3, 2]   , "Track 2", "formationB", 12, "Penetrating"},
    >    {s1, "PulseTorpedo", [5]      , "Track 0", "formationB", 12, "Semi-AP"}
    >  ]
    >}
    >```
  
1. `GameEngine` passes all `FirePacket`s to their intended `Formation`s for processing.
    - Each Formation evaluates each `FirePacket`:
      - Determine what Unit the FirePacket targets
        > Formations encode some behavior around tougher ships shielding weaker ones, or faster ones screening for slower, but some weapons-fire overrides that and accurately targets individual Units (Needle Beams, Fighters).
      - Potentially, *modify* incoming FirePackets
        > Formations can have an Evasion score based on the outcome of their Maneuvering -- this decrements some (but not necessarily all) die rolls
        > Natural 6s need to be treated carefully in this case (they do get downgraded, but still count as natural 6s for purposes of penetration unless they miss altogether).
      - Apply PointDefense capabilities from all `Unit`s against all incoming FirePackets, potentially with some prioritization logic in case there's not enough PD to catch all the incoming; result of this may be removal of dice from a FirePacket or removal of the FirePacket from the list altogether.
  
      > fA's firePackets get resolved on fB: s3 is 66% of the total Mass of the formation, and s4 is using its excess Thrust to shift an additional 10% of its "mass-share" onto s3, for a total of 76% change to target s3 and 24% to target s4. A random roll of 75 indicates the target is s3.
  
    - Each `Unit` evaluates each `FirePacket` 
      - Convert it into a `DamagePacket` based on its die-rolls, but in a manner specific to the `WeaponType` (in terms of amount of damage), `DamageType`, and possibly one or two of those other `FirePacket` properties, plus the properties of the target `Unit` (shields, evasion, ...): 
        - This may entail re-rolling some dice to add extra damage (6s on Penetrating beams, for example, get to roll for MORE damage which ignores shields and evasion). 
        > Unclear yet whether the FirePacket should be *created* with all its re-rolls or whether the *target* should perform the re-rolls if the initial 6s are valid hits.
        - Total damage gets recorded onto the ship's Armor / Hull graph like one of the following, depending on the damage-type; see examples in the FT Continuum book, page 31-32.
      - Record the result of each FirePacket into their `Formation`'s DamageEvents list, e.g. 
      - Determine whether enough damage to Hull has occurred to trigger Threshold Checks, and if so, roll them and possibly have some Systems get knocked out. Add any Threshold failures to the `Formation`'s DamageEvents list for reporting back to the GameEngine.
      - Determine whether the `Unit` is now disabled or destroyed; if so, flip relevant state flags in its `Formation` appropriately and add yet more DamageEvents.
    - Each Formation checks its `DamageEvents` list and takes any necessary state update actions (updating `MaxThrust` for main-drive hits; removing dead `Unit`s).
    - Each Formation returns its `DamageEvents` list back to the `GameEngine` for updating .
    
1. `​GameEngine` reads the `DamageEvent`s back from all the GameFormations and does lots of important stuff with it (writes events to a log file, emits them to console, updates the Distances graph if a Formation has been completely obliterated, etc).
    > ```
    > s3-123 - Beam-3 Fire from s1 to s3 [3D6 B* - 6, 5, 4; 6, 6, 4] dealt 4+2+2+1 damage, reduced to 3+2+2+1 by shields
    > s3-124 - Beam-2 Fire from s1 to s3 [3D6 B* - 4, 4] dealt 2 damage, reduced to 0 by shields
    > s3-125 - Pulse Torpedo Fire from s1 to s3 [1D6 SAP - 2] dealt 0 damage, clean miss at 12 MU
    > s3-126 - s3 triggered Threshold checks at +0
    > s3-127 - s3 failed Threshold checks on Life Support, PDS #7, FTL system, Main Drive
    > s3-128 - s3 Damage Control priority assignment to FTL system
    > fB-001 - Formation B Max Thrust drops to 2; s4 Excess Thrust rises to 4; To-Hit allocation changes to [86% s3(+20), 14% s4 (-20)].
    > ```
