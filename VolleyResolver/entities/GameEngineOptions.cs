using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace FireAndManeuver.GameEngine
{
    [XmlRoot("GameOptions")]
    public class GameEngineOptions
    {
        public static int RANGE_BAND_DEFAULT = 6;

    }
}