/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapter.cs
/// Description :  The main adapter class which inherits from Adapter
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public enum PollingType
    {
        Simple,
        Managed
    }

    public class AdoNetAdapter : Adapter
    {
        // Scheme associated with the adapter
        internal const string SCHEME = "ado";
        // Namespace for the proxy that will be generated from the adapter schema
        internal const string SERVICENAMESPACE = "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04";
        internal const string MESSAGENAMESPACE = "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/Messages";
        // Initializes the AdapterEnvironmentSettings class
        private static AdapterEnvironmentSettings environmentSettings = new AdapterEnvironmentSettings();

        #region Custom Generated Fields

        private string dataAvailableStatement;


        private string getDataStatement;


        private string endOperationStatement;


        private bool useAmbientTransaction;


        private System.Transactions.IsolationLevel isolationLevel;


        private bool pollWhileDataFound;


        private PollingType pollingType;


        private int pollingInterval;
        
        
        private string scheduleName;

        #endregion Custom Generated Fields

        #region  Constructor

        /// <summary>
        /// Initializes a new instance of the AdoNetAdapter class
        /// </summary>
        public AdoNetAdapter()
            : base(environmentSettings)
        {
            Settings.Metadata.DefaultMetadataNamespace = SERVICENAMESPACE;
        }

        /// <summary>
        /// Initializes a new instance of the AdoNetAdapter class with a binding
        /// </summary>
        public AdoNetAdapter(AdoNetAdapter binding)
            : base(binding)
        {
            this.DataAvailableStatement = binding.DataAvailableStatement;
            this.GetDataStatement = binding.GetDataStatement;
            this.EndOperationStatement = binding.EndOperationStatement;
            this.UseAmbientTransaction = binding.UseAmbientTransaction;
            this.isolationLevel = binding.isolationLevel;
            this.PollWhileDataFound = binding.PollWhileDataFound;
            this.PollingType = binding.PollingType;
            this.PollingInterval = binding.PollingInterval;
            this.ScheduleName = binding.ScheduleName;

            this.Settings.Messaging.SupportsTransactedInbound = false;
        }

        #endregion Constructor

        #region Custom Generated Properties

        [System.Configuration.ConfigurationProperty("DataAvailableStatement")]
        public string DataAvailableStatement
        {
            get
            {
                return this.dataAvailableStatement;
            }
            set
            {
                this.dataAvailableStatement = value;
            }
        }



        [System.Configuration.ConfigurationProperty("GetDataStatement")]
        public string GetDataStatement
        {
            get
            {
                return this.getDataStatement;
            }
            set
            {
                this.getDataStatement = value;
            }
        }



        [System.Configuration.ConfigurationProperty("EndOperationStatement")]
        public string EndOperationStatement
        {
            get
            {
                return this.endOperationStatement;
            }
            set
            {
                this.endOperationStatement = value;
            }
        }



        [System.Configuration.ConfigurationProperty("useAmbientTransaction", DefaultValue = true)]
        public bool UseAmbientTransaction
        {
            get
            {
                return this.useAmbientTransaction;
            }
            set
            {
                this.useAmbientTransaction = value;
            }
        }



        [System.Configuration.ConfigurationProperty("isolationLevel", DefaultValue = System.Transactions.IsolationLevel.ReadCommitted)]
        public System.Transactions.IsolationLevel IsolationLevel
        {
            get
            {
                return this.isolationLevel;
            }
            set
            {
                this.isolationLevel = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollWhileDataFound", DefaultValue = false)]
        public bool PollWhileDataFound
        {
            get
            {
                return this.pollWhileDataFound;
            }
            set
            {
                this.pollWhileDataFound = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollingType", DefaultValue = PollingType.Simple)]
        public PollingType PollingType
        {
            get
            {
                return this.pollingType;
            }
            set
            {
                this.pollingType = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollingInterval", DefaultValue = 30)]
        public int PollingInterval
        {
            get
            {
                return this.pollingInterval;
            }
            set
            {
                this.pollingInterval = value;
            }
        }

        
        
        [System.Configuration.ConfigurationProperty("scheduleName")]
        public string ScheduleName
        {
            get
            {
                return this.scheduleName;
            }
            set
            {
                this.scheduleName = value;
            }
        }

        #endregion Custom Generated Properties

        #region Public Properties

        /// <summary>
        /// Gets the URI transport scheme that is used by the adapter
        /// </summary>
        public override string Scheme
        {
            get
            {
                return SCHEME;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Creates a ConnectionUri instance from the provided Uri
        /// </summary>
        protected override ConnectionUri BuildConnectionUri(Uri uri)
        {
            return new AdoNetAdapterConnectionUri(uri);
        }

        /// <summary>
        /// Builds a connection factory from the ConnectionUri and ClientCredentials
        /// </summary>
        protected override IConnectionFactory BuildConnectionFactory(
            ConnectionUri connectionUri
            , ClientCredentials clientCredentials
            , System.ServiceModel.Channels.BindingContext context)
        {
            return new AdoNetAdapterConnectionFactory(connectionUri, clientCredentials, this);
        }

        /// <summary>
        /// Returns a clone of the adapter object
        /// </summary>
        protected override Adapter CloneAdapter()
        {
            return new AdoNetAdapter(this);
        }

        /// <summary>
        /// Indicates whether the provided TConnectionHandler is supported by the adapter or not
        /// </summary>
        protected override bool IsHandlerSupported<TConnectionHandler>()
        {
            return (
                  typeof(IOutboundHandler) == typeof(TConnectionHandler)
                || typeof(IInboundHandler) == typeof(TConnectionHandler)
                || typeof(IMetadataResolverHandler) == typeof(TConnectionHandler)
                || typeof(IMetadataBrowseHandler) == typeof(TConnectionHandler)
                || typeof(IMetadataSearchHandler) == typeof(TConnectionHandler));
        }

        /// <summary>
        /// Gets the namespace that is used when generating schema and WSDL
        /// </summary>
        protected override string Namespace
        {
            get
            {
                return SERVICENAMESPACE;
            }
        }

        #endregion Protected Methods
    }
}
