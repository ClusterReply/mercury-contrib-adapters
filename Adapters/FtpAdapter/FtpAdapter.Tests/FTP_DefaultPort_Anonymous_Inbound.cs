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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Ftp.Tests
{
    [TestFixture]
    public class FTP_DefaultPort_Anonymous_Inbound
    {
        private static string directory = "FtpAdapter";
        private static string rootFolder = Path.GetTempPath();
        private string inputFolder = Path.Combine(rootFolder, directory);
        private string ftpSeverPath = Path.Combine(AssemblyDirectory, "sfk171.exe");

        private Process ftpServer;

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [FixtureSetUp]
        public void StartServer()
        {
            //Debugger.Launch();

            var processInfo = new ProcessStartInfo(ftpSeverPath, "ftpserv -rw") { WorkingDirectory = rootFolder };

            ftpServer = Process.Start(processInfo);
        }

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

            IConnection connection;
            var inputHandler = Internal_CreateHandler(directory, "*.txt", out connection);

            try
            {
                Internal_CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
                connection.Close(TimeSpan.FromSeconds(30));
            }
        }

        [Test]
        public void Internal_ReadFile_SinglePost()
        {
            IConnection connection;
            var inputHandler = Internal_CreateHandler(directory, "*.txt", out connection);

            try
            {
                string inputFile = Path.Combine(inputFolder, "File.txt");
                System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

                Internal_CheckFile(inputHandler, inputFile, Properties.Resources.TestFile);
            }
            finally
            {
                inputHandler.StopListener(TimeSpan.MaxValue);
                connection.Close(TimeSpan.FromSeconds(30));
            }
        }

        [Test]
        public void Internal_ReadFile_PreAndPost()
        {
            string inputFile1 = Path.Combine(inputFolder, "File1.txt");
            System.IO.File.WriteAllText(inputFile1, Properties.Resources.TestFile);

            IConnection connection;
            var inputHandler = Internal_CreateHandler(directory, "*.txt", out connection);

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
                connection.Close(TimeSpan.FromSeconds(30));
            }
        }

        [Test]
        public void External_ReadFile_SinglePre()
        {
            string inputFile = Path.Combine(inputFolder, "File.txt");
            System.IO.File.WriteAllText(inputFile, Properties.Resources.TestFile);

            var serviceHost = External_CreateService(directory, "*.txt");

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
            var serviceHost = External_CreateService(directory, "*.txt");

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

            var serviceHost = External_CreateService(directory, "*.txt");

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

        private FtpAdapterInboundHandler Internal_CreateHandler(string inputFolder, string filter, out IConnection connection)
        {
            var adapter = new FtpAdapter { PollingInterval = 15 };
            var connectionUri = new FtpAdapterConnectionUri { HostName = "127.0.0.1", Path = inputFolder, FileName = filter };
            var factory = new FtpAdapterConnectionFactory(connectionUri, new ClientCredentials(), adapter);

            connection = factory.CreateConnection();
            connection.Open(TimeSpan.FromSeconds(30));

            var inputHandler = connection.BuildHandler<FtpAdapterInboundHandler>(null);

            inputHandler.StartListener(new string[0], TimeSpan.MaxValue);

            return inputHandler;
        }

        private void Internal_CheckFile(FtpAdapterInboundHandler inputHandler, string inputFile, string expectedContent)
        {
            Message message;
            IInboundReply reply;

            Assert.IsTrue(inputHandler.TryReceive(TimeSpan.FromMinutes(1), out message, out reply));
            
            Assert.AreEqual(
                new Uri(string.Format("ftp://127.0.0.1/{0}/{1}", directory, Path.GetFileName(inputFile))), 
                new Uri(message.Headers.Action));

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

            var connectionUri = new FtpAdapterConnectionUri { HostName = "127.0.0.1", Path = inputFolder, FileName = filter };

            host.AddServiceEndpoint(typeof(IService), new FtpAdapterBinding { PollingInterval = 15 }, connectionUri.Uri);
            host.Open();

            return host;
        }

        private void External_CheckFile(ServiceHost host, string inputFile, string expectedContent)
        {
            MessageItem message;
            var instance = host.SingletonInstance as Service;
            
            Assert.IsTrue(instance.Queue.TryTake(out message, TimeSpan.FromMinutes(1)));

            System.Threading.Thread.Sleep(1000);

            Assert.AreEqual(
                new Uri(string.Format("ftp://127.0.0.1/{0}/{1}", directory, Path.GetFileName(inputFile))), 
                new Uri(message.Action));
            Assert.AreEqual(expectedContent, message.Data);

            Assert.IsFalse(System.IO.File.Exists(inputFile));
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(inputFolder))
                Directory.Delete(inputFolder, true);
        }

        [FixtureTearDown]
        public void StopServer()
        {
            ftpServer.Kill();
        }
    }
}
