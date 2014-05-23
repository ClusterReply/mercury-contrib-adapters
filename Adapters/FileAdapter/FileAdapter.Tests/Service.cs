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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.File.Tests
{
    [ServiceContract]
    internal interface IService
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Execute(Message input);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class Service : IService
    {
        BlockingCollection<MessageItem> queue = new BlockingCollection<MessageItem>();

        public Message Execute(Message input)
        {
            queue.Add(new MessageItem(input));

            return Message.CreateMessage(MessageVersion.Default, string.Empty);
        }

        public BlockingCollection<MessageItem> Queue { get { return queue; } }
    }

    internal class MessageItem
    {
        public MessageItem(Message message)
        {
            Action = message.Headers.Action;

            Stream body = message.GetBody<Stream>();

            using (var reader = new StreamReader(body))
                Data = reader.ReadToEnd();
        }

        public string Action { get; private set; }
        public string Data { get; private set; }
    }
}
