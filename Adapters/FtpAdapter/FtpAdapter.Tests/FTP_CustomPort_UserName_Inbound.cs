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
using Microsoft.ServiceModel.Channels.Common;
using Reply.Cluster.Mercury.Adapters.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Mercury.Adapters.Ftp.Tests
{
    [TestFixture]
    public class FTP_CustomPort_UserName_Inbound : Inbound
    {
        protected override string GetFtpServerParameters()
        {
            return "ftpserv -rw -user=user -pw=pass -port=2021";
        }

        protected override FtpAdapterConnectionUri GetFtpAdapterConnectionUri(string inputFolder, string filter)
        {
            return new FtpAdapterConnectionUri { HostName = "127.0.0.1", Port = 2021, Path = inputFolder, FileName = filter };
        }

        protected override string GetExpectedAction(string directory, string fileName)
        {
            return string.Format("ftp://127.0.0.1:2021/{0}/{1}", directory, fileName);
        }

        protected override AdapterClientCredentials GetCredentials()
        {
            var credentials = new AdapterClientCredentials();

            credentials.UserName.UserName = "user";
            credentials.UserName.Password = "pass";

            return credentials;
        }
    }
}
