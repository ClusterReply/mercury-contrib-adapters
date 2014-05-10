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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.File.Tests
{
    [TestFixture]
    public class Uris
    {
        [Test]
        public void SetLocalUri()
        {
            var uri = new FileAdapterConnectionUri();

            uri.FileName = "*.txt";
            uri.Path = @"C:\Local\Folder";

            Assert.AreEqual(
                "file:///C:/Local/Folder/*.txt",
                uri.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void GetLocalUri()
        {
            var uri = new FileAdapterConnectionUri(new Uri(
                "file://C:/Local/Folder/*.txt"));

            Assert.AreEqual("*.txt", uri.FileName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual(@"C:\Local\Folder", uri.Path, StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void SetRemoteUri()
        {
            var uri = new FileAdapterConnectionUri();

            uri.FileName = "*.txt";
            uri.Path = @"\\server\C$\Local\Folder";

            Assert.AreEqual(
                "file://server/C$/Local/Folder/*.txt",
                uri.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void GetRemoteUri()
        {
            var uri = new FileAdapterConnectionUri(new Uri(
                "file://server/C$/Local/Folder/*.txt"));

            Assert.AreEqual("*.txt", uri.FileName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual(@"\\server\C$\Local\Folder", uri.Path, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
