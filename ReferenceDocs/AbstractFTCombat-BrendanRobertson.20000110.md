http://home.pacific.net.au/~southernsk/ft/abstract.htm

> From: Robertson, Brendan  
> Sent: Wednesday, 5 January 2000 17:46  
> Subject: [FT] Abstract Combat for campaigns  
> This started out as an abstract system so I could run a PBeM campaign without having to fight out the combats & wound up as a battlevalue grading of all the FTFB ships. Some of the values have a fudge factor built in to account for guess weaponry (SMLs mostly).  
> I still haven't factored in fighters properly, but it does throw up some startling comparisons in ship firepower & survivability.  
> 
> # Mechanics
> * **OV**: Sxx/Mxx/Lxx [xx/1] = Offence Value - tactical damage at each range band (see below)
> * **DV**: Defence Value
> 
> Both are rounded to nearest integer.
> 
> # Tactics
> * **Assault**: move to close range
> * **Circle**: move to medium range
> * **Snipe**: move to long range
> * **Retreat**: move to extreme range & disengage
> Ships change 1 range band for each 2 thrust difference in the ships.
> Initiative rolls (d6) will be used if both sides have the same thrust & conflicting tactics to see who is more successful.
> 
> # Resolution:
> Ships start at extreme range (36+ MU) and follow the assigned tactic from that point.  
> Ships should be grouped into task forces with similar thrusts and each task force is given a tactic & target priority chosen from: ESC, CRU, CAP.  
> The total OV & DV of the task group ships are added together & combat is resolved each turn until 1 side is reduced to 50% DV.  At this point, the task group will attempt to retreat.  
> 
> Damage is allocated amongst the ships on a % basis between ship types, excess damage is applied to the next largest ship type.  Full ships of one type are destroyed before applying damage to the next ship.  
> Eg. After a particularly disastrous battle, 6 DDs (dv:13) & 4 BBs (dv:58) have taken 140 points of damage.  60% is applied to the DDs (resulting in all 6 destroyed with 6 pts left over) and 40% is applied to the BBs (resulting in 1 BB with 56 damage + the 6 left over is 1 destroyed BB & another with a few paint scratches).  If required, the % damage can be applied to the FT hull to give the actual pts damage if you get to play a detailed game.  
> 
> # OV values
> * Beams => 1 per die x arc multiple
> * Pulsetorps => S = 4, M = 3, L = 2 x arc multiple
> * subpacs => S = 2, M = 0.4
> * Arc multiples: 1 arc = 0.6, 2 arc = 0.8, 3 arc = 1.0, 4 arc = 1.1, 5 arc = 1.2, 6 arc = 1.3.
> 
> * SML/Rs => Std = S,M = 7. ER = S,M,L = 7.
> * capship missiles => all ranges = 4.
> * Fighters are valued at average damage & degrade by 2 pts/attack for non-torpedo fighters.
> * Missiles are also noted after the OV with (total missile damage/turns). After the turns are exhausted, this value is deducted from the OV except when it reduces OV below 0.
> 
> # DV values
> * [Hull + armour + (<PDS+C1> x thrust/2)] x screen multiples.
> * PDS = 1, C1 = 0.3, Screen 1 = 1.2, screen 2 = 1.4
> * ADFC adds 50% to PDS value.
> 
> # Example
> ```text
> Orbital Weapon Platform
> Mass: 60
> OV: 13/10/1 [7/1]
> DV: 46
> 
> Cost: 211 pts
> Basic Station Drives (MD1 equivalent)
> Hull: Strong (24; 6/6/6/6)
> Armour: 6
> [OOOOOO/oooooo/o*oooo/ooo*oo/ooooo*]
> Screen - 2
> Firecons: 2
> ADFC
> 4 x PDS
> 1 x Class-3 (FP/F/FS)
> 1 x Class-2 (6-arc)
> SMR (std) (FP/F/FS)
> ```

