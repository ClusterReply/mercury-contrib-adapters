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
using System.IO;
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.Prerequisites.Base;
using SharpSetup.UI.Forms.Modern;
using Gui.Properties;

namespace Gui
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class InstallationStep : ModernActionStep
    {
        InstallationMode mode;
        public InstallationStep(InstallationMode mode)
        {
            InitializeComponent();
            this.mode = mode;
        }

        private void InstallationStep_Entered(object sender, EventArgs e)
        {
            ipProgress.StartListening();
            try
            {
                if (mode == InstallationMode.Uninstall)
                {
                    MsiConnection.Instance.Uninstall();
                    /*
                    try
                    {
                        MsiConnection.Instance.Open(new Guid("{60b435f8-fb8b-475e-9167-96018cdf68cb}"), false);
                        MsiConnection.Instance.Uninstall();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Uninstall");
                    }
                    */
                    if (File.Exists(Resources.MainMsiFile))
                        MsiConnection.Instance.Open(Resources.MainMsiFile, true);
                }
                else if (mode == InstallationMode.Install)
                {
                    PrerequisiteManager.Instance.Install();
                    /*
                    MsiConnection.Instance.SaveAs("MainInstall");
                    MsiConnection.Instance.EnableSettingsChanged = false;
                    MsiConnection.Instance.Open("other.msi", false);
                    MsiConnection.Instance.Install("");
                    MsiConnection.Instance.OpenSaved("MainInstall");
                    */
                    MsiConnection.Instance.Install();
                }
                else
                    MessageBox.Show("Unknown mode");
            }
            catch (MsiException mex)
            {
                if (mex.ErrorCode != (uint)InstallError.UserExit)
                    MessageBox.Show("Installation failed: " + mex.Message);
                Wizard.Finish();
            }
            ipProgress.StopListening();
            Wizard.NextStep();
        }

        public override bool CanClose()
        {
            return false;
        }
    }
}
