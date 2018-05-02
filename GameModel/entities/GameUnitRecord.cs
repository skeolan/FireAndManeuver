// <copyright file="GameUnitRecord.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    public class GameUnitRecord
    {
        public GameUnitRecord()
        {
            this.Turn = 0;
            this.Volley = 0;
            this.Exchange = 1;
            this.Event = "Info";
            this.Priority = "Low";
            this.Message = string.Empty;
        }

        public GameUnitRecord(int volley, int exchange, string eventType, string priority, string message)
        {
            this.Volley = volley;
            this.Exchange = exchange;
            this.Event = eventType;
            this.Priority = priority;
            this.Message = message;
        }

        [XmlAttribute("turn")]
        public int Turn { get; set; }

        [XmlAttribute("fmVolley")]
        public int Volley
        {
            get
            {
                if (this.VolleyInternal == 0 && this.Turn > 0)
                {
                    this.VolleyInternal = this.Turn;
                }

                return this.VolleyInternal;
            }

            set
            {
                this.VolleyInternal = value;
            }
        }

        [XmlAttribute("fmExchange")]
        public int Exchange { get; set; }

        [XmlAttribute("event")]
        public string Event { get; set; }

        [XmlAttribute("priority")]
        public string Priority { get; set; }

        [XmlText]
        public string Message { get; set; }

        [XmlIgnore]
        private int VolleyInternal { get; set; }

        public override string ToString()
        {
            return $"E{this.Exchange, 2} / V{this.Volley, 2} - {this.Priority, -6} - {this.Event, -6} - {this.Message.Trim()}";
        }
    }
}
