using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.AdoNet.Messages
{
    public static class Constants
    {
        public const string MESSAGENAMESPACE = "http://mercury.cluster.reply.eu/adapters/ado.net/2014/04/Messages";

        public const string CREATE = "Create";
        public const string READ = "Read";
        public const string UPDATE = "Update";
        public const string DELETE = "Delete";

        public const string INBOUND_DATA = "InboundData";
        public const string RESULT_SET = "ResultSet";
        public const string ROW = "Row";

        public const string CREATE_RESULT = "CreateResult";
        public const string UPDATE_RESULT = "UpdateResult";
        public const string DELETE_RESULT = "DeleteResult";
    }
}
