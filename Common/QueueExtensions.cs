// <copyright file="QueueExtensions.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.Common
{
using System;
using System.Collections.Generic;
using System.Text;

    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                queue.Enqueue(i);
            }
        }
    }
}
