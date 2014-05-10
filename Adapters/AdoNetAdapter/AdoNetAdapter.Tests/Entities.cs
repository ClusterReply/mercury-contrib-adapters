#region Copyright
/*
Copyright 2014 Cluster Reply s.r.l.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion
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
