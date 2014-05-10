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
        public const string COMPOSITE = "Composite";

        public const string INBOUND_DATA = "InboundData";
        public const string RESULT_SET = "ResultSet";
        public const string ROW = "Row";
        public const string RESULT = "Result";

        public const string MULTIEXECUTE_RESULT = "MultiExecuteResult";
        public const string CREATE_RESULT = "CreateResult";
        public const string UPDATE_RESULT = "UpdateResult";
        public const string DELETE_RESULT = "DeleteResult";
        public const string COMPOSITE_RESULT = "CompositeResult";
    }
}
