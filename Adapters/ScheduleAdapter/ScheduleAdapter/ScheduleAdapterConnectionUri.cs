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
/// Module      :  ScheduleAdapterConnectionUri.cs
/// Description :  This is the class for representing an adapter connection uri
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Schedule
{
    /// <summary>
    /// This is the class for building the ScheduleAdapterConnectionUri
    /// </summary>
    public class ScheduleAdapterConnectionUri : ConnectionUri
    {

        #region Custom Generated Fields

        private string scheduleName = null;

        #endregion Custom Generated Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class
        /// </summary>
        public ScheduleAdapterConnectionUri() { }

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class with a Uri object
        /// </summary>
        public ScheduleAdapterConnectionUri(Uri uri)
            : base()
        {
            Uri = uri;
        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.ComponentModel.Category("Schedule")]
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

        #region ConnectionUri Members

        /// <summary>
        /// Getter and Setter for the Uri
        /// </summary>
        public override Uri Uri
        {
            get
            {
                var builder = new UriBuilder(ScheduleAdapter.SCHEME, string.Empty, 0, ScheduleName);
                return builder.Uri;
            }
            set
            {
                if (value.Scheme != ScheduleAdapter.SCHEME)
                    throw new InvalidUriException(string.Format("Invalid scheme '{0}', was expected '{1}'.", value.Scheme, ScheduleAdapter.SCHEME));

                ScheduleName = value.AbsolutePath.Trim('/');
            }
        }

        #endregion ConnectionUri Members

    }
}