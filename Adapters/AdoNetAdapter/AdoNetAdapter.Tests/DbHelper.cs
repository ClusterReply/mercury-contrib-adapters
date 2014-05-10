using MbUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Adapter = Reply.Cluster.Mercury.Adapters.AdoNet;
using System.ServiceModel.Channels;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Tests
{
    [TestFixture]
    public class DbHelpers
    {
        static string DBPATH = Path.Combine(AssemblyDirectory, "TestDatabase.mdf");
        static string CONNECTIONSTRING = string.Format(@"Server=(LocalDB)\v11.0;AttachDbFilename={0};Database=TestDatabase;Trusted_Connection=Yes;", DBPATH);
        const string MASTER = @"Data Source=(LocalDB)\v11.0;Initial Catalog=master;Integrated Security=True";

        static public string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        [Test]
        public void CreateMessage()
        {
            Message msg = null;

            using (var connection = new SqlConnection(CONNECTIONSTRING))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT ID, Name, Date, Count, Value FROM TestTable";

                using (var reader = command.ExecuteReader())
                {
                    msg = Adapter.DbHelpers.CreateMessage(reader, "action");
                }
            }

            var body = msg.GetBody<EntityInboundData>();

            Assert.AreEqual(body.Count, 1);
            Assert.AreEqual(body[0].Count, 3);

            var rows = body[0].OrderBy(r => r.Name).ToArray();

            Assert.AreEqual(rows[0].Name, "A");
            Assert.AreEqual(rows[0].Date, new DateTime(2014, 2, 5));
            Assert.AreEqual(rows[0].Count, 3);
            Assert.AreEqual(rows[0].Value, (decimal)12.5);

            Assert.AreEqual(rows[1].Name, "B");
            Assert.AreEqual(rows[1].Date, null);
            Assert.AreEqual(rows[1].Count, 5);
            Assert.AreEqual(rows[1].Value, (decimal)0.5);

            Assert.AreEqual(rows[2].Name, "C");
            Assert.AreEqual(rows[2].Date, null);
            Assert.AreEqual(rows[2].Count, null);
            Assert.AreEqual(rows[2].Value, null);
        }

        [TearDown]
        public void TearDown()
        {
            using (var connection = new SqlConnection(MASTER))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = string.Format(@"ALTER DATABASE [TestDatabase] SET OFFLINE WITH ROLLBACK IMMEDIATE", DBPATH);
                command.ExecuteNonQuery();

                command.CommandText = string.Format(@"exec sp_detach_db '{0}'", DBPATH);
                command.ExecuteNonQuery();
            }
        }
    }
}
