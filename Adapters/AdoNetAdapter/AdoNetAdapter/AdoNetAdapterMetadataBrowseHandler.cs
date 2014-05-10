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
/// Module      :  AdoNetAdapterMetadataBrowseHandler.cs
/// Description :  This class is used while performing a connection-based browse for metadata from the target system.
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
    public class AdoNetAdapterMetadataBrowseHandler : AdoNetAdapterHandlerBase, IMetadataBrowseHandler
    {
        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterMetadataBrowseHandler class
        /// </summary>
        public AdoNetAdapterMetadataBrowseHandler(AdoNetAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
        }

        #region IMetadataBrowseHandler Members

        /// <summary>
        /// Retrieves an array of MetadataRetrievalNodes from the target system.
        /// The browse will return nodes starting from the childStartIndex in the path provided in absoluteName, and the number of nodes returned is limited by maxChildNodes.
        /// The method should complete within the specified timespan or throw a timeout exception.
        /// If absoluteName is null or an empty string, return nodes starting from the root + childStartIndex.
        /// If childStartIndex is zero, then return starting at the node indicated by absoluteName (or the root node if absoluteName is null or empty).
        /// </summary>
        public MetadataRetrievalNode[] Browse(string nodeId
            , int childStartIndex
            , int maxChildNodes, TimeSpan timeout)
        {
            //
            //TODO: Implement the metadata browse on the target system.
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion IMetadataBrowseHandler Members
    }
}
