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
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterOutboundHandler : AdoNetAdapterHandlerBase, IOutboundHandler
    {
        private static System.Text.RegularExpressions.Regex operationExp = 
            new System.Text.RegularExpressions.Regex(@"^(?<Type>Procedure|Create|Read|Update|Delete)/(?<Target>.+)$");

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
            var operation = this.MetadataLookup.GetOperationDefinitionFromInputMessageAction(action, timeout);

            var match = operationExp.Match(operation.UniqueId);

            if (!match.Success)
                throw new InvalidOperationException();

            string operationType = match.Groups["Type"].Value;
            string operationTarget = match.Groups["Target"].Value;

            using (var bodyReader = message.GetReaderAtBodyContents())
            {
                using (var connection = Connection.CreateDbConnection())
                {
                    connection.Open();

                    var scopeOptions = this.Connection.ConnectionFactory.Adapter.UseAmbientTransaction ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew;

                    using (var scope = new TransactionScope(scopeOptions, new TransactionOptions { IsolationLevel = Connection.ConnectionFactory.Adapter.IsolationLevel }))
                    {
                        if (operationType == "Procedure")
                        {
                            var command = connection.CreateCommand();

                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.CommandText = operationTarget;

                            bodyReader.MoveToContent();

                            var parameters = DbHelpers.CreateParameters(bodyReader.ReadSubtree(), command, string.Empty);

                            command.Parameters.AddRange(parameters.Values.ToArray());

                            // TODO: parametri in uscita

                            using (var reader = command.ExecuteReader())
                            {
                                return DbHelpers.CreateMessage(reader, operation.InputMessageAction);
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
                                return DbHelpers.CreateMessage(reader, operation.InputMessageAction);
                            }
                        }
                        else if (operationType == "Update")
                        {
                            int count = 0;
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);

                            while (bodyReader.ReadToFollowing("Pair"))
                            {
                                var command = commandBuilder.GetInsertCommand();
                                DbHelpers.SetSourceParameters(bodyReader.ReadSubtree(), command.Parameters);
                                DbHelpers.SetTargetParameters(bodyReader.ReadSubtree(), command.Parameters);

                                count += command.ExecuteNonQuery();
                            }

                            return DbHelpers.CreateMessage(operationType, count, action);
                        }
                        else if (operationType == "Delete")
                        {
                            int count = 0;
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);

                            while (bodyReader.ReadToFollowing("Row"))
                            {
                                var command = commandBuilder.GetInsertCommand();
                                DbHelpers.SetSourceParameters(bodyReader.ReadSubtree(), command.Parameters);

                                count += command.ExecuteNonQuery();
                            }

                            return DbHelpers.CreateMessage(operationType, count, action);
                        }
                    }
                }
            }

            return null;
        }

        #endregion IOutboundHandler Members
    }
}
