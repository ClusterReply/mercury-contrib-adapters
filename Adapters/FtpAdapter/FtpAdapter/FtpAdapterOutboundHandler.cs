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
/// Module      :  FtpAdapterOutboundHandler.cs
/// Description :  This class is used for sending data to the target system
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;

using Microsoft.ServiceModel.Channels.Common;
using Reply.Cluster.Mercury.Adapters.Helpers;
using System.IO;
using System.Net.FtpClient;
using System.Net;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Ftp
{
    public class FtpAdapterOutboundHandler : FtpAdapterHandlerBase, IOutboundHandler
    {
        /// <summary>
        /// Initializes a new instance of the FtpAdapterOutboundHandler class
        /// </summary>
        public FtpAdapterOutboundHandler(FtpAdapterConnection connection
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
            var generator = new NameGenerator(message,
                Connection.ConnectionFactory.ConnectionUri.Path, Connection.ConnectionFactory.ConnectionUri.FileName);

            string sourcePath = new Uri(message.Headers.Action).LocalPath;

            string targetFolder = generator.Folder;
            string targetPath = Path.Combine(targetFolder, generator.FileName).Replace('\\', '/');

            if (!targetPath.StartsWith("/"))
                targetPath = string.Concat("/", targetPath);

            var client = Connection.Client;

            if (!client.DirectoryExists(targetFolder))
                client.CreateDirectory(targetFolder);

            Stream outputStream = null;

            switch (Connection.ConnectionFactory.Adapter.OverwriteAction)
            {
                case OverwriteAction.None:
                    if (client.FileExists(targetPath))
                        throw new FtpException(string.Format("The file '{0}' already exists.", targetPath));

                    outputStream = client.OpenWrite(targetPath);
                    break;
                case OverwriteAction.Replace:
                    outputStream = client.OpenWrite(targetPath);
                    break;
                case OverwriteAction.Append:
                    outputStream = client.OpenAppend(targetPath);
                    break;
            }

            using (outputStream)
            {
                var inputStream = message.GetBody<Stream>();
                inputStream.CopyTo(outputStream);

                return Message.CreateMessage(MessageVersion.Default, string.Empty);
            }
        }

        #endregion IOutboundHandler Members
    }
}
