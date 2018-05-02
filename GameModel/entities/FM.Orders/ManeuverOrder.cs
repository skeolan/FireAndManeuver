// <copyright file="ManeuverOrder.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;

    [XmlRoot("Maneuver")]
    public class ManeuverOrder : FormationOrder
    {
        public ManeuverOrder()
        {
        }

        [XmlAttribute("type")]
        public string ManeuverType { get; set; } = "Maintain";

        public static int GetManeuverModifier(string priority)
        {
            return priority.ToLowerInvariant() == Constants.PrimaryManeuverPriority.ToLowerInvariant() ? 0 : -1;
        }

        public override string ToString() => $"{this.ManeuverType}  - {base.ToString()}";

        // TODO: Move this to the distanceGraph class
        public int CalculateRangeShift(int currentVolley, GameFormation source, int sourceSpeed, GameFormation target)
        {
            var type = this.ManeuverType;
            var priority = this.Priority;
            var speed = Math.Max(0, sourceSpeed + GetManeuverModifier(this.Priority));

            var log = $"   --- Execute [{source.FormationId}]{source.FormationName} : {priority} {type} ({speed}s)";

            var targetOrders = target.Orders
                .Where(tO => tO.Volley == currentVolley)
                .FirstOrDefault() ?? Constants.DefaultVolleyOrders;
            var targetManeuver = targetOrders.ManeuveringOrders
                .Where(tRO => tRO.TargetID == source.FormationId)
                .FirstOrDefault() ?? Constants.DefaultManeuverOrder;

            var opposingPriority = targetManeuver.Priority;
            var opposingType = targetManeuver.ManeuverType;
            var opposingSpeed = Math.Max(0, targetOrders.SpeedSuccesses + GetManeuverModifier(targetManeuver.Priority));

            log += $" vs [{target.FormationId}]{target.FormationName} : {opposingPriority} {opposingType} ({opposingSpeed}s) ";

            var margin = speed - opposingSpeed;
            var rangeShift = 0;
            var successDistance = this.ManeuverType == "Close" ? -Constants.RangeShiftPerSuccess : Constants.RangeShiftPerSuccess;

            var maneuverTypePair = new Tuple<string, string>(this.ManeuverType, targetManeuver.ManeuverType);

            // Unopposed auto-successes
            if (maneuverTypePair.Equals(Constants.CloseVersusClose) || maneuverTypePair.Equals(Constants.WithdrawVersusWithdraw))
            {
                // Ignore opponent's Speed successes, they will be resolved separately
                rangeShift = successDistance * speed;
            }

            // "True" versus test -- if active F got more successes, distance changes; otherwise not.
            if (maneuverTypePair.Equals(Constants.CloseVersusMaintain) || maneuverTypePair.Equals(Constants.CloseVersusWithdraw) || maneuverTypePair.Equals(Constants.WithdrawVersusClose) || maneuverTypePair.Equals(Constants.WithdrawVersusMaintain))
            {
                rangeShift = successDistance * Math.Max(0, margin);

                // Ignore cases where opponent's successes were higher, they will be resolved separately (if not a Maintain already)
            }

            // Console.WriteLine(log);
            return rangeShift;
        }
    }
}