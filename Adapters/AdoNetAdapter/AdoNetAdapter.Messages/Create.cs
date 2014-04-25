using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = "Create", ItemName = "Row", Namespace = Constants.MESSAGENAMESPACE)]
    public class Create<T> : List<T> { }

    [DataContract(Name = "CreateResult", Namespace = Constants.MESSAGENAMESPACE)]
    public class CreateResult : Result { }
}
