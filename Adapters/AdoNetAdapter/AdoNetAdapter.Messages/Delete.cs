using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.DELETE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class Delete<T> : List<T> { }

    [DataContract(Name = Constants.DELETE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    public class DeleteResult : Result { }

    [MessageContract(IsWrapped = false)]
    public class DeleteMessage<T>
    {
        [MessageBodyMember(Name = Constants.DELETE, Namespace = Constants.MESSAGENAMESPACE)]
        public Delete<T> Body { get; set; }
    }

    [MessageContract(IsWrapped = false)]
    public class DeleteResultMessage
    {
        [MessageBodyMember(Name = Constants.DELETE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
        public DeleteResult Body { get; set; }
    }
}