# NAC
|Class Name            | OV             |  DV  |  FTRS  |
|:---------------------|:--------------:|:----:|:------:|
|Harrison SC           |  1/ 0/ 0       |   3  |        |
|Arapaho CT            |  3/ 0/ 0       |   5  |        |
|Minerva FF            |  5/ 1/ 0       |   7  |        |
|Tacoma FH             |  7/ 2/ 0       |   9  |        |
|Ticonderoga DD        |  7/ 2/ 0       |  13  |        |
|Huron CL              |  9/ 6/ 0       |  23  |        |
|Furious CE            | 10/ 5/ 2       |  38  |        |
|Vandenburg CH         |  8/ 4/ 1       |  43  |        |
|Vandenburg CH/T       | 13/ 8/ 3       |  40  |        |
|Majestic BC           | 18/12/ 2 [7/3] |  53  |        |
|Victoria BB           | 19/11/ 5       |  58  |        |
|Excalibur BDN         | 17/ 9/ 4       |  67  | 1 (xx) |
|Valley Forge SDN      | 21/12/ 5       |  88  | 2 (xx) |
|Inflexible CVL        |  5/ 1/ 0       |  65  | 4 (xx) |
|Ark Royal CVH         |  7/ 3/ 0       |  86  | 7 (xx) |

# NSL
|Class Name            | OV             |  DV  |  FTRS  |
|:---------------------|:--------------:|:----:|:------:|
| Falke SC             | 1/0/0          | 4    |        |
| Falke SC/S           | 3/0/0          | 3    |        |
| Stroschen CT         | 2/1/0          | 8    |        |
| Ehrenhold FF         | 4/2/0          | 12   |        |
| Waldburg DD          | 7/2/0          | 17   |        |
| Waldburg/M DD        | 8/7/0 [7/2]    | 14   |        |
| Kronprinz CL         | 9/3/0          | 29   |        |
| Radetzky CE          | 11/4/0         | 33   |        |
| Markgraf CH          | 13/6/2         | 42   |        |
| Maximillian BC       | 16/8/3         | 47   |        |
| Richthofen BC        | 19/10/4        | 44   |        |
| Maria Von Burgund BB | 26/15/7        | 51   |        |
| Szent Istvan BDN     | 17/10/4        | 78   | 1 (xx) |
| Von Tegetthoff SDN   | 31/20/4 [7/4]  | 99   | 1 (xx) |
| Der Theuerdank CVA   | 19/10/3        | 107  | 4 (xx) |

# FSE
|Class Name            | OV              |  DV  |  FTRS  |
|:---------------------|:--------------: |:----:|:------:|
| Mistral SC           | 3/0/0           |  4            |
| Athena CT            | 5/0/0           |  9            |
| Ibiza FF             | 7/1/0           |  10           |
| San Miguel DD        | 7/2/0           |  20           |
| Trieste DH           | 10/8/0 [7/2]    |  17           |
| Suffren CL           | 11/9/0 [7/3]    |  22           |
| Milan CE             | 12/9/0 [7/3]    |  26           |
| Jerez CH             | 21/17/0 [14/2]  |  34           |
| Ypres BC             | 16/10/0 [7/3]   |  46           |
| Roma BB              | 25/19/14 [14/2] |  50           |
| Bonaparte BDN        | 17/5/1 [7/4]    |  74  | 1 (xx) |
| Foch SDN             | 36/30/23 [21/2] |  104 | 3 (xx) |
| Bologna CVL          | 14/10/7 [7/2]   |  72           |
| Jeanne D'Arc CVA     | 17/13/9 [7/2]   |  115 | 7 (xx) |

# ESU
|Class Name            | OV             |  DV  |  FTRS  |
|:---------------------|:--------------:|:----:|:------:|
| Lenov SC             | 1/0/0          | 2    |        |
| Nanuchka II CT       | 3/1/0          | 8    |        |
| Novgorod FF          | 5/1/0          | 12   |        |
| Warsaw DD            | 7/2/0          | 14   |        |
| Volga DH             | 9/3/0          | 17   |        |
| Beijing/B CE         | 10/4/1         | 34   |        |
| Beijing/C CE         | 7/2/0          | 49   |        |
| Gorshkov CH          | 24/18/1 [14/1] | 31   |        |
| Vorshilev CH         | 13/6/2         | 40   |        |
| Manchuria BC         | 13/7/2         | 52   |        |
| Petrograd BB         | 18/8/2         | 65   |        |
| Rostov BDN           | 13/7/2         | 74   | 1 (xx) |
| Komarov SDN          | 25/15/7/2      | 130  | 1 (xx) |
| Tsiolkovsy CVL       | 11/4/0         | 65   | 4 (xx) |
| Konstantin CVA       | 23/12/4        | 110  | 6 (xx) |

> Neath Southern Skies - http://users.mcmedia.com.au/~denian/  
> [mkw] Admiral Peter Rollins; Task Force Zulu  
> [pirates] Prince Rupert Raspberry; Base Commander 