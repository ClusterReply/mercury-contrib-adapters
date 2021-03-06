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
/// Module      :  FileAdapterHandlerBase.cs
/// Description :  This is the base class for handlers used to store common properties/helper functions
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.File
{
    public abstract class FileAdapterHandlerBase
    {
        #region Private Fields

        private FileAdapterConnection connection;
        private MetadataLookup metadataLookup;

        #endregion Private Fields

        protected FileAdapterHandlerBase(FileAdapterConnection connection
            , MetadataLookup metadataLookup)
        {
            this.connection = connection;
            this.metadataLookup = metadataLookup;
        }

        #region Public Properties

        public FileAdapterConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        public MetadataLookup MetadataLookup
        {
            get
            {
                return this.metadataLookup;
            }
        }

        #endregion Public Properties

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        protected virtual void Dispose(bool disposing)
        { }
    }
}

