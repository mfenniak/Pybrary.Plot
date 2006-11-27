namespace Pybrary.Plot
{
    partial class ManualNumericAxisForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.minValueBox = new System.Windows.Forms.TextBox();
            this.minManualButton = new System.Windows.Forms.RadioButton();
            this.minAutoButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.maxValueBox = new System.Windows.Forms.TextBox();
            this.maxManualButton = new System.Windows.Forms.RadioButton();
            this.maxAutoButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.minValueBox);
            this.groupBox1.Controls.Add(this.minManualButton);
            this.groupBox1.Controls.Add(this.minAutoButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(123, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Minimum:";
            // 
            // minValueBox
            // 
            this.minValueBox.Location = new System.Drawing.Point(12, 65);
            this.minValueBox.Name = "minValueBox";
            this.minValueBox.Size = new System.Drawing.Size(100, 20);
            this.minValueBox.TabIndex = 3;
            // 
            // minManualButton
            // 
            this.minManualButton.AutoSize = true;
            this.minManualButton.Location = new System.Drawing.Point(6, 42);
            this.minManualButton.Name = "minManualButton";
            this.minManualButton.Size = new System.Drawing.Size(60, 17);
            this.minManualButton.TabIndex = 2;
            this.minManualButton.TabStop = true;
            this.minManualButton.Text = "Manual";
            this.minManualButton.UseVisualStyleBackColor = true;
            this.minManualButton.CheckedChanged += new System.EventHandler(this.minManualButton_CheckedChanged);
            // 
            // minAutoButton
            // 
            this.minAutoButton.AutoSize = true;
            this.minAutoButton.Location = new System.Drawing.Point(6, 19);
            this.minAutoButton.Name = "minAutoButton";
            this.minAutoButton.Size = new System.Drawing.Size(72, 17);
            this.minAutoButton.TabIndex = 1;
            this.minAutoButton.TabStop = true;
            this.minAutoButton.Text = "Automatic";
            this.minAutoButton.UseVisualStyleBackColor = true;
            this.minAutoButton.CheckedChanged += new System.EventHandler(this.minAutoButton_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.maxValueBox);
            this.groupBox2.Controls.Add(this.maxManualButton);
            this.groupBox2.Controls.Add(this.maxAutoButton);
            this.groupBox2.Location = new System.Drawing.Point(141, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(123, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Maximum:";
            // 
            // maxValueBox
            // 
            this.maxValueBox.Location = new System.Drawing.Point(12, 65);
            this.maxValueBox.Name = "maxValueBox";
            this.maxValueBox.Size = new System.Drawing.Size(100, 20);
            this.maxValueBox.TabIndex = 6;
            // 
            // maxManualButton
            // 
            this.maxManualButton.AutoSize = true;
            this.maxManualButton.Location = new System.Drawing.Point(6, 42);
            this.maxManualButton.Name = "maxManualButton";
            this.maxManualButton.Size = new System.Drawing.Size(60, 17);
            this.maxManualButton.TabIndex = 5;
            this.maxManualButton.TabStop = true;
            this.maxManualButton.Text = "Manual";
            this.maxManualButton.UseVisualStyleBackColor = true;
            this.maxManualButton.CheckedChanged += new System.EventHandler(this.maxManualButton_CheckedChanged);
            // 
            // maxAutoButton
            // 
            this.maxAutoButton.AutoSize = true;
            this.maxAutoButton.Location = new System.Drawing.Point(6, 19);
            this.maxAutoButton.Name = "maxAutoButton";
            this.maxAutoButton.Size = new System.Drawing.Size(72, 17);
            this.maxAutoButton.TabIndex = 4;
            this.maxAutoButton.TabStop = true;
            this.maxAutoButton.Text = "Automatic";
            this.maxAutoButton.UseVisualStyleBackColor = true;
            this.maxAutoButton.CheckedChanged += new System.EventHandler(this.maxAutoButton_CheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(58, 118);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(139, 118);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ManualNumericAxisForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(273, 153);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManualNumericAxisForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manual Axis Scaling";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox minValueBox;
        private System.Windows.Forms.RadioButton minManualButton;
        private System.Windows.Forms.RadioButton minAutoButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox maxValueBox;
        private System.Windows.Forms.RadioButton maxManualButton;
        private System.Windows.Forms.RadioButton maxAutoButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}