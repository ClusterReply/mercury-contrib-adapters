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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.File.Tests
{
    [TestFixture]
    public class Outbound
    {
        private string outputFolder = Path.Combine(Path.GetTempPath(), "FileAdapter");

        private static MemoryStream GetTestStream(string fileContent)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            streamWriter.Write(fileContent);
            streamWriter.Flush();

            return stream;
        }

        private static Message CreateMessage(string path, MemoryStream body)
        {
            using (body)
            {
                return Message.CreateMessage(MessageVersion.Default, path, body.ToArray());
            }
        }

        [SetUp]
        public void Initialize()
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);
        }

        [Test]
        public void WriteFile_Simple()
        {
            OutputTest(
                @"C:\Prova\File.txt",
               outputFolder, 
                "Test.txt", 
                Path.Combine(outputFolder, @"Test.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void WriteFile_FileNameWithMacroPassthrough()
        {
            OutputTest(
                @"C:\Prova\File.txt",
                outputFolder,
                "%SourceFileName%",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void WriteFile_FolderWithMacro()
        {
            OutputTest(
                @"C:\Prova\File.txt",
                Path.Combine(outputFolder, @"%YEAR%/%MONTH%/%DAY%/"),
                "Test.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/Test.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void WriteFile_FileNameWithMacro()
        {
            OutputTest(
                @"C:\Prova\File.txt",
                outputFolder,
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format("{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void WriteFile_FolderWithMacroAndFileNameWithMacroPassthrough()
        {
            OutputTest(
                @"C:\Prova\File.txt",
                Path.Combine(outputFolder, @"%YEAR%/%MONTH%/%DAY%/"),
                "%SourceFileName%",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/File.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        public void WriteFile_FolderWithMacroAndFileNameWithMacro()
        {
            OutputTest(
                @"C:\Prova\File.txt",
                Path.Combine(outputFolder, @"%YEAR%/%MONTH%/%DAY%/"),
                "%YEAR%%MONTH%%DAY%.txt",
                Path.Combine(outputFolder, string.Format(@"{0:0000}/{1:00}/{2:00}/{0:0000}{1:00}{2:00}.txt", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)),
                Properties.Resources.TestFile);
        }

        [Test]
        [ExpectedException(typeof(IOException))]
        public void WriteFile_Existing()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            OutputTest(
                @"C:\Prova\File.txt",
               outputFolder,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile);
        }

        [Test]
        public void ReplaceFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            OutputTest(
                @"C:\Prova\File.txt",
               outputFolder,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile,
                OverwriteAction.Replace);
        }

        [Test]
        public void AppendFile()
        {
            System.IO.File.WriteAllText(Path.Combine(outputFolder, "File.txt"), Properties.Resources.TestFile_Existing);

            OutputTest(
                @"C:\Prova\File.txt",
               outputFolder,
                "File.txt",
                Path.Combine(outputFolder, "File.txt"),
                Properties.Resources.TestFile_Existing + Properties.Resources.TestFile,
                OverwriteAction.Append);
        }

        public void OutputTest(string sourcePath, string destinationFolder, string destinationFile, string expectedPath, string fileContent, OverwriteAction overwriteAction = default(OverwriteAction))
        {
            var factory = new ChannelFactory<IService>(new FileAdapterBinding { OverwriteAction = overwriteAction },
                new EndpointAddress(new FileAdapterConnectionUri { Path = destinationFolder, FileName = destinationFile }.Uri.ToString()));
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

        [TearDown]
        public void Cleanup()
        {
            //if (Directory.Exists(outputFolder))
            //    Directory.Delete(outputFolder, true);
        }
    }
}
