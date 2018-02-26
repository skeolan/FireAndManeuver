// <copyright file="Constants.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.GameModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Set of known constant values for the Game Model
    /// TODO: Combine this with the GameModelOptions class.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Die roll value that meets minimum for success in a typical roll (in FT Continuum, 4).
        /// </summary>
        public const int MinRollForSuccess = 4;

        /// <summary>
        /// Unmodified die roll value that counts as a double-hit or double-success (in FT Continuum, a "natural" 6).
        /// </summary>
        public const int MinRollForDoubleSuccess = 6;

        public static readonly int DefaultStartingRange = 60;
        public static readonly int RangeShiftPerSuccess = 3;

        public static readonly string PassiveManeuverType = "Maintain";
        public static readonly string PrimaryManeuverPriority = "Primary";

        public static readonly string DefaultManeuverPriority = "Default";
        public static readonly int DefaultManeuverTargetId = 0;
        public static readonly string DefaultManeuverTarget = $"{DefaultManeuverPriority} {PassiveManeuverType}";

        public static readonly ManeuverOrder DefaultManeuverOrder = new ManeuverOrder() { ManeuverType = PassiveManeuverType, Priority = DefaultManeuverPriority, TargetID = DefaultManeuverTargetId.ToString(), TargetFormationName = DefaultManeuverTarget };
        public static readonly List<ManeuverOrder> DefaultManeuverOrders = new List<ManeuverOrder>() { DefaultManeuverOrder };
        public static readonly VolleyOrders DefaultVolleyOrders = new VolleyOrders(volley: 0, speed: 0, evasion: 0, maneuveringOrders: DefaultManeuverOrders, firingOrders: new List<FireOrder>());

        public static readonly Tuple<string, string> CloseVersusClose = new Tuple<string, string>("Close", "Close"); // Close auto-succeeds
        public static readonly Tuple<string, string> CloseVersusMaintain = new Tuple<string, string>("Close", "Maintain"); // Close - Maintain decreases Range if positive
        public static readonly Tuple<string, string> CloseVersusWithdraw = new Tuple<string, string>("Close", "Withdraw"); // Close - Withdraw decreases Range if positive

        /*
         * Maintain v Close is passive
         * Maintain v Withdraw is passive
         * Maintain v Maintain is passive
         */

        public static readonly Tuple<string, string> WithdrawVersusClose = new Tuple<string, string>("Withdraw", "Close"); // Withdraw - Close increases Range if positive
        public static readonly Tuple<string, string> WithdrawVersusMaintain = new Tuple<string, string>("Withdraw", "Maintain"); // Withdraw - Maintain increases Range if positive
        public static readonly Tuple<string, string> WithdrawVersusWithdraw = new Tuple<string, string>("Withdraw", "Withdraw"); // Withdraw auto-succeeds
    }
}
