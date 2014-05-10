using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    public interface IResult { }

    [CollectionDataContract(ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSet<T> : RowCollection<T> { }

    [CollectionDataContract(ItemName = Constants.RESULT_SET, Namespace = Constants.MESSAGENAMESPACE)]
    public class ResultSetCollection<T> : List<ResultSet<T>>, IResult { }

    [DataContract(Namespace = Constants.MESSAGENAMESPACE)]
    public class Result : IResult
    {
        [DataMember(Name = "Count")]
        public string Count { get; set; }
    }
}
