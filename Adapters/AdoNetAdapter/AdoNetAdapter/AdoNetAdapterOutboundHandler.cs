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
            new System.Text.RegularExpressions.Regex(@"^(?<Target>.+)#(?<Type>Execute|Create|Read|Update|Delete)$");

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
                            var command = connection.CreateCommand();

                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandText = operationTarget;

                            dynamic staticCommandBuilder = new StaticMembersDynamicWrapper(commandBuilder.GetType());
                            staticCommandBuilder.DeriveParameters(command);

                            DbHelpers.SetParameters(bodyReader.ReadSubtree(), command.Parameters);

                            // TODO: parametri in uscita

                            using (var reader = command.ExecuteReader())
                            {
                                return DbHelpers.CreateMessage(reader, responseAction);
                            }
                        }
                        else if (operationType == "Create")
                        {
                            int count = 0;
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);

                            while (bodyReader.ReadToFollowing("Row"))
                            {
                                var command = commandBuilder.GetInsertCommand();
                                DbHelpers.SetTargetParameters(bodyReader.ReadSubtree(), command.Parameters);

                                count = command.ExecuteNonQuery();
                            }

                            return DbHelpers.CreateMessage(operationType, count, action);
                        }
                        else if (operationType == "Read")
                        {
                            bodyReader.ReadToFollowing("Query");
                            string query = bodyReader.ReadString();

                            var command = connection.CreateCommand();
                            command.CommandText = query;

                            using (var reader = command.ExecuteReader())
                            {
                                return DbHelpers.CreateMessage(reader, responseAction);
                            }
                        }
                        else if (operationType == "Update")
                        {
                            int count = 0;
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);

                            while (bodyReader.ReadToFollowing("Pair"))
                            {
                                var command = commandBuilder.GetUpdateCommand();

                                bodyReader.ReadToFollowing("Before");
                                DbHelpers.SetSourceParameters(bodyReader.ReadSubtree(), command.Parameters);

                                bodyReader.ReadToFollowing("After");
                                DbHelpers.SetTargetParameters(bodyReader.ReadSubtree(), command.Parameters);

                                count += command.ExecuteNonQuery();
                            }

                            return DbHelpers.CreateMessage(operationType, count, responseAction);
                        }
                        else if (operationType == "Delete")
                        {
                            int count = 0;
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);

                            while (bodyReader.ReadToFollowing("Row"))
                            {
                                var command = commandBuilder.GetDeleteCommand();
                                DbHelpers.SetSourceParameters(bodyReader.ReadSubtree(), command.Parameters);

                                count += command.ExecuteNonQuery();
                            }

                            return DbHelpers.CreateMessage(operationType, count, responseAction);
                        }
                    }
                }
            }

            return null;
        }

        #endregion IOutboundHandler Members
    }
}
