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
        private int unitId;
        private int unitMass;
        private string unitName;
        private int maxThrust = 0;

        public GameUnitFormationInfo()
        {
            this.unitReference = new GameUnit();
            this.Refresh();
        }

        public GameUnitFormationInfo(GameUnit unit)
        {
            this.unitReference = unit;
            this.Refresh();
        }

        public GameUnitFormationInfo(List<GameUnit> units, int unitId)
        {
            var unit = units.Find(u => u.IdNumeric == unitId) ??
                throw new System.InvalidOperationException($"Unit with ID {unitId} not found in Unit list.");

            this.unitReference = unit;
            this.Refresh();
        }

        [XmlAttribute("id")]
        public int UnitId
        {
            get => this.unitId; set => this.unitId = value;
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

        public override string ToString()
        {
            return $"[{this.UnitId, 2}] {this.UnitName} - {this.Mass, 3} MU - "
                 + $"Thrust {this.MaxThrust - this.ExtraThrust}/{this.MaxThrust}"
                 + $"{(this.IsFormationFlag ? " (Flagship)" : string.Empty)}";
        }

        private void GetGameUnitFormationInfo(GameUnit unit)
        {
            this.unitReference = unit;
            this.Refresh();
        }

        private void Refresh() // Consider redirecting getters/setters to work on the reference object instead?
        {
            var u = this.unitReference;
            this.Mass = u.Mass;
            this.MaxThrust = u.GetCurrentThrust();
            this.UnitId = u.IdNumeric;
            this.UnitName = u.Name;
        }
    }
}