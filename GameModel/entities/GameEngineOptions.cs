using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    [XmlRoot("GameOptions")]
    public class GameEngineOptions
    {
        public static int RANGE_BAND_DEFAULT = 6;

        internal static GameEngineOptions Clone(GameEngineOptions gameOptions)
        {
            //Clone with all primitive types set to the same value
            GameEngineOptions newOpt = (GameEngineOptions)gameOptions.MemberwiseClone();
            
            //No non-static, non-primitive options supported yet!
            

            return newOpt;
        }
    }
}