// <copyright file="RegexUtils.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>


namespace FireAndManeuver.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public static class RegexUtils
    {
        public static bool WildCardMatch(string value, string expr)
        {
            var regex = "^" + System.Text.RegularExpressions.Regex.Escape(expr).Replace("/", "\\/").Replace("\\?", "\\w").Replace("\\*", "\\w*") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(value, regex);
        }
    }
}
