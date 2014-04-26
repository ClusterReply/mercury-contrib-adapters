using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Helpers
{
    public class ScheduleHelper : IJob
    {
        private string scheduleName;
        private Action callback;

        public ScheduleHelper(string scheduleName, Action callback)
        {
            this.scheduleName = scheduleName;
            this.callback = callback;
        }

        public void Start() { }

        public void Stop() { }

        void IJob.Execute(IJobExecutionContext context)
        {
            callback();
        }
    }
}
