// <copyright file="CommonExtensions.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class CommonExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                queue.Enqueue(i);
            }
        }

        public static void AddTo<T>(this IEnumerable<T> self, List<T> destination)
        {
            if (self != null)
            {
                destination.AddRange(self);
            }
        }

        public static ILogger GetLogger(this IServiceProvider services, string loggerTitle = "VolleyResolver")
        {
            return services.GetService<ILoggerFactory>().CreateLogger(loggerTitle);
        }
    }
}
