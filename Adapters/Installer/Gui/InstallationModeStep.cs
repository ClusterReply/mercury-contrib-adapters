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
using System.Windows.Forms;
using SharpSetup.Base;
using SharpSetup.UI.Controls;
using SharpSetup.UI.Forms.Modern;
using System.Collections.ObjectModel;

namespace Gui
{
    [System.ComponentModel.ToolboxItem(false)]
    public partial class InstallationModeStep : ModernInfoStep
    {
        public InstallationModeStep(InstallationModeCollection installationModes)
        {
            InitializeComponent();
            imsModes.Modes.Clear();
            imsModes.Modes.AddRange(installationModes);
        }

        private void InstallationModeStep_MoveNext(object sender, ChangeStepEventArgs e)
        {
            Wizard.LifecycleAction(LifecycleActionType.ModeSelected, imsModes.SelectedMode);
        }
    }
}
