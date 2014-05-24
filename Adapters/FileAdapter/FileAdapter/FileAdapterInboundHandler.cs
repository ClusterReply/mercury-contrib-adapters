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
/// Module      :  FileAdapterInboundHandler.cs
/// Description :  This class implements an interface for listening or polling for data.
/// -----------------------------------------------------------------------------------------------------------
/// 
#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.ServiceModel.Channels;
using Reply.Cluster.Mercury.Adapters.Helpers;
#endregion

namespace Reply.Cluster.Mercury.Adapters.File
{
    public class FileAdapterInboundHandler : FileAdapterHandlerBase, IInboundHandler
    {
        private class FileItem
        {
            public FileItem(string path, FileStream stream)
            {
                Path = path;
                Stream = stream;
            }

            public string Path { get; private set; }
            public FileStream Stream { get; private set; }
        }

        /// <summary>
        /// Initializes a new instance of the FileAdapterInboundHandler class
        /// </summary>
        public FileAdapterInboundHandler(FileAdapterConnection connection
            , MetadataLookup metadataLookup)
            : base(connection, metadataLookup)
        {
            connectionUri = connection.ConnectionFactory.ConnectionUri;

            watcher = new FileSystemWatcher(connectionUri.Path, connectionUri.FileName);
            watcher.Changed += FileEvent;

            pollingInterval = connection.ConnectionFactory.Adapter.PollingInterval;
            timer = new Timer(new TimerCallback(t => GetFiles()));
        }

        #region Private Fields

        private FileAdapterConnectionUri connectionUri;
        private FileSystemWatcher watcher;

        private int pollingInterval;
        private Timer timer;

        private BlockingCollection<FileItem> queue = new BlockingCollection<FileItem>();
        private CancellationTokenSource cancelSource = new CancellationTokenSource();

        #endregion Private Fields

        #region IInboundHandler Members

        /// <summary>
        /// Start the listener
        /// </summary>
        public void StartListener(string[] actions, TimeSpan timeout)
        {
            timer.Change(0, pollingInterval * 1000);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public void StopListener(TimeSpan timeout)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            watcher.EnableRaisingEvents = false;

            queue.CompleteAdding();
            cancelSource.Cancel();
        }

        /// <summary>
        /// Tries to receive a message within a specified interval of time. 
        /// </summary>
        public bool TryReceive(TimeSpan timeout, out System.ServiceModel.Channels.Message message, out IInboundReply reply)
        {
            reply = null;
            message = null;

            if (queue.IsCompleted)
                return false;

            FileItem item = null;
            bool result = queue.TryTake(out item, (int)Math.Min(timeout.TotalMilliseconds, (long)int.MaxValue), cancelSource.Token);

            if (result)
            {
                message = ByteStreamMessage.CreateMessage(item.Stream);
                message.Headers.Action = new UriBuilder(item.Path).Uri.ToString();

                reply = new FileAdapterInboundReply(item.Path, item.Stream);
            }

            return result;
        }

        /// <summary>
        /// Returns a value that indicates whether a message has arrived within a specified interval of time.
        /// </summary>
        public bool WaitForMessage(TimeSpan timeout)
        {
            return true;
        }

        #endregion IInboundHandler Members

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                watcher.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Members

        private void GetFiles()
        {
            var files = Directory.GetFiles(connectionUri.Path, connectionUri.FileName);

            foreach (string file in files)
                AddFileToQueue(file);
        }

        private void FileEvent(object sender, FileSystemEventArgs e)
        {
            AddFileToQueue(e.FullPath);
        }

        private void AddFileToQueue(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Delete);

                    queue.Add(new FileItem(path, stream));
                }
            }
            catch (IOException) { }     
        }

        #endregion
    }
    internal class FileAdapterInboundReply : InboundReply
    {
        private FileStream stream;
        private string path;

        public FileAdapterInboundReply(string path, FileStream stream)
        {
            this.path = path;
            this.stream = stream;
        }

        #region InboundReply Members

        /// <summary>
        /// Abort the inbound reply call
        /// </summary>
        public override void Abort()
        {
            stream.Close();
        }

        /// <summary>
        /// Reply message implemented
        /// </summary>
        public override void Reply(System.ServiceModel.Channels.Message message
            , TimeSpan timeout)
        {
            System.IO.File.Delete(path);
            stream.Close();
        }


        #endregion InboundReply Members
    }

}
