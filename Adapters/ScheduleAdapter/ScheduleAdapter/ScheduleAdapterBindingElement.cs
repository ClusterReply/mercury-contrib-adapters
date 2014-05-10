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
/// Module      :  ScheduleAdapterBindingElement.cs
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

namespace Reply.Cluster.Mercury.Adapters.Schedule
{
    public class ScheduleAdapterBindingElement : StandardBindingElement
    {
        private ConfigurationPropertyCollection properties;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ScheduleAdapterBindingElement class
        /// </summary>
        public ScheduleAdapterBindingElement()
            : base(null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the ScheduleAdapterBindingElement class with a configuration name
        /// </summary>
        public ScheduleAdapterBindingElement(string configurationName)
            : base(configurationName)
        {
        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.Configuration.ConfigurationProperty("jobType", DefaultValue = "DefaultJob")]
        public string JobType
        {
            get
            {
                return ((string)(base["JobType"]));
            }
            set
            {
                base["JobType"] = value;
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
                return typeof(ScheduleAdapterBinding);
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
            ScheduleAdapterBinding adapterBinding = (ScheduleAdapterBinding)binding;
            this["JobType"] = adapterBinding.JobType;
        }

        /// <summary>
        /// Applies the configuration
        /// </summary>
        protected override void OnApplyConfiguration(Binding binding)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");

            ScheduleAdapterBinding adapterBinding = (ScheduleAdapterBinding)binding;
            adapterBinding.JobType = (System.String)this["JobType"];
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
                    configProperties.Add(new ConfigurationProperty("JobType", typeof(System.String), "DefaultJob", null, null, ConfigurationPropertyOptions.None));
                    this.properties = configProperties;
                }
                return this.properties;
            }
        }


        #endregion StandardBindingElement Members
    }
}
