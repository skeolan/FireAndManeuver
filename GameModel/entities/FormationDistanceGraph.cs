// <copyright file="FormationDistanceGraph.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents all the established distances between Formations as a *sparse, weighted, undirected* graph of <see cref="GameFormation"/> nodes connected by <see cref="FormationDistance"/> edges.
    /// </summary>
    /// <remarks>
    /// Since the graph is undirected but we still want to be able to perform lookups in either direction (a to b or b to a), the graph
    /// typically stores two edges for each pair of nodes.
    ///
    /// This graph also supports sparseness, however -- if edges are missing from the graph but then need to be evaluated, the graph will add them.
    /// Initial weight of any new edges is defined as <see cref="Constants.DefaultStartingRange"/>
    ///
    /// Methods that update a given edge A->B will also update the reciprocal edge B->A to the same value.
    ///
    /// Methods that destroy a given edge A->B will also destroy the reciprocal B->A.
    /// </remarks>
    internal class FormationDistanceGraph
    {
        private List<GameFormation> nodes;
        private List<FormationDistance> edges;

        public FormationDistanceGraph(List<GameFormation> formations, List<FormationDistance> distances)
        {
            this.nodes = formations;
            this.edges = distances;
        }

        public FormationDistance SetDistance(GameFormation nodeA, GameFormation nodeB, int distance)
        {
            var edge = this.GetOrEstablishDistance(nodeA.FormationId, nodeB.FormationId, distance);
            var edgeReciprocal = this.GetOrEstablishDistance(nodeB.FormationId, nodeA.FormationId, distance);
            edge.Value = distance;
            edgeReciprocal.Value = distance;

            return edge;
        }

        /// <summary>
        /// Gets the current <see cref="FormationDistance" edge between two <see cref="GameFormation"/>s;
        ///  if one does not exist, establishes a new entry of distance <see cref="Constants.DefaultStartingRange"/>/>
        /// </summary>
        /// <param name="source">A <see cref="GameFormation"/></param>
        /// <param name="target">A second <see cref="GameFormation"/></param>
        /// <returns><see cref="FormationDistance"/> between two <see cref="GameFormation"/>s specified.</returns>
        public FormationDistance GetOrEstablishDistance(GameFormation source, GameFormation target)
        {
            return this.GetOrEstablishDistance(source.FormationId, target.FormationId);
        }

        /// <summary>
        /// Gets the current <see cref="FormationDistance" edge between two <see cref="GameFormation"/>s;
        ///  if one does not exist, establishes a new entry of distance <see cref="Constants.DefaultStartingRange"/>/>
        /// </summary>
        /// <param name="sourceId">ID of a <see cref="GameFormation"/></param>
        /// <param name="targetId">ID of a second <see cref="GameFormation"/></param>
        /// <returns><see cref="FormationDistance"/> between two <see cref="GameFormation"/>s specified.</returns>
        public FormationDistance GetOrEstablishDistance(int sourceId, int targetId)
        {
            return this.GetOrEstablishDistance(sourceId, targetId, Constants.DefaultStartingRange);
        }

        public FormationDistance GetOrEstablishDistance(int sourceId, int targetId, int establishDistance)
        {
            // Ensure valid inputs
            GameFormation nodeA = this.GetNodeById(sourceId);
            GameFormation nodeB = this.GetNodeById(targetId);

            if (establishDistance < 0)
            {
                throw new InvalidOperationException($"Invalid starting distance {establishDistance}: Formation distances must always be nonnegative.");
            }

            FormationDistance edge = this.GetEdge(nodeA, nodeB);
            FormationDistance edgeReciprocal = this.GetEdge(nodeB, nodeA);

            // edge-weight is equal to the edge-weight of the existing edge(s) or else the specified "establishDistance"
            var edgeWeight = edge != null ? edge.Value : edgeReciprocal != null ? edgeReciprocal.Value : establishDistance;

            // ensure both edge and its reciprocal exist
            edge = edge ?? this.AddEdge(nodeA, nodeB, edgeWeight);
            edgeReciprocal = edgeReciprocal ?? this.AddEdge(nodeB, nodeA, edgeWeight);

            return edge;
        }

        public void UpdateDistance(GameFormation source, GameFormation target, int rangeShift)
        {
            (var rangeBetweenFormations, var rangeReciprocal) = this.UpdateDistance(source.FormationId, target.FormationId, rangeShift);

            var consoleTail = $"{Math.Abs(rangeShift)} [{rangeBetweenFormations.Value - rangeShift} >> {rangeBetweenFormations.Value}]";
            Console.WriteLine($"-- Range {(rangeShift == 0 ? "unchanged" : (rangeShift < 0 ? "decreases by " : "increases by ") + consoleTail)}");
        }

        public Tuple<FormationDistance, FormationDistance> UpdateDistance(int sourceFormationId, int targetFormationId, int valueChange)
        {
            (var nodeA, var nodeB) = (this.GetNodeById(sourceFormationId), this.GetNodeById(targetFormationId));

            (var edgeA, var edgeB) = this.GetGraphState(nodeA, nodeB);

            this.SyncGraphState(edgeA, edgeB, nodeA, nodeB);

            edgeA.Value = edgeB.Value = edgeB.Value + valueChange;

            return new Tuple<FormationDistance, FormationDistance>(edgeA, edgeB);
        }

        private FormationDistance AddEdge(GameFormation nodeA, GameFormation nodeB, int edgeWeight)
        {
            var newEdge = new FormationDistance(nodeA, nodeB, edgeWeight);

            this.edges.Add(newEdge);

            return newEdge;
        }

        private GameFormation GetNodeById(int nodeId)
        {
            return this.nodes.Where(n => n.FormationId == nodeId).FirstOrDefault() ?? throw new InvalidOperationException($"Node with Id '{nodeId}' not found on graph");
        }

        /// <summary>
        /// Find one existing <see cref="GameFormation"/> edge between two specified <see cref="GameFormation"/>s.
        /// </summary>
        /// <param name="nodeA">Desired "source" node.</param>
        /// <param name="nodeB">Desired "target" node.</param>
        /// <returns>
        /// The existing <see cref="FormationDistance"/> edge from <paramref name="nodeA"/> to <paramref name="nodeB"/>, or null if none exists
        /// </returns>
        /// <remarks>
        /// Note that this method will *not* return the reciprocal edge if one exists (although one shouldn't, if the specified edge does not).
        /// </remarks>
        private FormationDistance GetEdge(GameFormation nodeA, GameFormation nodeB)
        {
            return this.edges.Where(e => e.SourceFormationId == nodeA.FormationId && e.TargetFormationId == nodeB.FormationId).FirstOrDefault();
        }

        private Tuple<FormationDistance, FormationDistance> GetGraphState(GameFormation nodeA, GameFormation nodeB)
        {
            // No edges can exist between a Formation and itself
            if (nodeA.FormationId == nodeB.FormationId)
            {
                throw new InvalidOperationException($"Cannot get distance-graph data between a formation ([ID {nodeA.FormationId}] {nodeA.FormationName}) and itself ([ID {nodeB.FormationId})] {nodeB.FormationName}");
            }

            var edge = this.GetOrEstablishDistance(nodeA, nodeB);
            var edgeReciprocal = this.GetOrEstablishDistance(nodeB, nodeA);

            return new Tuple<FormationDistance, FormationDistance>(edge, edgeReciprocal);
        }

        private void SyncGraphState(FormationDistance edge, FormationDistance edgeReciprocal, GameFormation nodeA, GameFormation nodeB)
        {
            // If neither the source -> target nor target -> source distances are in the Distances graph, create them at default starting range
            if (edge is null && edgeReciprocal is null)
            {
                edge = this.AddEdge(nodeA, nodeB, Constants.DefaultStartingRange);
                edgeReciprocal = this.AddEdge(nodeB, nodeA, Constants.DefaultStartingRange);
            }
            else
            {
                // If either source -> target or target -> source is missing but the other is present, add the missing one
                if (edge is null)
                {
                    edge = this.AddEdge(nodeA, nodeB, edgeReciprocal.Value);
                }

                if (edgeReciprocal is null)
                {
                    edgeReciprocal = this.AddEdge(nodeB, nodeA, edge.Value);
                }
            }

            // If distances don't match, the graph is in a bad state
            if (edge.Value != edgeReciprocal.Value)
            {
                throw new InvalidOperationException("Range between Formations is not symmetrical, something went wrong in updating the distance graph.");
            }
        }
    }
}