# **Fire and Maneuver**: <br/> Abstract Full Thrust Combat Resolution #

# Intro #

This rules system is meant to replace the tabletop minis-and-rulers approach to Full Thrust combat with an abstract system which permits fast resolution of small-to-medium sized battles without needing much input from players after the initial setup. Exact position and facing are not tracked. SSDs for ships, stations, fighters, guboats, etc. from Full Thrust should be 100% compatible as-is with the F&M rules, although some systems may necessarily work slightly differently due to the abstract spatial representation.

Units utilize their Thrust ratings as a dice pool split into Speed, to attempt to increase, decrease or maintain distance to their maneuvering target(s); and Evasion, to avoid incoming fire, decreasing or eliminating its effectiveness. Units also organize into Formations for maneuver and attack, lending each other mutual support and providing an opportunity for nimbler escorts to shield (or hide behind) larger comrades.

The Fire and Maneuver rules also focus on being electronically resolvable, to eventually require only an input datafile of orders and ship/formation data to present the simulated outcome of an Exchange.

> Note: Familiarity with the 
[Full Thrust Continuum](https://emeraldcoastskunkworks.files.wordpress.com/2014/12/full-thrust-project-continuum-version-1-1-3-jan-2016.pdf) 
(or at minimum [Full Thrust Lite](http://downloads.groundzerogames.co.uk/FT0916_UK.pdf)) 
rules is assumed; Fire and Maneuver will not make much sense without a grasp of the underlying Full Thrust mechanics to which they "plug in."

# Definitions #
  > **Commander** -- a player in control of one or more **Formations** of **Units**.
  > 
  > **Evasion** -- the portion of a Formation's Thrust pool assigned to avoiding enemy attacks. Rolls of 4-5 grant one **Evasion Point**, rolls of 6 grant two **Evasion Points**.
  > 
  > **Evasion Point** -- **Successes** on Evasion dice. a value indicating increasing difficulty landing a solid hit against a Formation: each point of Evasion adds -1 DRM to direct-fire weapon attacks, like Pulse Torpedoes and Beams; and permits rerolling one failed PD die against Missile weapons.
  > 
  > **Exchange** -- A set of three **Volleys** executed sequentially using a pre-written plot.
  > 
  > **Formation** -- one or more Units assigned to move, fire and defend together. Orders are given to Formations rather than to Units. Hits against Formations are assigned by a probability roll based on the mass of each Unit, but Commanders can adjust this to put some ships in harm's way to spare others (See "[Formation Targeting](#formation-targeting)").
  > 
  > **Maneuver** -- a choice of action by a Formation representing attempted movement within space relative to other Formations: either to **Close**, **Maintain** or **Withdraw** relative to a target Formation.
  > 
  > **Speed** -- the portion of a Formation's Thrust pool assigned to succeeding at a Maneuver. Rolls of 4-5 grant one Speed Point, rolls of 6 grant two Speed Points.
  >
  > **Speed Point** -- Successes on Speed Dice. a value indicating 3 MU worth of Maneuvering by a Formation relative to its target.
  > 
  > **Success** -- unless specific rules dictate otherwise, a roll of 4 or 5 on 1D counts as one Success; a roll of 6 counts as two Successes.
  > 
  > **Under Fire** -- a single Unit within a Formation, determined (usually randomly) to be acquired by a Fire Control. *Each Fire Control targeting a Formation will place a single Unit Under Fire (exceptions probably exist).*
  > 
  > **Unit** -- a single Ship, Station, or squadron of Fighters or Gunboats. *Anything that would have its own SSD and counter or model-base in traditional Full Thrust is a Unit.*
  > 
  > **Volley** -- A short time-span during which all Formations simultaneously resolve their plotted Maneuver actions, then resolve their plotted Fire actions. See [*Volley Procedure*](#volley-procedure).

# Sequence of Play #
## Starting Positions ##
Record the distance between all Formations as 60MU. Space is big and hard to hide in, so battles usually start beyond firing range unless agreed otherwise.

## Plot the Exchange
> Replaces Phase 1 "Write Orders" and Phase 2 "Roll for Initiative" of the Full Thrust Continuum Sequence of Play, and only occurs once per Exchange.

Plot three Volleys: orders for each Formation to *Fire and Maneuver*

1. All commanders plot Maneuver Orders for each of their Formations in all three Volleys of the coming Exchange.
    1. Divide Formation Thrust into Speed and Evasion. Maximum Evasion is half Formation Thrust, unless Advanced Drives are in use.
    1. Specify Formation Thrust rating for the Volley -- this equals the *lowest* Thrust rating of any Unit in the Formation.
    1. Specify Formation Targeting percentiles, as described under "Formation Targeting."
    1. Designate a Primary Maneuver target.
    1. Designate one or more Secondary Maneuver targets, if desired.
    1. Specify one Maneuver against each Maneuver Target:
        * **Close**: Try to decrease the range to target.
        * **Maintain**: Try to hold the target at the current range.
        * **Withdraw**: Try to increase the range to target.
    1. Note: a single Formation must not plot **Close** and **Withdraw** Maneuvers against different Maneuver Targets during a single Volley.
1. All commanders plot Fire Orders for each of their Formations in all three Volleys of the coming Exchange.
    1. Designate as many Fire targets as desired, according to normal Full Thrust rules (i.e. limited by Fire Controls etc.)
    1. For each Fire Control per Unit in a Formation: 
       1. Specify a target Formation if the FireCon is active, or "inactive" otherwise.
       1. Specify all weapons assigned to the FireCon (Default is "all").
    1. Specify any weapons firing in PD mode instead.
    1. Specify any other special actions, system activations, etc.
    1. Ignore firing arcs for purposes of weapon targeting.
1. No need to roll for initiative: Formations' plotted actions are resolved simultaneously in all Volley phases.

## Execute Volley 1. ##
> Replaces Phases 3 through 15 of the Full Thrust Continuum Sequence of Play.

> See [*Volley Procedure*](#volley-procedure).

## Execute Volley 2. ##
> See [*Volley Procedure*](#volley-procedure).

## Execute Volley 3. ##
> See [*Volley Procedure*](#volley-procedure).

## Wrap up the Exchange.
1. Update records to show destroyed, crippled, or surrendered Units or Formations.
1. If scenario permits, move Units / Formations to and from Reserves (see Appendix).
1. Declare victory if appropriate.
1. If scenario rules permit, and no victory has been achieved, begin a new Exchange.

# Volley Procedure 
### Maneuver ###
> This covers phases 3, 4, 5, and 6 of the Full Thrust Continuum Sequence of Play.

All commanders *simultaneously* execute each Maneuver phase for all their Formations.
#### Launch Phase
 1. **Launch fighter groups or gunboats** -- these arrive as members of their parent Formation **or** as one or more new Formation(s) of their own. (If the latter, they need to have been noted as such in the Plotting Phase.) See "Fighters and Gunboats" in the Appendix for details.
 1. **Launch Ordnance** -- any weapon plotted as Firing this turn, regardless of whether its action will turn out to be valid, does indeed fire. Mark off rack-mounted ordnance, magazine ammunition, etc at this time.

#### Movement Phase
 1. **Maneuver Formations** -- Roll Formations' Speed and Evasion assignments as separate dice pools. 
 	1. Record Speed Successes as Speed Points.
	1. Record Evasion Successes as Evasion Points.
	1. For each pair of Formations maneuvering against each other, determine the outcome based on their starting Range, chosen Maneuvers, and Speed Points:
		1. Record the starting Range between the two Formations.
		1. First, every Speed Point for a **Close** decreases the Range (-3 MU). Record the new Range.
		1. Next, every Speed Point for a **Withdraw** increases the Range (+3 MU). Record the new Range.
		1. Finally, every Speed Point for a **Maintain** moves the Range 3 MU *back toward the starting Range for the Volley* (+/- 3 MU). 
		1. The final value is the new Range between the two Formations.
	1. If (due to a lot of Close successes) the Range drops *below zero*, the two Formations have overshot one another and ended up at a new range equal to the "negative distance." Accidental collisions in space combat are vanishingly rare.
	1. Treat Speed Points as one lower for any Secondary Maneuver target(s).
	1. If a Formation did not plot against another Formation, treat it as a Secondary Maneuver (Maintain) and apply the required -1 Speed Point.
 1. **Secondary fighter/gunboat Maneuvers** -- Formations with only Fighter/Gunboat Units may, if desired, make a secondary Maneuver in this phase. See "[Fighters and Gunboats](#fighters-and-gunboats)" in the Appendix.

### Fire ###
All commanders *simultaneously* execute each Fire phase for all their Formations.

#### Point Defense Phase
> This covers phases 7, 8 (Fighters against Missiles) and 9 of the Full Thrust Continuum Sequence of Play. 

> Phase 8 (Fighters against fighters) is shifted into the following Weapon Attack Phase.

> Feel free to skip this phase if there are no Missile weapons or Fighters in play this turn.
  
PD **resolves as normal for Full Thrust**, except as follows.

  1. All commanders *simultaneously* roll applicable Point Defense dice for each Formation.
  1. Only dedicated PD systems, and PD-capable weapons *explicitly* plotted as firing in PD mode, contribute to Point Defense.
  1. For each Evasion Point a Formation has, reroll one PD die that resulted in a miss.

#### Weapon Attack Phase
> This covers phase 8 (Fighters against Fighters), and phases 10, 11 and 12 of the Full Thrust Continuum Sequence of Play; however, damage incurred during this phase does not take effect until the Apply Damage phase

All weapons fire **resolves as normal for Full Thrust at the current range**, except as follows.
  1. All commanders *simultaneously* roll applicable Missile, Projectile and Beam attacks for each Formation.
  1. For each firing Weapon, randomly determine which Unit within the target Formation is under fire: 
  	1. See "[Formation Targeting](#formation-targeting)" in the Appendix for details
	1. Attacks by Fighters, Gunboats, and precision weaponry like Needle Beams are plotted against specific enemy Units instead.
  1. Missile Attacks:
  	1. A Missile weapon's maximum range in F&M includes its "final approach" length, e.g. 24MU + 6MU for standard Salvo Missiles.
  	1. If a Missile attack is a Salvo-style group, roll as normal to see how many are on target.
  	1. Singular, Heavy Missile-style attacks are always on target.
  	1. Apply PD results. 
  	1. Any surviving Missiles deal their damage, as specified in the Full Thrust rules, to Unit under fire (See "[Damage Phase](#damage-and-threshold-checks-phase)" below).
  1. Projectile and Beam attacks:
  	1. Apply a -1 DRM per target Evasion Point.
    		1. Weapons with 3 or 4 firing arcs ignore one Evasion Point.
    		1. Weapons with 5 or 6 firing arcs ignore two Evasion Points.
  	1. Apply any other relevant modifiers due to Screens, etc.
  	1. Roll Beam Weapon attacks invidiually, rather than pooling dice (since DRM can vary from one Beam battery to another if their arcs differ).
  	1. Projectile and Beam Weapons which hit deal damage to the Unit under fire (See "[Damage Phase](#damage-and-threshold-checks-phase)" below).

#### Damage and Threshold Checks Phase
> This covers Phases 13, 14 and 15 of the Full Thrust Continuum Sequence of Play.

After resolving all Weapon Attacks, apply damage *simultaneously* to each Unit. Damage affects Units **as normal for Full Thrust**.
  1. Damage marks off armor/hull boxes.
  1. Some Beam weapons apply Penetrating (BD\*) damage.
  1. AP/SAP damage works normally.
  1. Roll and resolve Threshold Checks.
  1. Make any Damage Control repair rolls.
    1. Default assumption is the following order: Core Systems (Reactors, then Life Support, then Command); Drives; Screens; at least one functional Fire Control; then weaponry in order of die count; then other systems.
    1. Optionally, commanders can plot an overriding Damage Control ordering if certain systems or components should be prioritized.
  1. Check for Reactor explosions.
  1. Damage control and Reactor explosions **resolve as normal for Full Thrust.**

# Appendices
## Formation Targeting
If a Formation under fire has more than one Unit in it, then each Weapon Attack typically resolves against exactly one of those Units:  a Salvo Missile, Heavy Missile, Pulse Torpedo, or set of Beam Weapon attack dice will hit (or miss) *one Unit* in the Formation. Which weapons get aimed at which target Units is determined by randomly rolling for target acquisition *per Fire Control*.

> Attacks by Fighters, Gunboats, and precision weaponry like Needle Beams **are** plotted against specific Units instead.

To determine which Unit is under fire for a given Fire Control:
  1. Calculate the total Mass of the Formation.
  1. Calculate the percentile range "covered" by the mass of each Unit, from largest to smallest.
  1. Generate a random number between 1 and 100 to determine which Unit is under fire.
  1. Units with higher Thrust than the Formation Thrust value can increase or decrease their "share" of the percentile spread by up to 5 percentile points per surplus Thrust, shifting points to or from other Units as desired.
  1. After all percentile adjustments, a Unit cannot have a lower than 1 percent chance of being under fire.

> For example, if a Formation contains a 44-Mass Destroyer and a 22-Mass Frigate, then the Destroyer (2/3 total mass) occupies percentiles 01-67 and the Frigate occupies percentiles 68-100.

> If the attacker targeting the Formation rolls a 75 for one Fire Control's percentile result, then the Frigate is under fire by that FireCon's assigned weapons.

> If the Frigate has Thrust 8 while the Destroyer has Thrust 6, the Frigate can either increase or decrease its percentile "share" by 2x5=10 percentile points:

> - Increase Exposure (to protect the Destroyer): 01-57 Destroyer | 58-100 Frigate

> - Decrease Exposure (to protect the Frigate): 01-77 Destroyer | 78-100 Frigate

## Reserves

If it's appropriate to the Scenario, an offboard Reserve of Units may exist. 

In such cases, Formations move into Reserves when they are at 65 MU or greater to *all* enemy Formations. 

In some scenarios, Formations have to be FTL-capable to move to Reserves.

The Reserve is a single "off-board" Formation which can never directly enter or affect F&M combat, but can engage in Damage Control and other repair/salvage activities each Volley.

During plotting for an Exchange, a commander may designate a new Formation composed of Reserve Units.

Such a Formation starts at "off-board" range, and must plot "Enter Combat" - no maneuvering, no fire - for the upcoming three Volleys of the Exchange. In the Conclusion Phase, those Formations arrive at range 60MU to all enemy Formations; they can then be plotted normally in the subsequent Exchange.

## Fighters and Gunboats
```C#
throw new NotImplementedException();
```
