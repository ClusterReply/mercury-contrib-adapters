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
/// Module      :  FileAdapterConnectionUri.cs
/// Description :  This is the class for representing an adapter connection uri
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.File
{
    /// <summary>
    /// This is the class for building the FileAdapterConnectionUri
    /// </summary>
    public class FileAdapterConnectionUri : ConnectionUri
    {

        #region Custom Generated Fields

        private string path = string.Empty;


        private string fileName = "*.*";

        #endregion Custom Generated Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class
        /// </summary>
        public FileAdapterConnectionUri() { }

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class with a Uri object
        /// </summary>
        public FileAdapterConnectionUri(Uri uri)
            : base()
        {
            this.Uri = uri;
        }

        #endregion Constructors

        #region Custom Generated Properties

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
                var builder = new UriBuilder(System.IO.Path.Combine(Path, FileName));
                return builder.Uri;
            }
            set
            {
                if (!value.IsFile)
                    throw new InvalidUriException("This is not a valid file URI.");

                Path = System.IO.Path.GetDirectoryName(value.LocalPath);
                FileName = System.IO.Path.GetFileName(value.LocalPath);
            }
        }

        #endregion ConnectionUri Members

    }
}