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
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Gui
{
    public partial class SetupWizard : ModernWizard
    {
        public SetupWizard()
        {
            InitializeComponent();
        }

        InstallationModeCollection GetInstallationModes(MsiInstallationModes mode)
        {
            var modes = SetupHelper.GetStandardInstallationModes(mode);
            //uncomment this line if you want to support modification mode iff reinstallation is possible
            //modes.InsertBefore(InstallationMode.Reinstall, InstallationMode.Modify);
            //uncomment this line if you don't want to support reinstallation
            //modes.Remove(InstallationMode.Reinstall);
            if (modes.Contains(SetupHelper.InstallationModeFromCommandLine))
                return new InstallationModeCollection(SetupHelper.InstallationModeFromCommandLine);
            else
                return modes;
        }

        public override void LifecycleAction(string type, object argument)
        {
            if (type == LifecycleActionType.Initialization)
            {
                AddStep(new InitializationStep());
            }
            else if (type == LifecycleActionType.ConnectionOpened)
            {
                //uncomment this line if you want to support MSI property passthrough
                //SetupHelper.ApplyMsiProperties();
                if (MsiConnection.Instance.IsRestored)
                {
                    AddStep(new InstallationStep(InstallationMode.Install));
                    AddStep(new FinishStep());
                }
                var modes = GetInstallationModes(MsiConnection.Instance.Mode);
                if (modes.Contains(InstallationMode.Downgrade))
                    AddStep(new FatalErrorStep(Gui.Properties.Resources.DowngradeNotSupported));
                else if (modes.Count > 1)
                    AddStep(new InstallationModeStep(modes));
                else if (modes.Count == 1)
                    AddStep(new WelcomeStep(modes[0]));
            }
            else if (type == LifecycleActionType.ModeSelected)
            {
                switch ((InstallationMode)argument)
                {
                    case InstallationMode.Install:
                        AddStep(new LicenseStep());
                        //AddStep(new PrerequisiteCheckStep());
                        //AddStep(new UserRegistrationStep());
                        //AddStep(new InstallationTypeStep());
                        AddStep(new InstallationLocationStep());
                        AddStep(new FeatureSelectionStep());
                        //AddStep(new Step1());
                        //AddStep(new ReadyStep());
                        AddStep(new InstallationStep(InstallationMode.Install));
                        AddStep(new FinishStep());
                        break;
                    case InstallationMode.Uninstall:
                        AddStep(new InstallationStep(InstallationMode.Uninstall));
                        AddStep(new FinishStep());
                        break;
                    case InstallationMode.Upgrade:
                        AddStep(new InstallationStep(InstallationMode.Uninstall));
                        //AddStep(new InstallationTypeStep());
                        AddStep(new InstallationLocationStep());
                        AddStep(new FeatureSelectionStep());
                        AddStep(new InstallationStep(InstallationMode.Install));
                        AddStep(new FinishStep());
                        break;
                    case InstallationMode.Reinstall:
                        AddStep(new InstallationStep(InstallationMode.Install));
                        AddStep(new FinishStep());
                        break;
                    default:
                        MessageBox.Show("Mode not supported: " + (InstallationMode)argument);
                        break;
                }
            }
            else
                MessageBox.Show("Unsupported lifecycle action");
        }
    }
}
