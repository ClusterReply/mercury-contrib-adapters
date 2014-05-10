using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(ItemName = Constants.RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    [KnownType(typeof(CreateResult))]
    [KnownType(typeof(UpdateResult))]
    [KnownType(typeof(DeleteResult))]
    public class CompositeResult : List<IResult> { }
}
