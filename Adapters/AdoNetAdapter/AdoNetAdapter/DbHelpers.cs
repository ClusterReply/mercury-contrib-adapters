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
using Reply.Cluster.Mercury.Adapters.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
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

        private static Dictionary<DbType, DataContractSerializer> serializers = new Dictionary<DbType, DataContractSerializer>
        {
            { DbType.AnsiString, new DataContractSerializer(typeof(string)) },
            { DbType.AnsiStringFixedLength, new DataContractSerializer(typeof(string)) },
            { DbType.Binary, new DataContractSerializer(typeof(byte[])) },
            { DbType.Boolean, new DataContractSerializer(typeof(bool?)) },
            { DbType.Byte, new DataContractSerializer(typeof(byte?)) },
            { DbType.Currency, new DataContractSerializer(typeof(decimal?)) },
            { DbType.Date, new DataContractSerializer(typeof(DateTime?)) },
            { DbType.DateTime, new DataContractSerializer(typeof(DateTime?)) },
            { DbType.DateTime2, new DataContractSerializer(typeof(DateTime?)) },
            { DbType.DateTimeOffset, new DataContractSerializer(typeof(DateTimeOffset?)) },
            { DbType.Decimal, new DataContractSerializer(typeof(decimal?)) },
            { DbType.Double, new DataContractSerializer(typeof(double?)) },
            { DbType.Guid, new DataContractSerializer(typeof(Guid?)) },
            { DbType.Int16, new DataContractSerializer(typeof(Int16?)) },
            { DbType.Int32, new DataContractSerializer(typeof(Int32?)) },
            { DbType.Int64, new DataContractSerializer(typeof(Int64?)) },
            { DbType.Object, new DataContractSerializer(typeof(object)) },
            { DbType.SByte, new DataContractSerializer(typeof(SByte?)) },
            { DbType.Single, new DataContractSerializer(typeof(Single?)) },
            { DbType.String, new DataContractSerializer(typeof(string)) },
            { DbType.StringFixedLength, new DataContractSerializer(typeof(string)) },
            { DbType.UInt16, new DataContractSerializer(typeof(UInt16?)) },
            { DbType.UInt32, new DataContractSerializer(typeof(UInt32?)) },
            { DbType.UInt64, new DataContractSerializer(typeof(UInt64?)) },
            { DbType.VarNumeric, new DataContractSerializer(typeof(double?)) },
            { DbType.Xml, new DataContractSerializer(typeof(string)) }
        };

        private static void WriteResult(XmlWriter writer, DbDataReader reader)
        {
            string ns = AdoNetAdapter.MESSAGENAMESPACE;

            writer.WriteStartElement("InboundData", ns);

            do
            {
                writer.WriteStartElement("ResultSet", ns);

                while (reader.Read())
                {
                    writer.WriteStartElement("Row", ns);

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            writer.WriteStartElement(reader.GetName(i), ns);

                            object value = reader[i];
                            var serializer = new DataContractSerializer(value.GetType());
                            objectSerializer.WriteObjectContent(writer, value);

                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            } while (reader.NextResult());

            writer.WriteEndElement();
        }

        private static void WriteResult(XmlWriter writer, string operationType, int count)
        {
            string ns = AdoNetAdapter.MESSAGENAMESPACE;

            writer.WriteStartElement(operationType + "Result", ns);
            writer.WriteElementString("Count", ns, count.ToString());
            writer.WriteEndElement();
        }

        public static Message CreateMessage(DbDataReader reader, string action)
        {
            return CreateMessage(reader, null, action);
        }

        public static Message CreateMessage(DbDataReader reader, Transaction transaction, string action)
        {
            var stream = new System.IO.MemoryStream();

            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { NamespaceHandling = NamespaceHandling.OmitDuplicates }))
            {
                WriteResult(writer, reader);

                writer.WriteEndDocument();
                writer.Flush();
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                var message = Message.CreateMessage(MessageVersion.Default, action, XmlReader.Create(stream));

                if (transaction != null)
                    TransactionMessageProperty.Set(transaction, message);

                return message;
            }
        }

        public static Message CreateMessage(string operationType, int count, string action)
        {
            var stream = new System.IO.MemoryStream();

            using (var writer = XmlWriter.Create(stream))
            {
                WriteResult(writer, operationType, count);

                writer.WriteEndDocument();
                writer.Flush();
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                return Message.CreateMessage(MessageVersion.Default, action, XmlReader.Create(stream));
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

        internal static void SetParameters(XmlReader reader, DbParameterCollection parameters)
        {
            var parms = GetParameters(parameters);
            
            reader.MoveToContent();
            reader.Read();

            do
            {
                string name = reader.LocalName;

                if (!parms.ContainsKey(name))
                    throw new IndexOutOfRangeException(string.Format("Parameter '{0}' not found", name));

                var parameter = parms[name];
                object value = serializers[parameter.DbType].ReadObject(reader, false);

                if (value != null)
                    parameter.Value = value;
                else
                    parameter.Value = DBNull.Value;
            } while (reader.NodeType == XmlNodeType.Element);
        }

        public static void SetSourceParameters(XmlReader reader, DbParameterCollection parameters)
        {
            var sourceParametes = GetSourceParameters(parameters);
            var nullParametes = GetNullMappingParameters(parameters);

            foreach (var parameter in sourceParametes.Values)
                parameter.Value = DBNull.Value;

            reader.MoveToContent();
            reader.Read();

            do
            {
                string name = reader.LocalName;

                if (!sourceParametes.ContainsKey(name))
                    throw new IndexOutOfRangeException(string.Format("Column '{0}' not found", name));

                var parameter = sourceParametes[name];
                object value = serializers[parameter.DbType].ReadObject(reader, false);

                if (value != null)
                {
                    if (nullParametes.ContainsKey(name))
                        nullParametes[name].Value = false;

                    parameter.Value = value;
                }
            } while (reader.NodeType == XmlNodeType.Element);
        }

        public static void SetTargetParameters(XmlReader reader, DbParameterCollection parameters)
        {
            var targetParametes = GetTargetParameters(parameters);

            foreach (var parameter in targetParametes.Values)
                parameter.Value = DBNull.Value;

            reader.MoveToContent();
            reader.Read();

            do
            {
                string name = reader.LocalName;
                
                if (!targetParametes.ContainsKey(name))
                    throw new IndexOutOfRangeException(string.Format("Column '{0}' not found", name));

                var parameter = targetParametes[name];
                object value = serializers[parameter.DbType].ReadObject(reader, false);
                                
                if (value != null)
                    parameter.Value = value;
            } while (reader.NodeType == XmlNodeType.Element);
        }

        public static Message Execute(XmlReader xmlReader, DbConnection connection, string procedureName, Type commandBuilderType, string action)
        {
            var command = connection.CreateCommand();

            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = procedureName;

            dynamic staticCommandBuilder = new StaticMembersDynamicWrapper(commandBuilderType);
            staticCommandBuilder.DeriveParameters(command);

            DbHelpers.SetParameters(xmlReader.ReadSubtree(), command.Parameters);

            // TODO: parametri in uscita

            using (var reader = command.ExecuteReader())
            {
                return DbHelpers.CreateMessage(reader, action);
            }
        }

        public static Message MultiExecute(XmlReader xmlReader, DbConnection connection, string procedureName, Type commandBuilderType, string action)
        {
            string ns = AdoNetAdapter.MESSAGENAMESPACE;
            dynamic staticCommandBuilder = new StaticMembersDynamicWrapper(commandBuilderType);

            var stream = new System.IO.MemoryStream();

            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartElement("MultiExecuteResult", ns);

                xmlReader.MoveToContent();
                xmlReader.Read();

                while (xmlReader.Read())
                {
                    var command = connection.CreateCommand();

                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;

                    staticCommandBuilder.DeriveParameters(command);

                    DbHelpers.SetParameters(xmlReader.ReadSubtree(), command.Parameters);

                    // TODO: parametri in uscita

                    using (var reader = command.ExecuteReader())
                    {
                        WriteResult(writer, reader);
                    }
                }

                writer.WriteEndDocument();
                writer.Flush();
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                return Message.CreateMessage(MessageVersion.Default, action, XmlReader.Create(stream));
            }
        }

        public static Message Create(XmlReader xmlReader, DbConnection connection, string operationType, DbCommandBuilder commandBuilder, string action)
        {
            int count = 0;

            while (xmlReader.ReadToFollowing("Row", AdoNetAdapter.MESSAGENAMESPACE))
            {
                var command = commandBuilder.GetInsertCommand();
                DbHelpers.SetTargetParameters(xmlReader.ReadSubtree(), command.Parameters);

                count += command.ExecuteNonQuery();
            }

            return DbHelpers.CreateMessage(operationType, count, action);
        }

        public static Message Read(XmlReader xmlReader, DbConnection connection, string action)
        {
            xmlReader.ReadToFollowing("Query", AdoNetAdapter.MESSAGENAMESPACE);
            string query = xmlReader.ReadString();

            var command = connection.CreateCommand();
            command.CommandText = query;

            using (var reader = command.ExecuteReader())
            {
                return DbHelpers.CreateMessage(reader, action);
            }
        }

        public static Message Update(XmlReader xmlReader, DbConnection connection, string operationType, DbCommandBuilder commandBuilder, string action)
        {
            int count = 0;

            while (xmlReader.ReadToFollowing("Pair", AdoNetAdapter.MESSAGENAMESPACE))
            {
                var command = commandBuilder.GetUpdateCommand();

                xmlReader.ReadToFollowing("Before", AdoNetAdapter.MESSAGENAMESPACE);
                DbHelpers.SetSourceParameters(xmlReader.ReadSubtree(), command.Parameters);

                xmlReader.ReadToFollowing("After", AdoNetAdapter.MESSAGENAMESPACE);
                DbHelpers.SetTargetParameters(xmlReader.ReadSubtree(), command.Parameters);

                count += command.ExecuteNonQuery();
            }

            return DbHelpers.CreateMessage(operationType, count, action);
        }

        public static Message Delete(XmlReader xmlReader, DbConnection connection, string operationType, DbCommandBuilder commandBuilder, string action)
        {
            int count = 0;

            while (xmlReader.ReadToFollowing("Row", AdoNetAdapter.MESSAGENAMESPACE))
            {
                var command = commandBuilder.GetDeleteCommand();
                DbHelpers.SetSourceParameters(xmlReader.ReadSubtree(), command.Parameters);

                count += command.ExecuteNonQuery();
            }

            return DbHelpers.CreateMessage(operationType, count, action);
        }

        public static Message Composite(XmlReader xmlReader, DbConnection connection, Type commandBuilderType, string action)
        {
            string ns = AdoNetAdapter.MESSAGENAMESPACE;
            dynamic staticCommandBuilder = new StaticMembersDynamicWrapper(commandBuilderType);

            var stream = new System.IO.MemoryStream();

            using (var writer = XmlWriter.Create(stream))
            {
                writer.WriteStartElement("CompositeResult", ns);

                xmlReader.MoveToContent();
                xmlReader.Read();

                while (xmlReader.Read())
                {
                    string operationType = xmlReader.LocalName;


                }

                writer.WriteEndDocument();
                writer.Flush();
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                return Message.CreateMessage(MessageVersion.Default, action, XmlReader.Create(stream));
            }
        }

        private static Dictionary<string, DbParameter> GetParameters(DbParameterCollection parameters)
        {
            return parameters.Cast<DbParameter>().ToDictionary(p => string.IsNullOrEmpty(p.SourceColumn) ? p.ParameterName.TrimStart('@') : p.SourceColumn);
        }

        private static Dictionary<string, DbParameter> GetSourceParameters(DbParameterCollection parameters)
        {
            return parameters.Cast<DbParameter>()
                .Where(p => p.SourceVersion == DataRowVersion.Original && !p.SourceColumnNullMapping).ToDictionary(p => p.SourceColumn);
        }

        private static Dictionary<string, DbParameter> GetTargetParameters(DbParameterCollection parameters)
        {
            return parameters.Cast<DbParameter>()
                .Where(p => p.SourceVersion == DataRowVersion.Current && !p.SourceColumnNullMapping).ToDictionary(p => p.SourceColumn);
        }

        private static Dictionary<string, DbParameter> GetNullMappingParameters(DbParameterCollection parameters)
        {
            return parameters.Cast<DbParameter>()
                .Where(p => p.SourceColumnNullMapping).ToDictionary(p => p.SourceColumn);
        }
    }
}
