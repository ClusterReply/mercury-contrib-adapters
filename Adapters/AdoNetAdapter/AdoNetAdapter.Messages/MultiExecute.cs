using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.MULTIEXECUTE_RESULT, ItemName = Constants.INBOUND_DATA, Namespace = Constants.MESSAGENAMESPACE)]
    public class MultiExecuteResult<T> : List<InboundData<T>>, IResult { }

    [MessageContract(IsWrapped = false)]
    public class MultiExecuteResultMessage<T>
    {
        [MessageBodyMember(Name = Constants.MULTIEXECUTE_RESULT)]
        MultiExecuteResult<T> Body { get; set; }
    }
}
