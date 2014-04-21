using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Helpers
{
    public class TimeoutHelper
    {
        private TimeSpan timeout;
        private DateTime creationTime;
        private bool isInfinite;

        public TimeoutHelper(TimeSpan timeout)
        {
            this.creationTime = DateTime.Now;
            this.timeout = timeout;
            if (timeout.Equals(Infinite)) this.isInfinite = true;
        }

        public static TimeSpan Infinite
        {
            get { return TimeSpan.MaxValue; }
        }

        public TimeSpan RemainingTimeout
        {
            get
            {
                if (this.isInfinite) return Infinite;
                return this.timeout.Subtract(DateTime.Now.Subtract(this.creationTime));
            }
        }

        public TimeSpan GetRemainingTimeoutAndThrowIfExpired(string exceptionMessage)
        {
            if (this.isInfinite) return Infinite;
            if (RemainingTimeout < TimeSpan.Zero)
            {
                throw new TimeoutException(exceptionMessage);
            }
            return RemainingTimeout;
        }

        public void ThrowIfTimeoutExpired(string exceptionMessage)
        {
            if (RemainingTimeout < TimeSpan.Zero)
            {
                throw new TimeoutException(exceptionMessage);
            }

        }

        public bool IsExpired
        {
            get
            {
                if (this.isInfinite) return false;
                return RemainingTimeout < TimeSpan.Zero;
            }
        }
    }
}
