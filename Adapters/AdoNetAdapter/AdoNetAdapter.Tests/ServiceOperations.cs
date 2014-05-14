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
using MbUnit.Framework;
using Reply.Cluster.Mercury.Adapters.AdoNet.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Tests
{
    [TestFixture]
    public class ServiceOperations
    {
        [Test]
        public void InsertTyped()
        {
            var factory = new ChannelFactory<ISqlService>("test");
            var proxy = factory.CreateChannel();

            //var request = Message.CreateMessage(MessageVersion.Default, "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/TestTable#Create",
            //    new Create<Entity>
            //    { 
            //        new Entity  { Id = Guid.NewGuid(), Name = "A", Value = 10, Count = 2, Date = "2014-05-05T16:18:00" },
            //        new Entity  { Id = Guid.NewGuid(), Name = "B", Value = 5, Count = 1, Date = "2014-05-05" }
            //    });
            //var request = Message.CreateMessage(MessageVersion.Default, "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/TestInsert#Execute",
            //    new Entity { Id = Guid.NewGuid(), Name = "B", Value = 5, Count = 1, Date = "2014-05-05" });
            //var response = proxy.Execute(request);
            //var result = new XmlDocument();
            //result.Load(response.GetReaderAtBodyContents());

            var response = proxy.InsertEntities(new CreateMessage<Entity>
            {
                Body = new Create<Entity>
                { 
                    new Entity  { Id = Guid.NewGuid(), Name = "A", Value = 10, Count = 2, Date = "2014-05-05T16:18:00" },
                    new Entity  { Id = Guid.NewGuid(), Name = "B", Value = 5, Count = 1, Date = "2014-05-05" }
                }
            });
        }

        [Test]
        public void InsertUntyped()
        {
            var factory = new ChannelFactory<ISqlService>("test");
            var proxy = factory.CreateChannel();

            var request = Message.CreateMessage(MessageVersion.Default, "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/TestTable#Create",
                new Create<Entity>
                { 
                    new Entity  { Id = Guid.NewGuid(), Name = "A", Value = 10, Count = 2, Date = "2014-05-05T16:18:00" },
                    new Entity  { Id = Guid.NewGuid(), Name = "B", Value = 5, Count = 1, Date = "2014-05-05" }
                });
            
            var response = proxy.Execute(request);
        }

        [Test]
        public void ExecuteUntyped()
        {
            var factory = new ChannelFactory<ISqlService>("test");
            var proxy = factory.CreateChannel();

            var request = Message.CreateMessage(MessageVersion.Default, "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/TestInsert#Execute",
                new Entity { Id = Guid.NewGuid(), Name = "B", Value = 5, Count = 1, Date = "2014-05-05" });
            var response = proxy.Execute(request);
        }
    }
}
