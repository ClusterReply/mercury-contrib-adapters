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
/// Module      :  AdoNetAdapterMetadataSearchHandler.cs
/// Description :  This class is used for performing a connection-based search for metadata from the target system
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceModel.Channels;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterMetadataSearchHandler : AdoNetAdapterHandlerBase, IMetadataSearchHandler
    {
        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterMetadataSearchHandler class
        /// </summary>
        public AdoNetAdapterMetadataSearchHandler(AdoNetAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
        }

        #region IMetadataSearchHandler Members

        /// <summary>
        /// Retrieves an array of MetadataRetrievalNodes (see Microsoft.ServiceModel.Channels) from the target system.
        /// The search will begin at the path provided in absoluteName, which points to a location in the tree of metadata nodes.
        /// The contents of the array are filtered by SearchCriteria and the number of nodes returned is limited by maxChildNodes.
        /// The method should complete within the specified timespan or throw a timeout exception.  If absoluteName is null or an empty string, return nodes starting from the root.
        /// If SearchCriteria is null or an empty string, return all nodes.
        /// </summary>
        public MetadataRetrievalNode[] Search(string nodeId
            , string searchCriteria
            , int maxChildNodes, TimeSpan timeout)
        {
            //
            //TODO: Search for metadata on the target system.
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion IMetadataSearchHandler Members
    }
}
