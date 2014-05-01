using Quartz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Helpers
{
    public class ScheduleHelper : IJob
    {
        private static Dictionary<JobKey, Action> subscriptions = new Dictionary<JobKey,Action>();

        public static void RegisterEvent(string scheduleName, Action action)
        {
            var key = GetKey(scheduleName);
            subscriptions[key] = action;
        }

        public static void CancelEvent(string scheduleName)
        {
            var key = GetKey(scheduleName);
            subscriptions.Remove(key);
        }

        void IJob.Execute(IJobExecutionContext context)
        {
            var action = subscriptions[context.JobDetail.Key];

            if (action != null)
                action();
        }

        private static JobKey GetKey(string scheduleName)
        {
            var parts = scheduleName.Split('.');

            if (parts.Length == 1)
                return JobKey.Create(scheduleName);
            else
                return JobKey.Create(parts[1], parts[0]);
        }
    }
}
