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

            string operation = soapAction.Replace(AdoNetAdapter.SERVICENAMESPACE + '/', string.Empty);
            var match = operationExp.Match(operation);

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

            Message result = null;

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
                            result = DbHelpers.Execute(bodyReader, connection, operationTarget, commandBuilder.GetType(), responseAction);
                        }
                        else if (operationType == "MultiExecute")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(string.Empty, connection);
                            result = DbHelpers.MultiExecute(bodyReader, connection, operationTarget, commandBuilder.GetType(), responseAction);
                        }
                        else if (operationType == "Create")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            result = DbHelpers.Create(bodyReader, connection, operationType, commandBuilder, responseAction);
                        }
                        else if (operationType == "Read")
                        {
                            result = DbHelpers.Read(bodyReader, connection, responseAction);
                        }
                        else if (operationType == "Update")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            result = DbHelpers.Update(bodyReader, connection, operationType, commandBuilder, responseAction);                            
                        }
                        else if (operationType == "Delete")
                        {
                            var commandBuilder = Connection.CreateDbCommandBuilder(operationTarget, connection);
                            result = DbHelpers.Delete(bodyReader, connection, operationType, commandBuilder, responseAction);            
                        }

                        scope.Complete();
                    }
                }
            }

            return result;
        }

        #endregion IOutboundHandler Members
    }
}
