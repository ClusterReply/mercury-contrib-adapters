#region Copyright
/*
Copyright 2014 Cluster Reply s.r.l.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion
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
