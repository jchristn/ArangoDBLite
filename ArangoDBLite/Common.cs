using System;
using System.Collections.Generic;
using System.Text;

namespace ArangoDBLite
{
    internal static class Common
    {
        internal static double TotalMsBetween(DateTime start, DateTime end)
        { 
            start = start.ToUniversalTime();
            end = end.ToUniversalTime();
            TimeSpan total = end - start;
            return total.TotalMilliseconds;
        } 
    }
}
