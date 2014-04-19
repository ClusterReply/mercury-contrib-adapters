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

        #region Custom Private Fields

        private DbConnection connection;

        #endregion Custom Private Fields

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

        #region IConnection Members

        /// <summary>
        /// Closes the connection to the target system
        /// </summary>
        public void Close(TimeSpan timeout)
        {
            connection.Close();
        }

        /// <summary>
        /// Returns a value indicating whether the connection is still valid
        /// </summary>
        public bool IsValid(TimeSpan timeout)
        {
            return connection.State == ConnectionState.Open;

        }

        /// <summary>
        /// Opens the connection to the target system.
        /// </summary>
        public void Open(TimeSpan timeout)
        {
            connection.Open();
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
