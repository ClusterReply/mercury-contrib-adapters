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
/// Module      :  AdoNetAdapterConnection.cs
/// Description :  Defines the connection to the target system.
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
using System.Data.Common;
using System.Data;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterConnection : IConnection
    {
        #region Private Fields

        private AdoNetAdapterConnectionFactory connectionFactory;
        private string connectionId;

        #endregion Private Fields

        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterConnection class with the AdoNetAdapterConnectionFactory
        /// </summary>
        public AdoNetAdapterConnection(AdoNetAdapterConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
            this.connectionId = Guid.NewGuid().ToString();
        }

        #region Public Properties

        /// <summary>
        /// Gets the ConnectionFactory
        /// </summary>
        public AdoNetAdapterConnectionFactory ConnectionFactory
        {
            get
            {
                return this.connectionFactory;
            }
        }

        #endregion Public Properties

        #region Custom Internal Properties

        internal string connectionString;
        internal DbProviderFactory providerFactory;

        #endregion Custom Private Fields

        #region IConnection Members

        /// <summary>
        /// Closes the connection to the target system
        /// </summary>
        public void Close(TimeSpan timeout) { }

        /// <summary>
        /// Returns a value indicating whether the connection is still valid
        /// </summary>
        public bool IsValid(TimeSpan timeout)
        {
            return true;
        }

        /// <summary>
        /// Opens the connection to the target system.
        /// </summary>
        public void Open(TimeSpan timeout) 
        { 
            var uri = ConnectionFactory.ConnectionUri;
            
            string name = uri.ConnectionName;
            string provider = uri.ProviderName;
            string connectionString = uri.ConnectionString;

            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(connectionString))
            {
                var csElement = System.Configuration.ConfigurationManager.ConnectionStrings[name];

                if (string.IsNullOrEmpty(provider))
                    provider = csElement.ProviderName;

                if (string.IsNullOrEmpty(connectionString))
                    connectionString = csElement.ConnectionString;
            }

            this.providerFactory = DbProviderFactories.GetFactory(provider);
            this.connectionString = connectionString;

            // TODO: gestione delle eccezioni
        }

        /// <summary>
        /// Clears the context of the Connection. This method is called when the connection is set back to the connection pool
        /// </summary>
        public void ClearContext() { }

        /// <summary>
        /// Builds a new instance of the specified IConnectionHandler type
        /// </summary>
        public TConnectionHandler BuildHandler<TConnectionHandler>(MetadataLookup metadataLookup)
             where TConnectionHandler : class, IConnectionHandler
        {

            if (typeof(IOutboundHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new AdoNetAdapterOutboundHandler(this, metadataLookup) as TConnectionHandler;
            }
            if (typeof(IInboundHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new AdoNetAdapterInboundHandler(this, metadataLookup) as TConnectionHandler;
            }
            if (typeof(IMetadataResolverHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new AdoNetAdapterMetadataResolverHandler(this, metadataLookup) as TConnectionHandler;
            }
            if (typeof(IMetadataBrowseHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new AdoNetAdapterMetadataBrowseHandler(this, metadataLookup) as TConnectionHandler;
            }
            if (typeof(IMetadataSearchHandler).IsAssignableFrom(typeof(TConnectionHandler)))
            {
                return new AdoNetAdapterMetadataSearchHandler(this, metadataLookup) as TConnectionHandler;
            }

            return default(TConnectionHandler);
        }

        /// <summary>
        /// Aborts the connection to the target system
        /// </summary>
        public void Abort() { }


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

        #region Internal Members

        internal DbConnection CreateDbConnection()
        {
            var connection = providerFactory.CreateConnection();
            connection.ConnectionString = connectionString;

            return connection;
        }

        internal DbCommandBuilder CreateDbCommandBuilder(string table, DbConnection connection)
        {
            var commandBuilder = providerFactory.CreateCommandBuilder();
            commandBuilder.DataAdapter = providerFactory.CreateDataAdapter();

            if (!string.IsNullOrWhiteSpace(table))
            {
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = string.Format("SELECT * FROM {0} WHERE 0=1", commandBuilder.QuoteIdentifier(table));
                commandBuilder.DataAdapter.SelectCommand = selectCommand;
            }

            return commandBuilder;
        }

        #endregion Internal Members
    }
}
