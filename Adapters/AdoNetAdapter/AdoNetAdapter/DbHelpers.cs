using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;

namespace Reply.Cluster.Mercury.Adapters.AdoNet
{
    static class DbHelpers
    {
        private static DataContractSerializer objectSerializer = new DataContractSerializer(typeof(object));

        public static Message CreateMessage(DbDataReader reader, string action)
        {
            return CreateMessage(reader, null, action);
        }

        public static Message CreateMessage(DbDataReader reader, Transaction transaction, string action)
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
                                objectSerializer.WriteObjectContent(writer, reader[i]);
                        
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    } while (reader.NextResult());

                    writer.WriteEndDocument();
                    writer.Flush();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    using (var xmlReader = XmlReader.Create(stream))
                    {
                        var message = Message.CreateMessage(MessageVersion.Default, action, xmlReader);

                        if (transaction != null)
                            TransactionMessageProperty.Set(transaction, message);

                        return message;
                    }
                }
            }
        }
      
        public static Message CreateMessage(string operationType, int count, string action)
        {
            string ns = AdoNetAdapter.SERVICENAMESPACE + "/Messages";

            using (var stream = new System.IO.MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream))
                {
                    writer.WriteStartElement(operationType + "Result", ns);
                    writer.WriteAttributeString("count", count.ToString());
                    writer.WriteEndDocument();

                    writer.Flush();
                    stream.Seek(0, System.IO.SeekOrigin.Begin);

                    using (var xmlReader = XmlReader.Create(stream))
                        return Message.CreateMessage(MessageVersion.Default, action, xmlReader);
                }
            }
        }

        public static Dictionary<string, DbParameter> CreateParameters(XmlReader reader, DbCommand command, string parameterPrefix)
        {
            var parameters = new Dictionary<string, DbParameter>();

            while (reader.Read())
            {
                string name = reader.LocalName;
                object value = objectSerializer.ReadObject(reader, false);

                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterPrefix + name;
                parameter.Value = value;

                parameters[name] = parameter;
            }

            return parameters;
        }
    }
}
