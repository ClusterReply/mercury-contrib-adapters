using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(ItemName = "Row", Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSet<T> : RowCollection<T> { }

    [CollectionDataContract(ItemName = "ResultSet", Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSetCollection<T> : List<ResultSet<T>> { }

    [DataContract(Name = "Result", Namespace = Constants.MESSAGENAMESPACE)]
    public class Result 
    {
        [DataMember(Name = "Count")]
        public string Count { get; set; }
    }
}
