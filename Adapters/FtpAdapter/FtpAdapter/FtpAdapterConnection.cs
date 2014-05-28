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
/// Module      :  FtpAdapterConnection.cs
/// Description :  Defines the connection to the target system.
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
using System.Net.FtpClient;
using System.Net;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Ftp
{
    public class FtpAdapterConnection : IConnection
    {
        #region Private Fields

        private FtpAdapterConnectionFactory connectionFactory;
        private string connectionId;

        #endregion Private Fields

        /// <summary>
        /// Initializes a new instance of the FtpAdapterConnection class with the FtpAdapterConnectionFactory
        /// </summary>
        public FtpAdapterConnection(FtpAdapterConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            this.connectionId = Guid.NewGuid().ToString();

            Client = CreateClient();
        }

        private FtpClient CreateClient()
        {
            ProtocolType protocol = ConnectionFactory.ConnectionUri.Protocol;
            string host = ConnectionFactory.ConnectionUri.HostName;
            int port = ConnectionFactory.ConnectionUri.Port;

            var credentials = ConnectionFactory.Credentials.UserName;

            var client = new FtpClient();

            client.Host = host;
            client.Port = port;

            switch (protocol)
            {
                case ProtocolType.FTP:
                    client.EncryptionMode = FtpEncryptionMode.None;
                    break;
                case ProtocolType.FTPS:
                    client.EncryptionMode = FtpEncryptionMode.Implicit;
                    break;
            }

            client.Credentials = new NetworkCredential(credentials.UserName, credentials.Password);

            return client;
        }

        #region Public Properties

        /// <summary>
        /// Gets the ConnectionFactory
        /// </summary>
        public FtpAdapterConnectionFactory ConnectionFactory
        {
            get
            {
                return this.connectionFactory;
            }
        }

        public FtpClient Client { get; private set; }

        #endregion Public Properties

        #region IConnection Members

        /// <summary>
        /// Closes the connection to the target system
        /// </summary>
        public void Close(TimeSpan timeout)
        {
            Client.Disconnect();
        }

        /// <summary>
        /// Returns a value indicating whether the connection is still valid
        /// </summary>
        public bool IsValid(TimeSpan timeout)
        {
            return Client.IsConnected;
        }

        /// <summary>
        /// Opens the connection to the target system.
        /// </summary>
        public void Open(TimeSpan timeout)
        {
            Client.ConnectTimeout = (int)timeout.TotalMilliseconds;
            Client.Connect();
        }

        /// <summary>
        /// Clears the context of the Connection. This method is called when the connection is set back to the connection pool
        /// </summary>
        public void ClearContext()
        { }

        /// <summary>
        /// Builds a new instance of the specified IConnectionHandler type
        /// </summary>
        public TConnectionHandler BuildHandler<TConnectionHandler>(MetadataLookup metadataLookup)
             where TConnectionHandler : class, IConnectionHandler
        {

            if (typeof(IOutboundHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new FtpAdapterOutboundHandler(this, metadataLookup) as TConnectionHandler;
            }
            if (typeof(IInboundHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new FtpAdapterInboundHandler(this, metadataLookup) as TConnectionHandler;
            }

            return default(TConnectionHandler);
        }

        /// <summary>
        /// Aborts the connection to the target system
        /// </summary>
        public void Abort()
        {
            Client.Disconnect();
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
