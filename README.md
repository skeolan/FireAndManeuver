# FireAndManeuver
A Full Thrust rules "plugin" for abstract movement - no tabletop, no minis, no measuring. Optimized for PbEM and automation.

[Current DRAFT ruleset is here](Ruleset-FireAndManeuver.md). These rules may see some further tweaks, but the core has been playtested enough that we're pretty sure it does the job. 

Scripting sheet examples using Google Sheets are [here](https://docs.google.com/spreadsheets/d/1K-JP-e6-w9P-Rw0wi63CcASXork-x1si9VUa5TSNN4k/edit?usp=sharing). Choose the format that makes the most sense to you; we'll probably settle on an "official" one eventually.

Apart from the above-mentioned rulesets and scripting sheets, the rest of this repo is an attempt to build out a .Net Core command-line application for resolving Full Thrust combats according to the Fire And Maneuver ruleset. 

Style notes:
* In Markdown documents, we're trying to adhere to the CommonMark spec here, mostly so we can edit in VSCode with usable previews.
* C# code is being developed according to the standard StyleCop rules, although for the moment we're suppressing the usual StyleCop insistence on XMLDoc comments for public members.
* This solution should be kept buildable/runnable in an isolated environment with only .NET Standard 2.0 available (and an internet connection for pulling down Nuget packages). Avoid major infrastructure requirements e.g. SQL, Azure storage / ServiceBus, etc.
