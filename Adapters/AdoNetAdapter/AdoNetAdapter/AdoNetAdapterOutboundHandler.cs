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

                        while (bodyReader.ReadToFollowing("Row"))
                        {
                            var command = connection.CreateCommand();

                            var rowReader = bodyReader.ReadSubtree();
                            var parameters = DbHelpers.CreateParameters(rowReader.ReadSubtree(), command, string.Empty);

                            var queryBuilder = new StringBuilder();
                            queryBuilder.AppendFormat("INSERT INTO {0} ( ", operationTarget);
                            
                            foreach (string column in parameters.Keys)
                                queryBuilder.AppendFormat("{0}, ", column);

                            queryBuilder.Remove(queryBuilder.Length - 2, 2);
                            queryBuilder.Append(" ) VALUES ( ");

                            foreach (string column in parameters.Keys)
                                queryBuilder.AppendFormat("{0}, ", parameters[column].ParameterName);

                            queryBuilder.Remove(queryBuilder.Length - 2, 2);
                            queryBuilder.Append(" )");

                            command.CommandText = queryBuilder.ToString();
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

                        while (bodyReader.ReadToFollowing("Pair"))
                        {
                            var command = connection.CreateCommand();

                            bodyReader.ReadToFollowing("Before");
                            var beforeReader = bodyReader.ReadSubtree();

                            var beforeParameters = DbHelpers.CreateParameters(beforeReader.ReadSubtree(), command, "pre");

                            bodyReader.ReadToFollowing("After");
                            var afterReader = bodyReader.ReadSubtree();

                            var afterParameters = DbHelpers.CreateParameters(afterReader.ReadSubtree(), command, "post");

                            var queryBuilder = new StringBuilder();
                            queryBuilder.AppendFormat("UPDATE TABLE {0} ", operationTarget);

                            foreach (string column in afterParameters.Keys)
                                queryBuilder.AppendFormat("SET {0} = {1}, ", column, afterParameters[column]);

                            queryBuilder.Remove(queryBuilder.Length - 2, 2);
                            queryBuilder.Append(" WHERE ");

                            foreach (string column in beforeParameters.Keys)
                                queryBuilder.AppendFormat("{0} = {1} AND ", column, beforeParameters[column]);

                            command.CommandText = queryBuilder.ToString();
                            command.Parameters.AddRange(beforeParameters.Values.ToArray());
                            command.Parameters.AddRange(afterParameters.Values.ToArray());

                            count += command.ExecuteNonQuery();
                        }

                        return DbHelpers.CreateMessage(operationType, count, action);
                    }
                    else if (operationType == "Delete")
                    {
                        int count = 0;

                        while (bodyReader.ReadToFollowing("Row"))
                        {
                            var command = connection.CreateCommand();

                            var parameters = DbHelpers.CreateParameters(bodyReader.ReadSubtree(), command, string.Empty);

                            var queryBuilder = new StringBuilder();
                            queryBuilder.AppendFormat("DELETE FROM {0} WHERE ", operationTarget);

                            foreach (string column in parameters.Keys)
                                queryBuilder.AppendFormat("{0} = {1} AND ", column, parameters[column]);

                            command.CommandText = queryBuilder.ToString();
                            command.Parameters.AddRange(parameters.Values.ToArray());

                            count += command.ExecuteNonQuery();
                        }

                        return DbHelpers.CreateMessage(operationType, count, action);
                    }
                }
            }

            return null;
        }

        #endregion IOutboundHandler Members
    }
}
