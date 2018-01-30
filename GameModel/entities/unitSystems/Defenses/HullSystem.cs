using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public class HullSystem : DefenseSystem
    {

        [XmlAttribute] public int value { get; set; }
        [XmlAttribute] public HullTypeLookup type { get; set; }
        [XmlAttribute("class")] public string hullClass { get; set; }="Military";

        public HullSystem()
        {
            rows = 4;
            SystemName = "Hull System";
        }

        [XmlAttribute]
        public int totalHullBoxes
        {
            get
            {
                //if (type == HullTypeLookup.Custom) return _totalHullBoxes;
                if (_totalHullBoxes != -1) return _totalHullBoxes;
                else
                {
                    //Automatically rounds DOWN?
                    return (int)(this.value * this.hullTypeMultiplier);
                }
            }
            set
            {
                this._totalHullBoxes = value;
            }
        }

        private int _totalHullBoxes = -1;
        private int _remainingHullBoxes = -1;

        [XmlIgnore] private decimal hullTypeMultiplier { get { return (int)type / (decimal)100.0; } }
        [XmlAttribute] public int rows { get; set; }
        [XmlAttribute] public int remainingHullBoxes 
        { 
            get
            {
                if(_remainingHullBoxes != -1) return _remainingHullBoxes;
                else return totalHullBoxes;
            } 
            set
            {
                _remainingHullBoxes = value;
            }
        }


        public override string ToString()
        {
            string typeDetail = type == HullTypeLookup.Custom ? "Custom" : string.Format("{0} [TMFx{1}]", System.Enum.GetName(typeof(HullTypeLookup), type), hullTypeMultiplier);
            return string.Format("{0, 3} / {1, 3} - {2} {3} Hull ({4} rows)", remainingHullBoxes, totalHullBoxes, typeDetail, hullClass, rows);
        }

        public List<string> HullDisplay()
        {
            List<String> hullDisplay = new List<String>();
            int minRowLength = totalHullBoxes / rows;
            int maxRowLength = minRowLength + 1;
            int numMaxRows = totalHullBoxes - (rows * minRowLength);

            int damageToAssign = totalHullBoxes - remainingHullBoxes;
            
            //TODO: extract crewFactorRatio, crewFactors, and DCPFrequency out into properties of Hull
            int crewFactorRatio;
            switch(hullClass.ToLowerInvariant())
            {
                case "civilian" : { crewFactorRatio = 50; break; }
                case "merchant" : { crewFactorRatio = 50; break; }
                case "military" : { crewFactorRatio = 20; break; }
                default: { crewFactorRatio = 20; break; }
            }
            int crewFactors  = (int)Math.Ceiling((double)value / (double)crewFactorRatio);
            int DCPFrequency = (int)Math.Ceiling((double)totalHullBoxes / (double)crewFactors);
            int hullPointAssigning = 1;
            int DCPAssigning = 1;

            for(int i=0; i<rows; i++)
            {
                var rowStr = "";
                int thisRowLength = i<numMaxRows ? maxRowLength : minRowLength;

                for(int j=0; j<thisRowLength; j++)
                {
                    string hullBox = "[ ]";                    
                    //DCP / Crew Factors eventl spread; last one needs to be assigned to the final hull box if not already assigned
                    if(hullPointAssigning % DCPFrequency == 0 || (hullPointAssigning==totalHullBoxes && DCPAssigning==crewFactors))
                    {
                        hullBox = "[*]";
                        DCPAssigning++;
                    }
                    if(damageToAssign > 0)
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