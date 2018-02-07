using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FireAndManeuver.GameModel
{
    public class GameUnitRecord
    {
        [XmlIgnore] private int _volley { get; set; }

        [XmlAttribute("turn")] public int Turn { get; set; }
        [XmlAttribute("fmVolley")] public int Volley
        {
            get { if (_volley == -1 && Turn > 0) { _volley = Turn; } return _volley; }
            set { _volley = value; }
        }
        [XmlAttribute("fmExchange")] public int Exchange { get; set; }
        [XmlAttribute("event")] public string Event { get; set; }
        [XmlAttribute("priority")] public string Priority { get; set; }
        [XmlText] public string Message { get; set; }

        public GameUnitRecord()
        {
            Turn = -1;
            Volley = -1;
            Exchange = 1;
            Event = "Info";
            Priority = "Low";
            Message = "";
        }

        public GameUnitRecord(int volley, int exchange, string eventType, string priority, string message)
        {
            Volley = volley;
            Exchange = exchange;
            Event = eventType;
            Priority = priority;
            Message = message;
        }

        public override string ToString()
        {
            return $"E{Exchange,2} / V{Volley,2} - {Priority,-6} - {Event,-6} - {Message.Trim()}";
        }
    }
}
