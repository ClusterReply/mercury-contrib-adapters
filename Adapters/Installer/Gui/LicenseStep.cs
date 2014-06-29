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
using SharpSetup.UI.Forms.Modern;

namespace Gui
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class LicenseStep : ModernActionStep
    {
        public LicenseStep()
        {
            InitializeComponent();
        }

        private void LicenseStep_Load(object sender, EventArgs e)
        {
            rtbLicense.Rtf = Gui.Properties.Resources.LicenseStepRtf;
        }

        private void cbAccept_CheckedChanged(object sender, EventArgs e)
        {
            Wizard.NextButton.Enabled = cbAccept.Checked;
        }
    }
}
