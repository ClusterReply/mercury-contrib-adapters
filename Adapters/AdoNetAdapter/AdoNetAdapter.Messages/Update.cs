using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.UPDATE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class Update<T> : List<Pair<T>> { }

    [DataContract(Name = "Pair", Namespace = Constants.MESSAGENAMESPACE)]
    public class Pair<T>
    {
        [DataMember(Name= "Before")]
        public T Before { get; set; }

        [DataMember(Name = "After")]
        public T After { get; set; }
    }

    [DataContract(Name = Constants.UPDATE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    public class UpdateResult : Result { }
}
