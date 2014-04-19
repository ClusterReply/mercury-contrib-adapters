/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterConnectionFactory.cs
/// Description :  Defines the connection factory for the target system.
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Selectors;
using System.ServiceModel.Description;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterConnectionFactory : IConnectionFactory
    {
        #region Private Fields

        // Stores the client credentials
        private ClientCredentials clientCredentials;
        // Stores the adapter class
        private AdoNetAdapter adapter;

        #endregion Private Fields

        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterConnectionFactory class
        /// </summary>
        public AdoNetAdapterConnectionFactory(ConnectionUri connectionUri
            , ClientCredentials clientCredentials
            , AdoNetAdapter adapter)
        {
            this.clientCredentials = clientCredentials;
            this.adapter = adapter;
        }

        #region Public Properties

        /// <summary>
        /// Gets the adapter
        /// </summary>
        public AdoNetAdapter Adapter
        {
            get
            {
                return this.adapter;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Creates the connection to the target system
        /// </summary>
        public IConnection CreateConnection()
        {
            return new AdoNetAdapterConnection(this);
        }

        #endregion Public Methods
    }
}
