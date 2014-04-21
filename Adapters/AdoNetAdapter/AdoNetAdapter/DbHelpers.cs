using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    static class DbHelpers
    {
        public static Message CreateMessage(DbDataReader reader, string action)
        {
            string ns = AdoNetAdapter.SERVICENAMESPACE + "/Messages";

            using (var stream = new System.IO.MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    writer.WriteStartElement("InboundData", ns);

                    do
                    {
                        writer.WriteStartElement("ResultSet", ns);

                        while (reader.Read())
                        {
                            writer.WriteStartElement("Row", ns);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                // TODO: conversione del tipo

                                writer.WriteElementString(reader.GetName(i), ns, reader.GetString(i));
                            }

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    } while (reader.NextResult());

                    writer.Flush();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    using (var xmlReader = XmlReader.Create(stream))
                        return Message.CreateMessage(MessageVersion.Default, action, xmlReader);
                }
            }
        }

        public static Array CreateParameters(Message message, DbCommand command)
        {
            var parameters = new List<DbParameter>();

            using (var reader = message.GetReaderAtBodyContents())
            {
                reader.MoveToStartElement();

                while (reader.Read())
                {
                    // TODO: conversione del tipo

                    string name = reader.Name;
                    string value = reader.ReadString();

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = name;
                    parameter.Value = value;

                    parameters.Add(parameter);
                }
            }

            return parameters.ToArray();
        }
    }
}
