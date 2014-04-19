/// -----------------------------------------------------------------------------------------------------------
/// Module      :  AdoNetAdapterBindingCollectionElement.cs
/// Description :  Binding Collection Element class which implements the StandardBindingCollectionElement
/// -----------------------------------------------------------------------------------------------------------

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;

using Microsoft.ServiceModel.Channels.Common;
#endregion

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    /// <summary>
    /// Initializes a new instance of the AdoNetAdapterBindingCollectionElement class
    /// </summary>
    public class AdoNetAdapterBindingCollectionElement : StandardBindingCollectionElement<AdoNetAdapterBinding,
        AdoNetAdapterBindingElement>
    {
    }
}

