// <copyright file="HullSystem.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class HullSystem : DefenseSystem
    {
        private int totalHull = 0;
        private int remainingHull = 0;

        public HullSystem()
        {
            this.HullRows = 4;
            this.SystemName = "Hull System";
        }

        [XmlAttribute("value")]
        public int HullValue { get; set; }

        [XmlAttribute("type")]
        public HullTypeProperty HullType { get; set; }

        [XmlAttribute("class")]
        public string HullClass { get; set; } = "Military";

        [XmlAttribute("rows")]
        public int HullRows { get; set; }

        [XmlAttribute("totalHullBoxes")]
        public int TotalHullBoxes
        {
            get
            {
                // if (type == HullTypeLookup.Custom) return _totalHullBoxes;
                if (this.totalHull != 0)
                {
                    return this.totalHull;
                }
                else
                {
                    // Automatically rounds DOWN?
                    return (int)(this.HullValue * this.HullTypeMultiplier);
                }
            }

            set
            {
                this.totalHull = value;
            }
        }

        [XmlAttribute("remainingHullBoxes")]
        public int RemainingHullBoxes
        {
            get
            {
                if (this.remainingHull != 0)
                {
                    return this.remainingHull;
                }
                else
                {
                    return this.TotalHullBoxes;
                }
            }

            set
            {
                this.remainingHull = value;
            }
        }

        [XmlIgnore]
        private decimal HullTypeMultiplier
        {
            get { return (int)this.HullType / (decimal)100.0; }
        }

        public override string ToString()
        {
            string typeDetail = this.HullType == HullTypeProperty.Custom ? "Custom" : string.Format("{0} [TMFx{1}]", System.Enum.GetName(typeof(HullTypeProperty), this.HullType), this.HullTypeMultiplier);
            return string.Format("{0, 3} / {1, 3} - {2} {3} Hull ({4} rows)", this.RemainingHullBoxes, this.TotalHullBoxes, typeDetail, this.HullClass, this.HullRows);
        }

        public List<string> HullDisplay()
        {
            List<string> hullDisplay = new List<string>();
            int minRowLength = this.TotalHullBoxes / this.HullRows;
            int maxRowLength = minRowLength + 1;
            int numMaxRows = this.TotalHullBoxes - (this.HullRows * minRowLength);

            int damageToAssign = this.TotalHullBoxes - this.RemainingHullBoxes;

            // TODO: extract crewFactorRatio, crewFactors, and DCPFrequency out into properties of Hull
            int crewFactorRatio;
            switch (this.HullClass.ToLowerInvariant())
            {
                case "civilian":
                {
                    crewFactorRatio = 50;
                        break;
                }

                case "merchant":
                {
                    crewFactorRatio = 50;
                        break;
                }

                case "military":
                {
                    crewFactorRatio = 20;
                        break;
                }

                default:
                {
                    crewFactorRatio = 20;
                        break;
                }
            }

            int crewFactors = (int)Math.Ceiling((double)this.HullValue / (double)crewFactorRatio);
            int dCPFrequency = (int)Math.Ceiling((double)this.TotalHullBoxes / (double)crewFactors);
            int hullPointAssigning = 1;
            int dCPAssigning = 1;

            for (int i = 0; i < this.HullRows; i++)
            {
                var rowStr = string.Empty;
                int thisRowLength = i < numMaxRows ? maxRowLength : minRowLength;

                for (int j = 0; j < thisRowLength; j++)
                {
                    string hullBox = "[ ]";

                    // DCP / Crew Factors eventl spread; last one needs to be assigned to the final hull box if not already assigned
                    if (hullPointAssigning % dCPFrequency == 0 || (hullPointAssigning == this.TotalHullBoxes && dCPAssigning == crewFactors))
                    {
                        hullBox = "[*]";
                        dCPAssigning++;
                    }

                    if (damageToAssign > 0)
                    {
                        hullBox = "[X]";
                        damageToAssign--;
                    }

                    hullPointAssigning++;
                    rowStr += hullBox;
                }

                hullDisplay.Add(rowStr);
            }

            return hullDisplay;
        }
    }
}