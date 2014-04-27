using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSet<T> : RowCollection<T> { }

    [CollectionDataContract(ItemName = Constants.RESULT_SET, Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSetCollection<T> : List<ResultSet<T>> { }

    [DataContract(Namespace = Constants.MESSAGENAMESPACE)]
    public class Result 
    {
        [DataMember(Name = "Count")]
        public string Count { get; set; }
    }
}
