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
            // TODO: differenziazione delle operazioni

            string action = message.Headers.Action;
            var operation = this.MetadataLookup.GetOperationDefinitionFromInputMessageAction(action, timeout);

            using (var connection = Connection.CreateDbConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = operation.OriginalName;

                command.Parameters.AddRange(DbHelpers.CreateParameters(message, command));

                using (var reader = command.ExecuteReader())
                {
                    return DbHelpers.CreateMessage(reader, operation.InputMessageAction);
                }
            }
        }

        #endregion IOutboundHandler Members
    }
}
