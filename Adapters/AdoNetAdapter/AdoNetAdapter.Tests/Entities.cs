using Reply.Cluster.Mercury.Adapters.AdoNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Tests
{
    [DataContract(Namespace = Constants.MESSAGENAMESPACE)]
    class Entity
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime? Date { get; set; }

        [DataMember]
        public int? Count { get; set; }

        [DataMember]
        public decimal? Value { get; set; }
    }

    [CollectionDataContract(Name = Constants.CREATE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    class CreateEntity : Create<Entity> { }

    [CollectionDataContract(Name = Constants.UPDATE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    class UpdateEntity : Update<Entity> { }

    [CollectionDataContract(Name = Constants.DELETE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    class DeleteEntity : Delete<Entity> { }

    [CollectionDataContract(Name = Constants.INBOUND_DATA, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    class EntityInboundData : InboundData<Entity> { }

    [DataContract(Name = "Procedure", Namespace = Constants.MESSAGENAMESPACE)]
    class ExecuteEntity : Entity { }
}
