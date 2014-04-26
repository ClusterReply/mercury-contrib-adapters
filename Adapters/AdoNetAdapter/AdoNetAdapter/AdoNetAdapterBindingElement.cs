/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterBindingElement.cs
/// Description :  Provides a base class for the configuration elements.
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Globalization;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterBindingElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterBindingElement class
        /// </summary>
        public AdoNetAdapterBindingElement()
            : base(null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterBindingElement class with a configuration name
        /// </summary>
        public AdoNetAdapterBindingElement(string configurationName)
            : base(configurationName)
        {
        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.Configuration.ConfigurationProperty("DataAvailableStatement")]
        [System.ComponentModel.Category("Polling Data")]
        public string DataAvailableStatement
        {
            get
            {
                return ((string)(base["DataAvailableStatement"]));
            }
            set
            {
                base["DataAvailableStatement"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("GetDataStatement")]
        [System.ComponentModel.Category("Polling Data")]
        public string GetDataStatement
        {
            get
            {
                return ((string)(base["GetDataStatement"]));
            }
            set
            {
                base["GetDataStatement"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("EndOperationStatement")]
        [System.ComponentModel.Category("Polling Data")]
        public string EndOperationStatement
        {
            get
            {
                return ((string)(base["EndOperationStatement"]));
            }
            set
            {
                base["EndOperationStatement"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("useAmbientTransaction", DefaultValue = true)]
        [System.ComponentModel.Category("Transactions")]
        public bool UseAmbientTransaction
        {
            get
            {
                return ((bool)(base["UseAmbientTransaction"]));
            }
            set
            {
                base["UseAmbientTransaction"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("isolationLevel", DefaultValue = System.Transactions.IsolationLevel.ReadCommitted)]
        [System.ComponentModel.Category("Transactions")]
        public System.Transactions.IsolationLevel IsolationLevel
        {
            get
            {
                return ((System.Transactions.IsolationLevel)(base["IsolationLevel"]));
            }
            set
            {
                base["IsolationLevel"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollWhileDataFound", DefaultValue = false)]
        [System.ComponentModel.Category("Schedule")]
        public bool PollWhileDataFound
        {
            get
            {
                return ((bool)(base["PollWhileDataFound"]));
            }
            set
            {
                base["PollWhileDataFound"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollingType", DefaultValue = PollingType.Simple)]
        [System.ComponentModel.Category("Schedule")]
        public PollingType PollingType
        {
            get
            {
                return ((PollingType)(base["PollingType"]));
            }
            set
            {
                base["PollingType"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("pollingInterval", DefaultValue = 30)]
        [System.ComponentModel.Category("Schedule")]
        public int PollingInterval
        {
            get
            {
                return ((int)(base["PollingInterval"]));
            }
            set
            {
                base["PollingInterval"] = value;
            }
        }



        [System.Configuration.ConfigurationProperty("scheduleName")]
        [System.ComponentModel.Category("Schedule")]
        public string ScheduleName
        {
            get
            {
                return ((string)(base["ScheduleName"]));
            }
            set
            {
                base["ScheduleName"] = value;
            }
        }

        #endregion Custom Generated Properties

        #region Protected Properties

        /// <summary>
        /// Gets the type of the BindingElement
        /// </summary>
        protected override Type BindingElementType
        {
            get
            {
                return typeof(AdoNetAdapterBinding);
            }
        }

        #endregion Protected Properties

        #region StandardBindingElement Members

        /// <summary>
        /// Initializes the binding with the configuration properties
        /// </summary>
        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            AdoNetAdapterBinding adapterBinding = (AdoNetAdapterBinding)binding;
            this["DataAvailableStatement"] = adapterBinding.DataAvailableStatement;
            this["GetDataStatement"] = adapterBinding.GetDataStatement;
            this["EndOperationStatement"] = adapterBinding.EndOperationStatement;
            this["UseAmbientTransaction"] = adapterBinding.UseAmbientTransaction;
            this["IsolationLevel"] = adapterBinding.IsolationLevel;
            this["PollWhileDataFound"] = adapterBinding.PollWhileDataFound;
            this["PollingType"] = adapterBinding.PollingType;
            this["PollingInterval"] = adapterBinding.PollingInterval;
            this["ScheduleName"] = adapterBinding.ScheduleName;
        }

        /// <summary>
        /// Applies the configuration
        /// </summary>
        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            AdoNetAdapterBinding adapterBinding = (AdoNetAdapterBinding)binding;
            adapterBinding.DataAvailableStatement = (System.String)this["DataAvailableStatement"];
            adapterBinding.GetDataStatement = (System.String)this["GetDataStatement"];
            adapterBinding.EndOperationStatement = (System.String)this["EndOperationStatement"];
            adapterBinding.UseAmbientTransaction = (System.Boolean)this["UseAmbientTransaction"];
            adapterBinding.IsolationLevel = (System.Transactions.IsolationLevel)this["IsolationLevel"];
            adapterBinding.PollWhileDataFound = (System.Boolean)this["PollWhileDataFound"];
            adapterBinding.PollingType = (PollingType)this["PollingType"];
            adapterBinding.PollingInterval = (System.Int32)this["PollingInterval"];
            adapterBinding.ScheduleName = (System.String)this["ScheduleName"];
        }

        /// <summary>
        /// Returns a collection of the configuration properties
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection configProperties = base.Properties;
                    configProperties.Add(new ConfigurationProperty("DataAvailableStatement", typeof(System.String), null, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("GetDataStatement", typeof(System.String), null, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("EndOperationStatement", typeof(System.String), null, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("UseAmbientTransaction", typeof(System.Boolean), true, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("IsolationLevel", typeof(System.Transactions.IsolationLevel), System.Transactions.IsolationLevel.ReadCommitted, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("PollWhileDataFound", typeof(System.Boolean), false, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("PollingType", typeof(PollingType), PollingType.Simple, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("PollingInterval", typeof(System.Int32), (System.Int32)30, null, null, ConfigurationPropertyOptions.None));
                    configProperties.Add(new ConfigurationProperty("ScheduleName", typeof(System.String), null, null, null, ConfigurationPropertyOptions.None));
                    this.properties = configProperties;
                }
                return this.properties;
            }
        }


        #endregion StandardBindingElement Members
    }
}
