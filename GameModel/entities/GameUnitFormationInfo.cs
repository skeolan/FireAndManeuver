// <copyright file="GameUnitFormationInfo.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("FormationUnit")]
    public class GameUnitFormationInfo
    {
        private GameUnit unitReference;
        private GameFormation formationReference;
        private int unitId;
        private int unitMass;
        private string unitName;
        private int maxThrust = 0;

        public GameUnitFormationInfo()
        {
            this.unitReference = new GameUnit();
            this.formationReference = new GameFormation();
            this.Refresh();
        }

        public GameUnitFormationInfo(GameUnit unit, GameFormation formation)
        {
            this.unitReference = unit;
            this.formationReference = formation;
            this.Refresh();
        }

        public GameUnitFormationInfo(List<GameUnit> units, int unitId, GameFormation formation)
        {
            this.unitReference = GetUnitById(units, unitId);
            this.formationReference = formation;
            this.Refresh();
        }

        public GameUnitFormationInfo(List<GameUnit> units, int unitId, List<GameFormation> formations, int formationId)
        {
            this.unitReference = GetUnitById(units, unitId);
            this.formationReference = GetFormationById(formations, formationId);
            this.Refresh();
        }

        [XmlAttribute("id")]
        public int UnitId
        {
            get => this.unitId; set => this.unitId = value;
        }

        [XmlIgnore]
        public int FormationId
        {
            get => this.formationReference.FormationId;
        }

        [XmlAttribute("mass")]
        public int Mass { get => this.unitMass; set => this.unitMass = value; }

        [XmlAttribute("hitModifier")]
        public int HitModifier { get; set; } = 0;

        [XmlAttribute("extraThrust")]
        public int ExtraThrust { get; set; } = 0;

        [XmlAttribute("maxThrust")]
        public int MaxThrust { get => this.maxThrust; set => this.maxThrust = value; }

        [XmlAttribute("flag")]
        public bool IsFormationFlag { get; set; } = false;

        [XmlText]
        public string UnitName { get => this.unitName; set => this.unitName = value; }

        [XmlIgnore]
        public int PercentileLowerBound { get; private set; } = 1;

        [XmlIgnore]
        public int PercentileUpperBound { get; private set; } = 100;

        public override string ToString()
        {
            return $"[{this.UnitId, 2}] {this.UnitName} - {this.Mass, 3} MU - "
                 + $"Thrust {this.MaxThrust - this.ExtraThrust}/{this.MaxThrust}"
                 + $"{(this.IsFormationFlag ? " (Flagship)" : string.Empty)}";
        }

        public GameUnit GetUnitReference()
        {
            return this.unitReference;
        }

        public GameFormation GetFormationReference()
        {
            return this.formationReference;
        }

        public bool CoversPercentile(int unitAssignmentPercentile)
        {
            (this.PercentileLowerBound, this.PercentileUpperBound) = this.formationReference.GetHitRangeForUnit(this.UnitId);

            return this.PercentileLowerBound <= unitAssignmentPercentile && this.PercentileUpperBound >= unitAssignmentPercentile;
        }

        internal ScreenRating GetAreaScreenRating()
        {
            ScreenRating areaScreenRating = new ScreenRating();

            if (this.unitReference != null)
            {
                areaScreenRating = this.unitReference.GetAreaScreenRating();
            }

            return areaScreenRating;
        }

        internal ScreenRating GetUnitScreenRating()
        {
            ScreenRating unitScreenRating = null;
            ScreenRating formationScreenRating = this.formationReference.GetFormationAreaScreenRating();

            if (this.unitReference != null)
            {
                // Screens can stack up in cases where a Unit has local screens and the formation has area screens.
                unitScreenRating = ScreenRating.Combine(formationScreenRating, this.unitReference.GetLocalScreenRating());
            }

            return unitScreenRating;
        }

        private static GameUnit GetUnitById(List<GameUnit> units, int unitId)
        {
            return units.Find(u => u.IdNumeric == unitId) ??
                            throw new System.InvalidOperationException($"Unit with ID {unitId} not found in Unit list.");
        }

        private static GameFormation GetFormationById(List<GameFormation> formations, int id)
        {
            return formations.Find(f => f.FormationId == id) ??
                            throw new System.InvalidOperationException($"Formation with ID {id} not found in Formation list.");
        }

        private void Refresh() // Consider redirecting getters/setters to work on the reference object instead?
        {
            var u = this.unitReference;
            var f = this.formationReference;
            this.Mass = u.Mass;
            this.MaxThrust = u.GetCurrentThrust();
            this.UnitId = u.IdNumeric;
            this.UnitName = u.Name;
        }
    }
}