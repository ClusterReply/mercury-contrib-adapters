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
/// Module      :  ScheduleAdapterConnection.cs
/// Description :  Defines the connection to the target system.
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Schedule
{
    public class ScheduleAdapterConnection : IConnection
    {
        #region Private Fields

        private ScheduleAdapterConnectionFactory connectionFactory;
        private string connectionId;

        #endregion Private Fields

        /// <summary>
        /// Initializes a new instance of the ScheduleAdapterConnection class with the ScheduleAdapterConnectionFactory
        /// </summary>
        public ScheduleAdapterConnection(ScheduleAdapterConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            this.connectionId = Guid.NewGuid().ToString();
        }

        #region Public Properties

        /// <summary>
        /// Gets the ConnectionFactory
        /// </summary>
        public ScheduleAdapterConnectionFactory ConnectionFactory
        {
            get
            {
                return this.connectionFactory;
            }
        }

        #endregion Public Properties

        #region IConnection Members

        /// <summary>
        /// Closes the connection to the target system
        /// </summary>
        public void Close(TimeSpan timeout)
        {
            //
            //TODO: Implement physical closing of the connection
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns a value indicating whether the connection is still valid
        /// </summary>
        public bool IsValid(TimeSpan timeout)
        {
            //
            //TODO: Implement physical checking for the validity of the opened connection
            //
            throw new NotImplementedException("The method or operation is not implemented.");

        }

        /// <summary>
        /// Opens the connection to the target system.
        /// </summary>
        public void Open(TimeSpan timeout)
        {
            //
            //TODO: Implement physical opening of the connection
            //
            throw new NotImplementedException("The method or operation is not implemented.");

        }

        /// <summary>
        /// Clears the context of the Connection. This method is called when the connection is set back to the connection pool
        /// </summary>
        public void ClearContext()
        {
            //
            //TODO: Implement clear context to set the connection back to the pool.
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Builds a new instance of the specified IConnectionHandler type
        /// </summary>
        public TConnectionHandler BuildHandler<TConnectionHandler>(MetadataLookup metadataLookup)
             where TConnectionHandler : class, IConnectionHandler
        {

            if (typeof(IInboundHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new ScheduleAdapterInboundHandler(this, metadataLookup) as TConnectionHandler;
            }

            return default(TConnectionHandler);
        }

        /// <summary>
        /// Aborts the connection to the target system
        /// </summary>
        public void Abort()
        {
            //
            //TODO: Implement abort logic. DO NOT throw an exception from this method
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }


        /// <summary>
        /// Gets the Id of the Connection
        /// </summary>
        public String ConnectionId
        {
            get
            {
                return connectionId;
            }
        }

        #endregion IConnection Members
    }
}
