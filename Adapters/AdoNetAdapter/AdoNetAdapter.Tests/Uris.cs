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

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Tests
{
    [TestFixture]
    public class Uris
    {
        [Test]
        public void SetUri()
        {
            var uri = new AdoNetAdapterConnectionUri();

            uri.ConnectionName = "TestConnection";
            uri.ProviderName = "System.Data.SqlClient";
            uri.ConnectionString = "Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;";
            uri.InboundID = "TestID";

            Assert.AreEqual(
                "ado://TestConnection/TestID?adoNetProvider=System.Data.SqlClient&Server=myServerAddress&Database=myDataBase&Trusted_Connection=True",
                uri.Uri.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void GetUri()
        {
            var uri = new AdoNetAdapterConnectionUri(new Uri(
                "ado://TestConnection/TestID?adoNetProvider=System.Data.SqlClient&Server=myServerAddress&Database=myDataBase&Trusted_Connection=True"));

            Assert.AreEqual("TestConnection", uri.ConnectionName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("System.Data.SqlClient", uri.ProviderName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True", uri.ConnectionString, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("TestID", uri.InboundID, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
