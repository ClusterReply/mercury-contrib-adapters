/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterBindingElementExtensionElement.cs
/// Description :  This class is provided to surface Adapter as a binding element, so that it 
///                can be used within a user-defined WCF "Custom Binding".
///                In configuration file, it is defined under
///                <system.serviceModel>
///                  <extensions>
///                     <bindingElementExtensions>
///                         <add name="{name}" type="{this}, {assembly}"/>
///                     </bindingElementExtensions>
///                  </extensions>
///                </system.serviceModel>
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
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    public class AdoNetAdapterBindingElementExtensionElement : BindingElementExtensionElement
    {

        #region  Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AdoNetAdapterBindingElementExtensionElement()
        {
        }

        #endregion Constructor

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

        #region BindingElementExtensionElement Methods
        /// <summary>
        /// Return the type of the adapter (binding element)
        /// </summary>
        public override Type BindingElementType
        {
            get
            {
                return typeof(AdoNetAdapter);
            }
        }
        /// <summary>
        /// Returns a collection of the configuration properties
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
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
                return configProperties;
            }
        }

        /// <summary>
        /// Instantiate the adapter.
        /// </summary>
        /// <returns></returns>
        protected override BindingElement CreateBindingElement()
        {
            AdoNetAdapter adapter = new AdoNetAdapter();
            this.ApplyConfiguration(adapter);
            return adapter;
        }

        /// <summary>
        /// Apply the configuration properties to the adapter.
        /// </summary>
        /// <param name="bindingElement"></param>
        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            AdoNetAdapter adapterBinding = ((AdoNetAdapter)(bindingElement));
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
        /// Initialize the binding properties from the adapter.
        /// </summary>
        /// <param name="bindingElement"></param>
        protected override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            AdoNetAdapter adapterBinding = ((AdoNetAdapter)(bindingElement));
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
        /// Copy the properties to the custom binding
        /// </summary>
        /// <param name="from"></param>
        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            AdoNetAdapterBindingElementExtensionElement adapterBinding = ((AdoNetAdapterBindingElementExtensionElement)(from));
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

        #endregion BindingElementExtensionElement Methods
    }
}

