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
using System.Diagnostics;
using SharpSetup.Base;
using SharpSetup.Prerequisites.Base;
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class FinishStep : ModernInfoStep
    {
        public FinishStep()
        {
            InitializeComponent();
        }

        private void FinishStep_Entered(object sender, EventArgs e)
        {
            Wizard.BackButton.Enabled = false;
        }

        private void FinishStep_Finish(object sender, ChangeStepEventArgs e)
        {
            if (cbRunNow.Checked)
                Process.Start(string.Format(Gui.Properties.Resources.FinishStepCommand, MsiConnection.Instance.GetPath("INSTALLLOCATION")));
            if (cbRestartNow.Checked)
                SetupHelper.Restart(this, RestartOptions.Schedule | RestartOptions.NoAsk);
        }

        private void FinishStep_Entering(object sender, ChangeStepEventArgs e)
        {
            cbRunNow.Visible = cbRunNow.Checked = Globals.GetVariable<bool>("AllowRunOnFinish") && !MsiConnection.Instance.RebootRequired;
            cbRestartNow.Visible = cbRestartNow.Checked = (MsiConnection.Instance.RebootRequired || PrerequisiteManager.Instance.GetProperty(StandardProperties.RebootRequired, false)) && !SetupHelper.GetCommandLineOption("sssp.norestart", false);
        }
    }
}
