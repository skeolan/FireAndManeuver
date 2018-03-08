﻿// <copyright file="VolleyResolutionEngine.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using FireAndManeuver.EventModel;
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using Microsoft.Extensions.Logging.Debug;

    public class VolleyResolutionEngine
    {
        public static GameState ResolveVolley(GameState gameState, int volley, string sourceFileName, ILogger logger)
        {
            gameState.Volley = volley;

            var actors = GetActors(gameState, logger);

            // MANEUVER
            ExecuteManeuversForVolley(gameState.Volley, gameState, logger);

            // FIRE
            ExecuteCombatForVolley(gameState.Volley, gameState.Exchange, gameState, actors, logger);

            return gameState;
        }

        public static void RecordVolleyReport(GameState gameState, string originalSourceFile, string destinationFolder, ILogger logger)
        {
            var destFileName = $"Game-{gameState.Id}.{Path.GetFileNameWithoutExtension(originalSourceFile)}.VolleyResults.E{gameState.Exchange}V{gameState.Volley}" + Path.GetExtension(originalSourceFile);
            destFileName = destFileName.Replace($"Game-{gameState.Id}.Game-{gameState.Id}.", $"Game-{gameState.Id}.");
            var destFileFullName = Path.Combine(
                destinationFolder,
                destFileName);
            gameState.SourceFile = destFileFullName;
            logger.LogInformation($" - Volley {gameState.Volley} interim report saving to:" +
                $"\n          {gameState.SourceFile}");
            GameStateStreamUtilities.SaveToFile(gameState.SourceFile, gameState);
        }

        public static void RecordExchangeReport(GameState gameState, string originalSourceFile, ILogger logger)
        {
            var destFileName = Path.GetFileNameWithoutExtension(originalSourceFile) + $".ExchangeResults.{gameState.Exchange}" + Path.GetExtension(originalSourceFile);
            var destFileFullName = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                destFileName);
            gameState.SourceFile = destFileFullName;
            logger.LogInformation($" - Exchange {gameState.Exchange} completed report saving to:" +
                $"\n          {gameState.SourceFile}");
            GameStateStreamUtilities.SaveToFile(gameState.SourceFile, gameState);
        }

        private static bool ExecuteCombatForVolley(int currentVolley, int currentExchange, GameState gameState, IList<IEventActor> actors, ILogger logger)
        {
            logger.LogInformation(" - FIRE SEGMENT");

            // TODO: DI this interaction -- pass in EventHandlingEngine as a parameter
            // TODO: EventHandlingEngine should populate its own IList<GameActor> property
            var eventEngine = new EventHandlingEngine();

            eventEngine.ExecuteGamePhase(actors, new FiringPhaseEvent(currentVolley, currentExchange), 1, 1);

            //--And return!
            return true;
        }

        private static IList<IEventActor> GetActors(GameState gameState, ILogger logger)
        {
            var actors = new List<IEventActor>
            {
                new EventLoggingActor(logger)
            };

            actors.AddRange(gameState.Formations.Select(f => new GameFormationActor(f)));

            // TODO: Similarly add IEventActors for Units, and for the gameState itself
            return actors;
        }

        private static bool ExecuteManeuversForVolley(int currentVolley, GameState gameState, ILogger logger)
        {
            var builder = new StringBuilder();

            builder.AppendLine(" - MANEUVER SEGMENT");

            Dictionary<int, ManeuverSuccessSet> maneuverResultsById = new Dictionary<int, ManeuverSuccessSet>();

            // TODO -- Launch Phase (Ordnance, Fighters, Gunboats)

            //--Movement Phase

            // --- Determine Speed and Evasion successes for all Units
            foreach (var f in gameState.Formations)
            {
                builder.AppendLine($"Roll Speed and Evasion for {f.FormationName}, volley {currentVolley}");
                var formationOrders = f.Orders.Where(o => o.Volley == currentVolley).FirstOrDefault() ?? Constants.DefaultVolleyOrders;

                // Side effect: sets the ManeuverSuccesses and SpeedSuccesses property of the formation's current VolleyOrders.
                var result = f.RollManeuverSpeedAndEvasion(formationOrders, f.FormationId, currentVolley, speedDRM: 0, evasionDRM: 0);
                maneuverResultsById.Add(f.FormationId, result);
            }

            // -- Adjudicate all maneuver tests
            foreach (var f in gameState.Formations)
            {
                ExecuteManeuversForFormation(currentVolley, f, gameState);
            }

            logger.LogInformation(builder.ToString());

            // TODO --Secondary Movement Phase (Fighters and Gunboats only)
            return true;
        }

        private static void ExecuteManeuversForFormation(int currentVolley, GameFormation f, GameState gameState)
        {
            VolleyOrders orders = f.GetOrdersForVolley(currentVolley);
            foreach (var o in orders.GetSortedManeuveringOrders())
            {
                var target = gameState.Formations.Where(t => t.FormationId == o.TargetID).FirstOrDefault();

                var speed = Math.Max(0, orders.SpeedSuccesses + ManeuverOrder.GetManeuverModifier(o.Priority));

                if (target == null || o.ManeuverType == Constants.PassiveManeuverType)
                {
                    // Bail early if the maneuver is a passive one, e.g. "Maintain"
                    // Bail early if the maneuver is not against a valid target Id (e.g. "Target 0" default orders or orders against Formations that have been destroyed)
                    continue;
                }

                int rangeShift = o.CalculateRangeShift(currentVolley, f, orders.SpeedSuccesses, target);
                gameState.DistanceGraph.UpdateDistance(f, target, rangeShift);
            }
        }
    }
}
