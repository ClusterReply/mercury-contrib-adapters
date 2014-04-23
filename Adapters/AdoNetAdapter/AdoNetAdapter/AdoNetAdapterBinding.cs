/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterBinding.cs
/// Description :  This is the class used while creating a binding for an adapter
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterBinding : AdapterBinding
    {
        // Scheme in Binding does not have to be the same as Adapter Scheme. 
        // Over write this value as appropriate.
        private const string BindingScheme = "ado";

        /// <summary>
        /// Initializes a new instance of the AdapterBinding class
        /// </summary>
        public AdoNetAdapterBinding() { }

        /// <summary>
        /// Initializes a new instance of the AdapterBinding class with a configuration name
        /// </summary>
        public AdoNetAdapterBinding(string configName)
        {
            ApplyConfiguration(configName);
        }

        /// <summary>
        /// Applies the current configuration to the AdoNetAdapterBindingCollectionElement
        /// </summary>
        private void ApplyConfiguration(string configurationName)
        {
            /*
            //
            // TODO : replace the <The config name of your adapter> below with the configuration name of your adapter
            //
            BindingsSection bindingsSection = (BindingsSection)System.Configuration.ConfigurationManager.GetSection("system.serviceModel/bindings");
            AdoNetAdapterBindingCollectionElement bindingCollectionElement = (AdoNetAdapterBindingCollectionElement)bindingsSection["<The config name of your adapter>"];
            AdoNetAdapterBindingElement element = bindingCollectionElement.Bindings[configurationName];
            if (element != null)
            {
                element.ApplyConfiguration(this);
            }
            */
            throw new NotImplementedException("The method or operation is not implemented.");
        }



        #region Private Fields

        private AdoNetAdapter binding;

        #endregion Private Fields

        #region Custom Generated Fields

        private string dataAvailableStatement;


        private string getDataStatement;


        private string endOperationStatement;


        private bool useAmbientTransaction;


        private System.Transactions.IsolationLevel isolationLevel;


        private bool pollWhileDataFound;


        private int pollingInterval;

        #endregion Custom Generated Fields

        #region Public Properties

        /// <summary>
        /// Gets the URI transport scheme that is used by the channel and listener factories that are built by the bindings.
        /// </summary>
        public override string Scheme
        {
            get
            {
                return BindingScheme;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this binding supports metadata browsing.
        /// </summary>
        public override bool SupportsMetadataBrowse
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this binding supports metadata retrieval.
        /// </summary>
        public override bool SupportsMetadataGet
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this binding supports metadata searching.
        /// </summary>
        public override bool SupportsMetadataSearch
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the custom type of the ConnectionUri.
        /// </summary>
        public override Type ConnectionUriType
        {
            get
            {
                return typeof(AdoNetAdapterConnectionUri);
            }
        }

        #endregion Public Properties

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

        #endregion Custom Generated Properties

        #region Private Properties

        private AdoNetAdapter BindingElement
        {
            get
            {
                if (binding == null)
                    binding = new AdoNetAdapter();
                binding.DataAvailableStatement = this.DataAvailableStatement;
                binding.GetDataStatement = this.GetDataStatement;
                binding.EndOperationStatement = this.EndOperationStatement;
                binding.UseAmbientTransaction = this.UseAmbientTransaction;
                binding.IsolationLevel = this.IsolationLevel;
                binding.PollWhileDataFound = this.PollWhileDataFound;
                binding.PollingInterval = this.PollingInterval;
                return binding;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Creates a clone of the existing BindingElement and returns it
        /// </summary>
        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection bindingElements = new BindingElementCollection();
            //Only create once
            bindingElements.Add(this.BindingElement);
            //Return the clone
            return bindingElements.Clone();
        }

        #endregion Public Methods

    }
}
