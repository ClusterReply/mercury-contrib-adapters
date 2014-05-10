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
/// Module      :  ScheduleAdapter.cs
/// Description :  The main adapter class which inherits from Adapter
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Schedule
{
    public class ScheduleAdapter : Adapter
    {
        // Scheme associated with the adapter
        internal const string SCHEME = "schedule";
        // Namespace for the proxy that will be generated from the adapter schema
        internal const string SERVICENAMESPACE = "http://mercury.cluster.reply.eu/adapters/schedule/2014/05";
        // Initializes the AdapterEnvironmentSettings class
        private static AdapterEnvironmentSettings environmentSettings = new AdapterEnvironmentSettings();

        #region Custom Generated Fields

        private string jobType;

        #endregion Custom Generated Fields

        #region  Constructor

        /// <summary>
        /// Initializes a new instance of the ScheduleAdapter class
        /// </summary>
        public ScheduleAdapter()
            : base(environmentSettings)
        {
            Settings.Metadata.DefaultMetadataNamespace = SERVICENAMESPACE;
        }

        /// <summary>
        /// Initializes a new instance of the ScheduleAdapter class with a binding
        /// </summary>
        public ScheduleAdapter(ScheduleAdapter binding)
            : base(binding)
        {
            this.JobType = binding.JobType;
        }

        #endregion Constructor

        #region Custom Generated Properties

        [System.Configuration.ConfigurationProperty("jobType", DefaultValue = "DefaultJob")]
        public string JobType
        {
            get
            {
                return this.jobType;
            }
            set
            {
                this.jobType = value;
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
            return new ScheduleAdapterConnectionUri(uri);
        }

        /// <summary>
        /// Builds a connection factory from the ConnectionUri and ClientCredentials
        /// </summary>
        protected override IConnectionFactory BuildConnectionFactory(
            ConnectionUri connectionUri
            , ClientCredentials clientCredentials
            , System.ServiceModel.Channels.BindingContext context)
        {
            return new ScheduleAdapterConnectionFactory(connectionUri, clientCredentials, this);
        }

        /// <summary>
        /// Returns a clone of the adapter object
        /// </summary>
        protected override Adapter CloneAdapter()
        {
            return new ScheduleAdapter(this);
        }

        /// <summary>
        /// Indicates whether the provided TConnectionHandler is supported by the adapter or not
        /// </summary>
        protected override bool IsHandlerSupported<TConnectionHandler>()
        {
            return (
                  typeof(IInboundHandler) == typeof(TConnectionHandler));
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
