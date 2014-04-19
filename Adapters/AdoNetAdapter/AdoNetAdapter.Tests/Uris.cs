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

            Assert.AreEqual(new Uri(
                "ado://TestConnection/TestID?adoNetProvider=System.Data.SqlClient&Server=myServerAddress&Database=myDataBase&Trusted_Connection=True"), 
                uri.Uri);
        }

        [Test]
        public void GetUri()
        {
            var uri = new AdoNetAdapterConnectionUri(new Uri(
                "ado://TestConnection/TestID?adoNetProvider=System.Data.SqlClient&Server=myServerAddress&Database=myDataBase&Trusted_Connection=True"));

            Assert.AreEqual("TestConnection", uri.ConnectionName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("System.Data.SqlClient", uri.ProviderName, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;", uri.ConnectionString, StringComparison.InvariantCultureIgnoreCase);
            Assert.AreEqual("TestID", uri.InboundID, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
