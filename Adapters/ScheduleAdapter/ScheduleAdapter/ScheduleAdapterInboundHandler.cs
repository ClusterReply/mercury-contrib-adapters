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

/// -----------------------------------------------------------------------------------------------------------
/// Module      :  ScheduleAdapterInboundHandler.cs
/// Description :  This class implements an interface for listening or polling for data.
/// -----------------------------------------------------------------------------------------------------------
/// 
#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
using System.Collections.Concurrent;
using System.Threading;
using Reply.Cluster.Mercury.Adapters.Schedule.Jobs;
using Reply.Cluster.Mercury.Adapters.Helpers;
using System.ServiceModel.Channels;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Schedule
{
    public class ScheduleAdapterInboundHandler : ScheduleAdapterHandlerBase, IInboundHandler
    {
        /// <summary>
        /// Initializes a new instance of the ScheduleAdapterInboundHandler class
        /// </summary>
        public ScheduleAdapterInboundHandler(ScheduleAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
            uri = connection.ConnectionFactory.ConnectionUri.Uri;
            scheduleName = connection.ConnectionFactory.ConnectionUri.ScheduleName;

            action = string.Format("{0}/{1}#Event", ScheduleAdapter.SERVICENAMESPACE, scheduleName);

            job = Activator.CreateInstance(connection.ConnectionFactory.Adapter.JobType) as IScheduleJob;
        }

        #region Private Fields

        private Uri uri;
        private string action;
        private string scheduleName;

        private IScheduleJob job;

        private BlockingCollection<object> queue = new BlockingCollection<object>();
        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        #endregion Private Fields

        #region IInboundHandler Members

        /// <summary>
        /// Start the listener
        /// </summary>
        public void StartListener(string[] actions, TimeSpan timeout)
        {
            ScheduleHelper.RegisterEvent(scheduleName, () => queue.Add(job.Execute(uri)));
        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public void StopListener(TimeSpan timeout)
        {
            ScheduleHelper.CancelEvent(scheduleName);

            queue.CompleteAdding();
            cancelSource.Cancel();
        }

        /// <summary>
        /// Tries to receive a message within a specified interval of time. 
        /// </summary>
        public bool TryReceive(TimeSpan timeout, out System.ServiceModel.Channels.Message message, out IInboundReply reply)
        {
            reply = new ScheduleAdapterInboundReply();
            message = null;

            if (queue.IsCompleted)
                return false;

            object scheduleObject = null;
            bool result = queue.TryTake(out scheduleObject, (int)Math.Min(timeout.TotalMilliseconds, (long)int.MaxValue), cancelSource.Token);

            if (result)
            {
                if (scheduleObject == null)
                    return false;

                message = Message.CreateMessage(MessageVersion.Default, action, scheduleObject);
            }

            return result;
        }

        /// <summary>
        /// Returns a value that indicates whether a message has arrived within a specified interval of time.
        /// </summary>
        public bool WaitForMessage(TimeSpan timeout)
        {
            return true;
        }

        #endregion IInboundHandler Members
    }
    internal class ScheduleAdapterInboundReply : InboundReply
    {
        #region InboundReply Members

        /// <summary>
        /// Abort the inbound reply call
        /// </summary>
        public override void Abort()
        { }

        /// <summary>
        /// Reply message implemented
        /// </summary>
        public override void Reply(System.ServiceModel.Channels.Message message
            , TimeSpan timeout)
        { }


        #endregion InboundReply Members
    }

}
