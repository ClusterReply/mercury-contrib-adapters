using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.DELETE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class Delete<T> : List<T> { }

    [DataContract(Name = Constants.DELETE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    public class DeleteResult : Result { }
}
