using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.INBOUND_DATA, ItemName = Constants.RESULT_SET, Namespace = Constants.MESSAGENAMESPACE)]
    public class InboundData<T> : ResultSetCollection<T> { }
}
