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
using Microsoft.ServiceModel.Channels.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.File.Tests
{
    [TestFixture]
    public class Inbound
    {
        private string inputFolder = Path.Combine(Path.GetTempPath(), "FileAdapter");

        [SetUp]
        public void Initialize()
        {
            if (Directory.Exists(inputFolder))
                Directory.Delete(inputFolder, true);

            Directory.CreateDirectory(inputFolder);
        }

        [Test]
        public void Internal_ReadFile_SinglePre()
        {
            string inputFile = Path.Combine(inputFolder, "File.txt");
            System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

            var inputHandler = Internal_CreateHandler(inputFolder, "*.txt");

            try
            {
                Internal_CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
            }
        }

        [Test]
        public void Internal_ReadFile_SinglePost()
        {
            var inputHandler = Internal_CreateHandler(inputFolder, "*.txt");

            try
            {
                string inputFile = Path.Combine(inputFolder, "File.txt");
                System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

                Internal_CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
            }
        }

        [Test]
        public void Internal_ReadFile_PreAndPost()
        {
            string inputFile1 = Path.Combine(inputFolder, "File1.txt");
            System.IO.File.WriteAllText(inputFile1, Properties.Resources.TestFile);

            var inputHandler = Internal_CreateHandler(inputFolder, "*.txt");

            try
            {
                Internal_CheckFile(inputHandler, inputFile1, Properties.Resources.TestFile); 
                
                string inputFile2 = Path.Combine(inputFolder, "File2.txt");
                System.IO.File.WriteAllText(inputFile2, Properties.Resources.TestFile_Existing);

                Internal_CheckFile(inputHandler, inputFile2, Properties.Resources.TestFile_Existing);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
            }
        }

        [Test]
        public void External_ReadFile_SinglePre()
        {
            string inputFile = Path.Combine(inputFolder, "File.txt");
            System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

            var serviceHost = External_CreateService(inputFolder, "*.txt");

            try
            {
                External_CheckFile(serviceHost, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                serviceHost.Close();
            }
        }

        [Test]
        public void External_ReadFile_SinglePost()
        {
            var serviceHost = External_CreateService(inputFolder, "*.txt");

            try
            {
                string inputFile = Path.Combine(inputFolder, "File.txt");
                System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

                External_CheckFile(serviceHost, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                serviceHost.Close();
            }
        }

        [Test]
        public void External_ReadFile_PreAndPost()
        {
            string inputFile1 = Path.Combine(inputFolder, "File1.txt");
            System.IO.File.WriteAllText(inputFile1, Properties.Resources.TestFile);

            var serviceHost = External_CreateService(inputFolder, "*.txt");

            try
            {
                External_CheckFile(serviceHost, inputFile1, Properties.Resources.TestFile);

                string inputFile2 = Path.Combine(inputFolder, "File2.txt");
                System.IO.File.WriteAllText(inputFile2, Properties.Resources.TestFile_Existing);

                External_CheckFile(serviceHost, inputFile2, Properties.Resources.TestFile_Existing);
            }
            finally
            {
                serviceHost.Close();
            }
        }

        private FileAdapterInboundHandler Internal_CreateHandler(string inputFolder, string filter)
        {
            var adapter = new FileAdapter();
            var connectionUri = new FileAdapterConnectionUri { Path = inputFolder, FileName = filter };
            var factory = new FileAdapterConnectionFactory(connectionUri, new ClientCredentials(), adapter);

            var connection = factory.CreateConnection();
            var inputHandler = connection.BuildHandler<FileAdapterInboundHandler>(null);

            inputHandler.StartListener(new string[0], TimeSpan.MaxValue);

            return inputHandler;
        }

        private void Internal_CheckFile(FileAdapterInboundHandler inputHandler, string inputFile, string expectedContent)
        {
            Message message;
            IInboundReply reply;

            Assert.IsTrue(inputHandler.TryReceive(TimeSpan.FromMinutes(1), out message, out reply));
            
            Assert.AreEqual(new Uri(inputFile), new Uri(message.Headers.Action));

            Stream body = message.GetBody<Stream>();

            using (var reader = new StreamReader(body))
                Assert.AreEqual(expectedContent, reader.ReadToEnd());

            reply.Reply(message, TimeSpan.FromMinutes(1));

            Assert.IsFalse(System.IO.File.Exists(inputFile));
        }

        private ServiceHost External_CreateService(string inputFolder, string filter)
        {
            var instance = new Service();
            var host = new ServiceHost(instance);
            
            host.AddServiceEndpoint(typeof(IService), new FileAdapterBinding(), new FileAdapterConnectionUri { Path = inputFolder, FileName = filter }.Uri);
            host.Open();

            return host;
        }

        private void External_CheckFile(ServiceHost host, string inputFile, string expectedContent)
        {
            System.Threading.Thread.Sleep(1000);

            MessageItem message;
            var instance = host.SingletonInstance as Service;
            
            Assert.IsTrue(instance.Queue.TryTake(out message, TimeSpan.FromMinutes(1)));

            Assert.AreEqual(new Uri(inputFile), new Uri(message.Action));
            Assert.AreEqual(expectedContent, message.Data);

            Assert.IsFalse(System.IO.File.Exists(inputFile));
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(inputFolder))
                Directory.Delete(inputFolder, true);
        }
    }
}
