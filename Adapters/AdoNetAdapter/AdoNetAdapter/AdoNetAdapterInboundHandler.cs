/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterInboundHandler.cs
/// Description :  This class implements an interface for listening or polling for data.
/// -----------------------------------------------------------------------------------------------------------
/// 
#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
using System.ServiceModel.Channels;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    public class AdoNetAdapterInboundHandler : AdoNetAdapterHandlerBase, IInboundHandler
    {
        /// <summary>
        /// Initializes a new instance of the AdoNetAdapterInboundHandler class
        /// </summary>
        public AdoNetAdapterInboundHandler(AdoNetAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
            pollingTimer = new System.Timers.Timer((double)connection.ConnectionFactory.Adapter.PollingInterval * 1000);
            pollingTimer.Elapsed += pollingTimer_Elapsed;

            UriBuilder actionBuilder = new UriBuilder(AdoNetAdapter.SERVICENAMESPACE);
            actionBuilder.Path = System.IO.Path.Combine(actionBuilder.Path, connection.ConnectionFactory.ConnectionUri.ConnectionName);
            actionBuilder.Path = System.IO.Path.Combine(actionBuilder.Path, "Receive");
            actionBuilder.Path = System.IO.Path.Combine(actionBuilder.Path, connection.ConnectionFactory.ConnectionUri.InboundID);

            action = actionBuilder.ToString();
        }

        #region Private Fields

        private string action;

        private System.Timers.Timer pollingTimer;
        private BlockingCollection<Message> queue = new BlockingCollection<Message>();
        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        #endregion Private Fields

        #region IInboundHandler Members

        /// <summary>
        /// Start the listener
        /// </summary>
        public void StartListener(string[] actions, TimeSpan timeout)
        {
            pollingTimer.Start();
        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public void StopListener(TimeSpan timeout)
        {
            pollingTimer.Stop();
            queue.CompleteAdding();
            cancelSource.Cancel();
        }

        /// <summary>
        /// Tries to receive a message within a specified interval of time. 
        /// </summary>
        public bool TryReceive(TimeSpan timeout, out System.ServiceModel.Channels.Message message, out IInboundReply reply)
        {
            reply = new AdoNetAdapterInboundReply();
            message = null;

            if (queue.IsCompleted)
                return false;

            return queue.TryTake(out message, (int)timeout.TotalMilliseconds, cancelSource.Token);
        }

        /// <summary>
        /// Returns a value that indicates whether a message has arrived within a specified interval of time.
        /// </summary>
        public bool WaitForMessage(TimeSpan timeout)
        {
            // TODO: vedere se fare una logica più sensata

            return !queue.IsCompleted;
        }

        #endregion IInboundHandler Members

        #region Event Handlers

        private void pollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ExecutePolling();
        }

        private void ExecutePolling()
        {
            string dataAvailableStatement = Connection.ConnectionFactory.Adapter.DataAvailableStatement;
            string getDataStatement = Connection.ConnectionFactory.Adapter.GetDataStatement;
            string endOperationStatement = Connection.ConnectionFactory.Adapter.EndOperationStatement;

            using (var connection = Connection.CreateDbConnection())
            {
                bool dataAvailable = true;

                if (!string.IsNullOrWhiteSpace(dataAvailableStatement))
                {
                    var dataAvailableCommand = connection.CreateCommand();
                    dataAvailableCommand.CommandText = dataAvailableStatement;

                    int? count = dataAvailableCommand.ExecuteScalar() as int?;
                    dataAvailable = count.HasValue && (count > 0);
                }
                
                if (dataAvailable)
                {
                    bool goOn = Connection.ConnectionFactory.Adapter.PollWhileDataFound;

                    do
                    {
                        if (queue.IsAddingCompleted)
                            return;

                        var getDataCommand = connection.CreateCommand();
                        getDataCommand.CommandText = getDataStatement;

                        var reader = getDataCommand.ExecuteReader();

                        if (reader.HasRows)
                        {
                            queue.Add(CreateMessage(reader));

                            if (!string.IsNullOrWhiteSpace(endOperationStatement))
                            {
                                var endOperationCommand = connection.CreateCommand();
                                endOperationCommand.CommandText = endOperationStatement;

                                endOperationCommand.ExecuteNonQuery();
                            }
                        }
                        else
                            goOn = false;
                    } while (goOn);
                }
            }
        }

        #endregion Event Handlers

        #region Private Members

        private Message CreateMessage(System.Data.Common.DbDataReader reader)
        {
            string ns = AdoNetAdapter.SERVICENAMESPACE + "/Messages";

            using (var stream = new System.IO.MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    writer.WriteStartElement("InboundData", ns);

                    do
                    {
                        writer.WriteStartElement("ResultSet", ns);

                        while (reader.Read())
                        {
                            writer.WriteStartElement("Row", ns);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // TODO: conversione del tipo

                                writer.WriteElementString(reader.GetName(i), ns, reader.GetString(i));
                            }
                            
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    } while (reader.NextResult());

                    writer.Flush();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    using (var xmlReader = XmlReader.Create(stream))
                        return Message.CreateMessage(MessageVersion.Default, action, xmlReader);
                }
            }
        }

        #endregion Private Members

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pollingTimer.Dispose();
            }
        }

        #endregion IDisposable Members
    }
    internal class AdoNetAdapterInboundReply : InboundReply
    {
        #region InboundReply Members

        /// <summary>
        /// Abort the inbound reply call
        /// </summary>
        public override void Abort()
        { }

        /// <summary>
        /// Reply message implemented
        /// </summary>
        public override void Reply(System.ServiceModel.Channels.Message message
            , TimeSpan timeout)
        { }


        #endregion InboundReply Members
    }

}
