/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterTrace.cs
/// Description :  Implements adapter tracing
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    // Use AdoNetAdapterUtilities.Trace in the code to trace the adapter
    public class AdoNetAdapterUtilities
    {
        //
        // Initializes a new instane of  Microsoft.ServiceModel.Channels.Common.AdapterTrace using the specified name for the source
        //
        static AdapterTrace trace = new AdapterTrace("AdoNetAdapter");

        /// <summary>
        /// Gets the AdapterTrace
        /// </summary>
        public static AdapterTrace Trace
        {
            get
            {
                return trace;
            }
        }

    }


}

