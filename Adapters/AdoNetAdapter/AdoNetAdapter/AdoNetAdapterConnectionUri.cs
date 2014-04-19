/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterConnectionUri.cs
/// Description :  This is the class for representing an adapter connection uri
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Linq;
using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    /// <summary>
    /// This is the class for building the AdoNetAdapterConnectionUri
    /// </summary>
    public class AdoNetAdapterConnectionUri : ConnectionUri
    {
        private const string PROVIDER_KEY = "adoNetProvider";

        #region Custom Generated Fields

        private string connectionString = null;


        private string connectionName = null;


        private string providerName = null;


        private string inboundID = null;

        #endregion Custom Generated Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class
        /// </summary>
        public AdoNetAdapterConnectionUri() { }

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class with a Uri object
        /// </summary>
        public AdoNetAdapterConnectionUri(Uri uri)
            : base()
        {
            this.Uri = uri;
        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.ComponentModel.Category("Connection String")]
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }


        [System.ComponentModel.Category("IDs")]
        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
            set
            {
                this.connectionName = value;
            }
        }


        [System.ComponentModel.Category("Connection String")]
        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
            set
            {
                this.providerName = value;
            }
        }


        [System.ComponentModel.Category("IDs")]
        public string InboundID
        {
            get
            {
                return this.inboundID;
            }
            set
            {
                this.inboundID = value;
            }
        }

        #endregion Custom Generated Properties

        #region ConnectionUri Members

        /// <summary>
        /// Getter and Setter for the Uri
        /// </summary>
        public override Uri Uri
        {
            get
            {
                if (string.IsNullOrWhiteSpace(connectionName))
                    throw new InvalidOperationException("The connection name must have value.");

                var builder = new UriBuilder(AdoNetAdapter.SCHEME, connectionName);

                if (!string.IsNullOrWhiteSpace(inboundID))
                    builder.Path = inboundID;

                var queryBuilder = new System.Text.StringBuilder();

                if (!string.IsNullOrWhiteSpace(providerName))
                    queryBuilder.AppendFormat("{0}={1}&", PROVIDER_KEY, providerName);

                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    var csBuilder = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connectionString };

                    foreach (string key in csBuilder.Keys)
                        queryBuilder.AppendFormat("{0}={1}&", key, csBuilder[key]);
                }

                if (queryBuilder[queryBuilder.Length - 1] == '&')
                    queryBuilder.Remove(queryBuilder.Length - 1, 1);

                if (queryBuilder.Length > 0)
                    builder.Query = queryBuilder.ToString();

                return builder.Uri;
            }
            set
            {
                if (value.Scheme != AdoNetAdapter.SCHEME)
                    throw new InvalidUriException(string.Format("Invalid scheme '{0}', was expected '{1}'.", value.Scheme, AdoNetAdapter.SCHEME));

                connectionName = value.Host;
                inboundID = value.Segments.Where(s => (s != "/") && (s != @"\")).FirstOrDefault();

                var query = System.Web.HttpUtility.ParseQueryString(value.Query);

                providerName = query.Get(PROVIDER_KEY);

                var builder = new System.Data.Common.DbConnectionStringBuilder();

                foreach (string key in query.Keys)
                    if (string.Compare(key, PROVIDER_KEY, true) != 0)
                        builder.Add(key, query.Get(key));

                connectionString = builder.ConnectionString;
            }
        }

        #endregion ConnectionUri Members

    }
}