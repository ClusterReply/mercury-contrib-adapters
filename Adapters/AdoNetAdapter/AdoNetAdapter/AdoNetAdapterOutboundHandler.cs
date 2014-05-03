/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterOutboundHandler.cs
/// Description :  This class is used for sending data to the target system
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;
using System.Linq;

using Microsoft.ServiceModel.Channels.Common;
using System.Transactions;
using Reply.Cluster.Mercury.Adapters.Helpers;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterOutboundHandler : AdoNetAdapterHandlerBase, IOutboundHandler
    {
        private static System.Text.RegularExpressions.Regex operationExp =
            new System.Text.RegularExpressions.Regex(@"^(?<Target>.+)#(?<Type>Execute|MultiExecute|Create|Read|Update|Delete)$");

        private static Tuple<string, string> ParseAction(string soapAction)
        {
            if (!soapAction.StartsWith(AdoNetAdapter.SERVICENAMESPACE))
                throw new InvalidOperationException();

            Uri actionUri = new Uri(soapAction);
            Uri operationUri = actionUri.MakeRelativeUri(new Uri(AdoNetAdapter.SERVICENAMESPACE));

            var segments = actionUri.Segments;
            string actionId = segments[segments.Length - 1];

            var match = operationExp.Match(actionId);

            if (!match.Success)
                throw new InvalidOperationException();

            string operationType = match.Groups["Type"].Value;
            string operationTarget = match.Groups["Target"].Value;

            return new Tuple<string, string>(operationType, operationTarget);
        }

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
            string action = message.Headers.Action;
            string responseAction = action + "Response";

            var operation = ParseAction(action);

            string operationType = operation.Item1;
            string operationTarget = operation.Item2;

            using (var bodyReader = message.GetReaderAtBodyContents())
            {
                using (var connection = Connection.CreateDbConnection())
                {
                    connection.Open();

                    var scopeOptions = this.Connection.ConnectionFactory.Adapter.UseAmbientTransaction ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew;

                    using (var scope = new TransactionScope(scopeOptions, new TransactionOptions { IsolationLevel = Connection.ConnectionFactory.Adapter.IsolationLevel }))
                    {
                        if (operationType == "Execute")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(string.Empty, connection);
                            return DbHelpers.Execute(bodyReader, connection, operationTarget, commandBuilder.GetType(), responseAction);
                        }
                        else if (operationType == "MultiExecute")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(string.Empty, connection);
                            return DbHelpers.MultiExecute(bodyReader, connection, operationTarget, commandBuilder.GetType(), responseAction);
                        }
                        else if (operationType == "Create")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            return DbHelpers.Create(bodyReader, connection, operationType, commandBuilder, responseAction);
                        }
                        else if (operationType == "Read")
                        {
                            return DbHelpers.Read(bodyReader, connection, responseAction);
                        }
                        else if (operationType == "Update")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            return DbHelpers.Update(bodyReader, connection, operationType, commandBuilder, responseAction);                            
                        }
                        else if (operationType == "Delete")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            return DbHelpers.Delete(bodyReader, connection, operationType, commandBuilder, responseAction);            
                        }
                    }
                }
            }

            return null;
        }

        #endregion IOutboundHandler Members
    }
}
