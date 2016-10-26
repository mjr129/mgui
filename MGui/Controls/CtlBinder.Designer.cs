using MGui.Controls;

namespace MGui.Controls
{
    partial class CtlBinder<T>
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._cmsRevertButton = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._mnuUndoChanges = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuSetToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this._mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._errorProvider = new MGui.Controls.CtlError(this.components);
            this._cmsRevertButton.SuspendLayout();
            // 
            // _cmsRevertButton
            // 
            this._cmsRevertButton.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mnuHelp,
            this.toolStripSeparator1,
            this._mnuSetToDefault,
            this._mnuUndoChanges});
            this._cmsRevertButton.Name = "_cmsRevertButton";
            this._cmsRevertButton.ShowImageMargin = false;
            this._cmsRevertButton.Size = new System.Drawing.Size(126, 76);
            // 
            // _mnuUndoChanges
            // 
            this._mnuUndoChanges.Name = "_mnuUndoChanges";
            this._mnuUndoChanges.Size = new System.Drawing.Size(125, 22);
            this._mnuUndoChanges.Text = "Undo changes";
            // 
            // _mnuSetToDefault
            // 
            this._mnuSetToDefault.Name = "_mnuSetToDefault";
            this._mnuSetToDefault.Size = new System.Drawing.Size(125, 22);
            this._mnuSetToDefault.Text = "Set to default";
            // 
            // _mnuHelp
            // 
            this._mnuHelp.Name = "_mnuHelp";
            this._mnuHelp.Size = new System.Drawing.Size(125, 22);
            this._mnuHelp.Text = "Details...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(122, 6);
            this._cmsRevertButton.ResumeLayout(false);

        }

        #endregion

        private CtlError _errorProvider;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip _cmsRevertButton;
        private System.Windows.Forms.ToolStripMenuItem _mnuUndoChanges;
        private System.Windows.Forms.ToolStripMenuItem _mnuSetToDefault;
        private System.Windows.Forms.ToolStripMenuItem _mnuHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
