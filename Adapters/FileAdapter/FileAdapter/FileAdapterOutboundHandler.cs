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
/// Module      :  FileAdapterOutboundHandler.cs
/// Description :  This class is used for sending data to the target system
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;

using Microsoft.ServiceModel.Channels.Common;
using System.IO;
using Reply.Cluster.Mercury.Adapters.Helpers;
#endregion

namespace Reply.Cluster.Mercury.Adapters.File
{
    public class FileAdapterOutboundHandler : FileAdapterHandlerBase, IOutboundHandler
    {
        /// <summary>
        /// Initializes a new instance of the FileAdapterOutboundHandler class
        /// </summary>
        public FileAdapterOutboundHandler(FileAdapterConnection connection
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
            string targetPath = Path.Combine(generator.Folder, generator.FileName);

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            Stream outputStream = null;
            var inputStream = message.GetBody<Stream>();

            switch (Connection.ConnectionFactory.Adapter.OverwriteAction)
            {
                case OverwriteAction.None:
                    outputStream = System.IO.File.Open(targetPath, FileMode.CreateNew);
                    break;
                case OverwriteAction.Replace:
                    outputStream = System.IO.File.Create(targetPath);
                    break;
                case OverwriteAction.Append:
                    outputStream = System.IO.File.OpenWrite(targetPath);
                    break;
            }

            using (outputStream)
            {
                inputStream.CopyTo(outputStream);

                return Message.CreateMessage(MessageVersion.Default, string.Empty);
            }
        }

        #endregion IOutboundHandler Members
    }
}
