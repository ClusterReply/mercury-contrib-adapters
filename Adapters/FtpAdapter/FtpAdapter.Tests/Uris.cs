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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Ftp.Tests
{
    [TestFixture]
    public class Uris
    {
        //[SetUp]
        public void Debug()
        {
            Debugger.Launch();
        }

        [Test]
        public void SetUri()
        {
            var uri = new FtpAdapterConnectionUri();

            uri.Protocol = ProtocolType.FTP;
            uri.Port = 2021;
            uri.HostName = "server.domain.com";
            uri.Path = "remote/folder";
            uri.FileName = "*.txt";

            Assert.AreEqual(
                "ftp://server.domain.com:2021/remote/folder/*.txt",
                uri.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void GetUri()
        {
            var uri = new FtpAdapterConnectionUri(new Uri(
                "ftp://server.domain.com:2021/remote/folder/*.txt"));

            Assert.AreEqual(ProtocolType.FTP, uri.Protocol);
            Assert.AreEqual(2021, uri.Port);
            Assert.AreEqual("server.domain.com", uri.HostName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("remote/folder", uri.Path, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("*.txt", uri.FileName, StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void SetUri_DefaultPort()
        {
            var uri = new FtpAdapterConnectionUri();

            uri.Protocol = ProtocolType.FTP;
            uri.HostName = "server.domain.com";
            uri.Path = "remote/folder";
            uri.FileName = "*.txt";

            Assert.AreEqual(
                "ftp://server.domain.com/remote/folder/*.txt",
                uri.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void GetUri_DefaultPort()
        {
            var uri = new FtpAdapterConnectionUri(new Uri(
                "ftp://server.domain.com/remote/folder/*.txt"));

            Assert.AreEqual(ProtocolType.FTP, uri.Protocol);
            Assert.AreEqual(21, uri.Port);
            Assert.AreEqual("server.domain.com", uri.HostName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("remote/folder", uri.Path, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("*.txt", uri.FileName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
