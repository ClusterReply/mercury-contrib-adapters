/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterOutboundHandler.cs
/// Description :  This class is used for sending data to the target system
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterOutboundHandler : AdoNetAdapterHandlerBase, IOutboundHandler
    {
        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterOutboundHandler class
        /// </summary>
        public AdoNetAdapterOutboundHandler(AdoNetAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
        }

        #region IOutboundHandler Members

        /// <summary>
        /// Executes the request message on the target system and returns a response message.
        /// If there isn’t a response, this method should return null
        /// </summary>
        public Message Execute(Message message, TimeSpan timeout)
        {
            //
            //TODO: Implement Execute
            //
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion IOutboundHandler Members
    }
}
