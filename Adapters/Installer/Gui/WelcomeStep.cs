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
using SharpSetup.Base;
using SharpSetup.UI.Forms.Modern;
using System.ComponentModel;

namespace Gui
{
    [ToolboxItem(false)]
    public partial class WelcomeStep : ModernInfoStep
    {
        InstallationMode mode;
        public WelcomeStep(InstallationMode mode)
        {
            this.mode = mode;
            InitializeComponent();
            lblMode.Text = Gui.Properties.Resources.ResourceManager.GetString("WelcomeStepGreeting" + mode.ToString()) ?? lblMode.Text;
        }

        private void WelcomeStep_MoveNext(object sender, ChangeStepEventArgs e)
        {
            Wizard.LifecycleAction(LifecycleActionType.ModeSelected, mode);
        }
    }
}
