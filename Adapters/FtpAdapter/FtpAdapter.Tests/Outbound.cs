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
using System.Threading;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Ftp.Tests
{
    public abstract class Outbound
    {
        private static string directory = "FtpAdapter";
        private static string rootFolder = Path.GetTempPath();
        private string outputFolder = Path.Combine(rootFolder, directory);
        private string ftpSeverPath = Path.Combine(AssemblyDirectory, "sfk171.exe");

        private Process ftpServer;

        protected abstract string GetFtpServerParameters();
        protected abstract FtpAdapterConnectionUri GetFtpAdapterConnectionUri(string outputFolder, string destinationFile);
        protected abstract ClientCredentials GetCredentials();

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

        private static MemoryStream GetTestStream(string fileContent)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.Write(fileContent);
            streamWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private static Message CreateMessage(string path, MemoryStream body)
        {
            var message = ByteStreamMessage.CreateMessage(body);
            message.Headers.Action = new Uri(path).ToString();

            return message;
        }

        [FixtureSetUp]
        public void StartServer()
        {
            //Debugger.Launch();

            var processInfo = new ProcessStartInfo(ftpSeverPath, GetFtpServerParameters()) { WorkingDirectory = rootFolder };

            ftpServer = Process.Start(processInfo);
        }

        [SetUp]
        public void Initialize()
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);
        }

        [Test]
        public void External_WriteFile_Simple()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "Test.txt",
                Path.Combine(outputFolder, @"Test.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_WriteFile_FileNameWithMacroPassthrough()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "%SourceFileName%",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_WriteFile_FolderWithMacro()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "Test.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/Test.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_WriteFile_FileNameWithMacro()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format("{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_WriteFile_FolderWithMacroAndFileNameWithMacroPassthrough()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "%SourceFileName%",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/File.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_WriteFile_FolderWithMacroAndFileNameWithMacro()
        {
            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        [ExpectedException(typeof(System.Net.FtpClient.FtpException))]
        public void External_WriteFile_Existing()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void External_ReplaceFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile,
                OverwriteAction.Replace);
        }

        [Test]
        [Ignore("The sfk FTP server does not support the APPE command.")]
        public void External_AppendFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            External_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile_Existing + Properties.Resources.TestFile,
                OverwriteAction.Append);
        }

        [Test]
        public void Internal_WriteFile_Simple()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "Test.txt",
                Path.Combine(outputFolder, @"Test.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_WriteFile_FileNameWithMacroPassthrough()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "%SourceFileName%",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_WriteFile_FolderWithMacro()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "Test.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/Test.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_WriteFile_FileNameWithMacro()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format("{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_WriteFile_FolderWithMacroAndFileNameWithMacroPassthrough()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "%SourceFileName%",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/File.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_WriteFile_FolderWithMacroAndFileNameWithMacro()
        {
            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory + "/%YEAR%/%MONTH%/%DAY%/",
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        [ExpectedException(typeof(System.Net.FtpClient.FtpException))]
        public void Internal_WriteFile_Existing()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void Internal_ReplaceFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile,
                OverwriteAction.Replace);
        }

        [Test]
        [Ignore("The sfk FTP server does not support the APPE command.")]
        public void Internal_AppendFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            Internal_OutputTest(
                "ftp://127.0.0.1/Prova/File.txt",
                directory,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile_Existing + Properties.Resources.TestFile,
                OverwriteAction.Append);
        }

        public void External_OutputTest(string sourcePath, string destinationFolder, string destinationFile, string expectedPath, string fileContent, OverwriteAction overwriteAction = default(OverwriteAction))
        {
            var factory = new ChannelFactory<IService>(new FtpAdapterBinding { OverwriteAction = overwriteAction },
                new EndpointAddress(GetFtpAdapterConnectionUri(destinationFolder, destinationFile).Uri.ToString()));
            var channel = factory.CreateChannel();

            try
            {
                channel.Execute(CreateMessage(sourcePath, GetTestStream(fileContent)));

                Assert.IsTrue(System.IO.File.Exists(expectedPath));
                Assert.AreEqual(fileContent, System.IO.File.ReadAllText(expectedPath));
            }
            finally
            {
                var innerChannel = channel as IChannel;

                if (innerChannel != null)
                {
                    if (innerChannel.State == CommunicationState.Opened)
                        innerChannel.Close();
                    else
                        innerChannel.Abort();
                }
            }
        }

        public void Internal_OutputTest(string sourcePath, string destinationFolder, string destinationFile, string expectedPath, string fileContent, OverwriteAction overwriteAction = default(OverwriteAction))
        {
            var adapter = new FtpAdapter { OverwriteAction = overwriteAction };
            var connectionUri = GetFtpAdapterConnectionUri(destinationFolder, destinationFile);
            var factory = new FtpAdapterConnectionFactory(connectionUri, GetCredentials(), adapter);

            var connection = factory.CreateConnection();

            try
            {
                connection.Open(TimeSpan.FromSeconds(15));

                var outputHandler = connection.BuildHandler<FtpAdapterOutboundHandler>(null);

                outputHandler.Execute(CreateMessage(sourcePath, GetTestStream(fileContent)), TimeSpan.MaxValue);

                Assert.IsTrue(System.IO.File.Exists(expectedPath));
                Assert.AreEqual(fileContent, System.IO.File.ReadAllText(expectedPath));
            }
            finally
            {
                connection.Close(TimeSpan.FromSeconds(15));
            }
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);
        }

        [FixtureTearDown]
        public void StopServer()
        {
            ftpServer.Kill();
        }
    }
}
