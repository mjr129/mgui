namespace MGui
{
    partial class FrmTest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.ctlColourEditor1 = new MGui.Controls.CtlColourEditor();
            this.ctlError1 = new MGui.Controls.CtlError(this.components);
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(56, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Show input box";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ctlColourEditor1
            // 
            this.ctlColourEditor1.Location = new System.Drawing.Point(8, 8);
            this.ctlColourEditor1.Name = "ctlColourEditor1";
            this.ctlColourEditor1.SelectedColor = System.Drawing.SystemColors.Control;
            this.ctlColourEditor1.TabIndex = 0;
            this.ctlColourEditor1.UseVisualStyleBackColor = true;
            this.ctlColourEditor1.Click += new System.EventHandler(this.ctlColourEditor1_Click);
            // 
            // FrmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 656);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ctlColourEditor1);
            this.Name = "FrmTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmTest";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CtlColourEditor ctlColourEditor1;
        private Controls.CtlError ctlError1;
        private System.Windows.Forms.Button button1;
    }
}