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

            var inputHandler = CreateHandler(inputFolder, "*.txt");

            try
            {
                CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
            }
        }

        [Test]
        public void Internal_ReadFile_SinglePost()
        {
            var inputHandler = CreateHandler(inputFolder, "*.txt");

            try
            {
                string inputFile = Path.Combine(inputFolder, "File.txt");
                System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

                CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
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

            var inputHandler = CreateHandler(inputFolder, "*.txt");

            try
            {
                CheckFile(inputHandler, inputFile1, Properties.Resources.TestFile); 
                
                string inputFile2 = Path.Combine(inputFolder, "File2.txt");
                System.IO.File.WriteAllText(inputFile2, Properties.Resources.TestFile_Existing);

                CheckFile(inputHandler, inputFile2, Properties.Resources.TestFile_Existing);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
            }
        }

        private FileAdapterInboundHandler CreateHandler(string inputFolder, string filter)
        {
            var adapter = new FileAdapter();
            var connectionUri = new FileAdapterConnectionUri { Path = inputFolder, FileName = filter };
            var factory = new FileAdapterConnectionFactory(connectionUri, new ClientCredentials(), adapter);

            var connection = factory.CreateConnection();
            var inputHandler = connection.BuildHandler<FileAdapterInboundHandler>(null);

            inputHandler.StartListener(new string[0], TimeSpan.MaxValue);

            return inputHandler;
        }

        private void CheckFile(FileAdapterInboundHandler inputHandler, string inputFile, string expectedContent)
        {
            Message message;
            IInboundReply reply;

            Assert.IsTrue(inputHandler.TryReceive(TimeSpan.FromMinutes(1), out message, out reply));
            reply.Reply(message, TimeSpan.FromMinutes(1));

            Assert.AreEqual(new Uri(inputFile), new Uri(message.Headers.Action));

            byte[] body = message.GetBody<byte[]>();

            using (var reader = new StreamReader(new MemoryStream(body)))
                Assert.AreEqual(expectedContent, reader.ReadToEnd());

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
