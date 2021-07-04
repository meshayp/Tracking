using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracking
{
    public class TimeDiff
    {
        public static DateTime LastTime;
        public static void init()
        {
            LastTime = DateTime.Now;
        }
        public static long Get()
        {
            DateTime NewNow = DateTime.Now;
            TimeSpan Diff = NewNow - LastTime;
            LastTime = NewNow;

            return (long)Diff.TotalMilliseconds;
        }
    };
}
