using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.CREATE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class Create<T> : List<T> { }

    [DataContract(Name = Constants.CREATE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    public class CreateResult : Result { }

    [MessageContract(IsWrapped = false)]
    public class CreateMessage<T>
    {
        [MessageBodyMember(Name = Constants.CREATE, Namespace = Constants.MESSAGENAMESPACE)]
        public Create<T> Body { get; set; }
    }

    [MessageContract(IsWrapped = false)]
    public class CreateResultMessage
    {
        [MessageBodyMember(Name = Constants.CREATE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
        public CreateResult Body { get; set; }
    }
}
