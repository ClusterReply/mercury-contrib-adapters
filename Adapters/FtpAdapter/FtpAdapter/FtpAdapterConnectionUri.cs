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
/// Module      :  FtpAdapterConnectionUri.cs
/// Description :  This is the class for representing an adapter connection uri
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.Ftp
{
    public enum ProtocolType
    {
        FTP,
        FTPS
    }

    /// <summary>
    /// This is the class for building the FtpAdapterConnectionUri
    /// </summary>
    public class FtpAdapterConnectionUri : ConnectionUri
    {

        #region Custom Generated Fields

        private ProtocolType protocol = ProtocolType.FTP;


        private string hostName = null;


        private int port = 21;


        private string path = null;


        private string fileName = null;

        #endregion Custom Generated Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class
        /// </summary>
        public FtpAdapterConnectionUri() { }

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class with a Uri object
        /// </summary>
        public FtpAdapterConnectionUri(Uri uri)
            : base()
        {
            Uri = uri;
        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.ComponentModel.Category("Server")]
        public ProtocolType Protocol
        {
            get
            {
                return this.protocol;
            }
            set
            {
                this.protocol = value;
            }
        }


        [System.ComponentModel.Category("Server")]
        public string HostName
        {
            get
            {
                return this.hostName;
            }
            set
            {
                this.hostName = value;
            }
        }


        [System.ComponentModel.Category("Server")]
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
            }
        }


        [System.ComponentModel.Category("Path")]
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }


        [System.ComponentModel.Category("Path")]
        public string FileName
        {
            get
            {
                return this.fileName;
            }
            set
            {
                this.fileName = value;
            }
        }

        #endregion Custom Generated Properties

        #region ConnectionUri Members

        /// <summary>
        /// Getter and Setter for the Uri
        /// </summary>
        public override Uri Uri
        {
            get
            {
                string scheme = Enum.GetName(typeof(ProtocolType), Protocol).ToLower();
                string path = System.IO.Path.Combine(Path, FileName);

                return new UriBuilder(scheme, HostName, Port, path).Uri;
            }
            set
            {
                Protocol = (ProtocolType)Enum.Parse(typeof(ProtocolType), value.Scheme.ToUpper());
                HostName = value.Host;
                Port = value.Port;
                Path = System.IO.Path.GetDirectoryName(value.AbsolutePath).Replace('\\', '/').Replace("%25", "%").TrimEnd('/');
                FileName = System.IO.Path.GetFileName(value.AbsolutePath).Replace("%25", "%");
            }
        }

        #endregion ConnectionUri Members

    }
}