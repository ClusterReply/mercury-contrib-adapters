/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterConnectionUri.cs
/// Description :  This is the class for representing an adapter connection uri
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    /// <summary>
    /// This is the class for building the AdoNetAdapterConnectionUri
    /// </summary>
    public class AdoNetAdapterConnectionUri : ConnectionUri
    {

        #region Custom Generated Fields

        private string connectionString = null;


        private string connectionName = null;


        private string providerName = null;


        private string inboundID = null;

        #endregion Custom Generated Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class
        /// </summary>
        public AdoNetAdapterConnectionUri() { }

        /// <summary>
        /// Initializes a new instance of the ConnectionUri class with a Uri object
        /// </summary>
        public AdoNetAdapterConnectionUri(Uri uri)
            : base()
        {

        }

        #endregion Constructors

        #region Custom Generated Properties

        [System.ComponentModel.Category("Connection String")]
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }


        [System.ComponentModel.Category("IDs")]
        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
            set
            {
                this.connectionName = value;
            }
        }


        [System.ComponentModel.Category("Connection String")]
        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
            set
            {
                this.providerName = value;
            }
        }


        [System.ComponentModel.Category("IDs")]
        public string InboundID
        {
            get
            {
                return this.inboundID;
            }
            set
            {
                this.inboundID = value;
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
                //
                //TODO: Return the composed uri in valid format
                //
                throw new NotImplementedException("The method or operation is not implemented.");
            }
            set
            {
                //
                //TODO: Parse the uri into its relevant parts to produce a valid Uri object. (For example scheme, host, query).
                //
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }

        #endregion ConnectionUri Members

    }
}