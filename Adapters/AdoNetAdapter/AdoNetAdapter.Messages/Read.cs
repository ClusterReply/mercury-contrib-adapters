using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [DataContract(Name = Constants.READ, Namespace = Constants.MESSAGENAMESPACE)]
    public class Read
    {
        [DataMember(Name = "Query")]
        public string Query { get; set; }
    }

    [MessageContract(IsWrapped = false)]
    public class ReadMessage
    {
        [MessageBodyMember(Name = Constants.READ, Namespace = Constants.MESSAGENAMESPACE)]
        public Read Body { get; set; }
    }
}
