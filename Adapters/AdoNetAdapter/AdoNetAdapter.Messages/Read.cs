using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [DataContract(Name = "Read", Namespace = Constants.MESSAGENAMESPACE)]
    public class Read
    {
        [DataMember(Name = "Query")]
        public string Query { get; set; }
    }
}
