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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;

namespace Gui
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class PrerequisiteCheckStep : SharpSetup.UI.Forms.Modern.ModernActionStep
    {
        public PrerequisiteCheckStep()
        {
            InitializeComponent();
        }

        private void plList_Finish(object sender, EventArgs e)
        {
            lblError.Visible = (plList.Status == PrerequisiteCheckStatus.Error);
            lblWarning.Visible = (plList.Status == PrerequisiteCheckStatus.Warning);
            Wizard.NextButton.Enabled = (plList.Status == PrerequisiteCheckStatus.Warning || plList.Status == PrerequisiteCheckStatus.Ok);
            Wizard.NextButton.Focus();
        }

        delegate void Action();
        bool test = false;
        private void plList_Check(object sender, PrerequisiteCheckEventArgs e)
        {
            if (e.Id == "tst1")
            {
                //put your custom test logic here
                e.Status = PrerequisiteCheckStatus.Ok;
                e.Message = "tst1 passed";
            }
            else if (e.Id == "tst2")
            {
                //put your custom test logic here
                e.Status = PrerequisiteCheckStatus.Ok;
                e.Message = "tst2 warning";
                test = !test;
                BeginInvoke((Action)(delegate() { MsiConnection.Instance.SetProperty("TEST", test ? "YES" : "NO"); }));
            }
            else if (e.Id == "prereqmodules")
            {
                //put your custom test logic here
                e.Status = PrerequisiteCheckStatus.Warning;
                e.Message = "Checks from .NET prerequisite modules are not executed when running GUI without bootstrapper.";
            }
        }

        private void PrerequisiteCheckStep_Entered(object sender, EventArgs e)
        {
            if (plList.Status == PrerequisiteCheckStatus.Unknown)
            {
                Wizard.NextButton.Enabled = false;
                plList.Start();
            }
            else
            {
                Wizard.NextButton.Enabled = (plList.Status == PrerequisiteCheckStatus.Warning || plList.Status == PrerequisiteCheckStatus.Ok);
                Wizard.NextButton.Focus();
            }
        }

        private void btnRerun_Click(object sender, EventArgs e)
        {
            Wizard.NextButton.Enabled = false;
            plList.Clear();
            plList.Start();
        }
    }
}
