namespace Gui
{
    partial class PrerequisiteCheckStep
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrerequisiteCheckStep));
            SharpSetup.Base.PrerequisiteCheckItem prerequisiteCheckItem1 = new SharpSetup.Base.PrerequisiteCheckItem();
            SharpSetup.Base.PrerequisiteCheckItem prerequisiteCheckItem2 = new SharpSetup.Base.PrerequisiteCheckItem();
            SharpSetup.Base.PrerequisiteCheckItem prerequisiteCheckItem3 = new SharpSetup.Base.PrerequisiteCheckItem();
            this.plList = new SharpSetup.UI.Controls.PrerequisiteList();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.btnRerun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // plList
            // 
            resources.ApplyResources(this.plList, "plList");
            prerequisiteCheckItem1.Id = "tst1";
            resources.ApplyResources(prerequisiteCheckItem1, "prerequisiteCheckItem1");
            prerequisiteCheckItem2.Id = "tst2";
            resources.ApplyResources(prerequisiteCheckItem2, "prerequisiteCheckItem2");
            prerequisiteCheckItem3.Id = "prereqmodules";
            resources.ApplyResources(prerequisiteCheckItem3, "prerequisiteCheckItem3");
            this.plList.CheckItems.Add(prerequisiteCheckItem1);
            this.plList.CheckItems.Add(prerequisiteCheckItem2);
            this.plList.CheckItems.Add(prerequisiteCheckItem3);
            this.plList.Name = "plList";
            this.plList.Check += new System.EventHandler<SharpSetup.UI.Controls.PrerequisiteCheckEventArgs>(this.plList_Check);
            this.plList.Finish += new System.EventHandler<System.EventArgs>(this.plList_Finish);
            // 
            // lblHeader
            // 
            resources.ApplyResources(this.lblHeader, "lblHeader");
            this.lblHeader.Name = "lblHeader";
            // 
            // lblWarning
            // 
            resources.ApplyResources(this.lblWarning, "lblWarning");
            this.lblWarning.Name = "lblWarning";
            // 
            // lblError
            // 
            resources.ApplyResources(this.lblError, "lblError");
            this.lblError.Name = "lblError";
            // 
            // btnRerun
            // 
            resources.ApplyResources(this.btnRerun, "btnRerun");
            this.btnRerun.Name = "btnRerun";
            this.btnRerun.UseVisualStyleBackColor = true;
            this.btnRerun.Click += new System.EventHandler(this.btnRerun_Click);
            // 
            // PrerequisiteCheckStep
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.btnRerun);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblWarning);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.plList);
            this.Name = "PrerequisiteCheckStep";
            this.Entered += new System.EventHandler<System.EventArgs>(this.PrerequisiteCheckStep_Entered);
            this.Controls.SetChildIndex(this.plList, 0);
            this.Controls.SetChildIndex(this.lblHeader, 0);
            this.Controls.SetChildIndex(this.lblWarning, 0);
            this.Controls.SetChildIndex(this.lblError, 0);
            this.Controls.SetChildIndex(this.btnRerun, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpSetup.UI.Controls.PrerequisiteList plList;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnRerun;
    }
}
