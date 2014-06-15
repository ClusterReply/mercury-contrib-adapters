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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    [CollectionDataContract(Name = Constants.DELETE, ItemName = Constants.ROW, Namespace = Constants.MESSAGENAMESPACE)]
    public class Delete<T> : List<T> 
    {
        public Delete() : base() { }
        public Delete(IEnumerable collection) : base(collection.OfType<T>()) { }
        public Delete(IEnumerable<T> collection) : base(collection) { }
    }

    [DataContract(Name = Constants.DELETE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
    public class DeleteResult : Result { }

    [MessageContract(IsWrapped = false)]
    public class DeleteMessage<T>
    {
        public DeleteMessage()
        {
            Body = new Delete<T>();
        }

        public DeleteMessage(IEnumerable collection)
        {
            Body = new Delete<T>(collection);
        }

        public DeleteMessage(IEnumerable<T> collection)
        {
            Body = new Delete<T>(collection);
        }

        [MessageBodyMember(Name = Constants.DELETE, Namespace = Constants.MESSAGENAMESPACE)]
        public Delete<T> Body { get; set; }
    }

    [MessageContract(IsWrapped = false)]
    public class DeleteResultMessage
    {
        [MessageBodyMember(Name = Constants.DELETE_RESULT, Namespace = Constants.MESSAGENAMESPACE)]
        public DeleteResult Body { get; set; }
    }
}
