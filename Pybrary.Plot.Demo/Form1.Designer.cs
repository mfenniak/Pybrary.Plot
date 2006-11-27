namespace Pybrary.Plot.Demo
{
    partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.standardPlotControl = new Pybrary.Plot.PlotControl();
            this.standardMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.manyPlotControl = new Pybrary.Plot.PlotControl();
            this.manyMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.datePlotControl = new Pybrary.Plot.PlotControl();
            this.dateMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.stackedPlotControl = new Pybrary.Plot.PlotControl();
            this.stackedMenuStrip = new System.Windows.Forms.MenuStrip();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(631, 484);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.standardPlotControl);
            this.tabPage1.Controls.Add(this.standardMenuStrip);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(623, 458);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Standard XY";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // standardPlotControl
            // 
            this.standardPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.standardPlotControl.Location = new System.Drawing.Point(3, 27);
            this.standardPlotControl.Name = "standardPlotControl";
            this.standardPlotControl.Plot = null;
            this.standardPlotControl.Size = new System.Drawing.Size(617, 428);
            this.standardPlotControl.TabIndex = 0;
            this.standardPlotControl.Text = "plotControl1";
            // 
            // standardMenuStrip
            // 
            this.standardMenuStrip.Location = new System.Drawing.Point(3, 3);
            this.standardMenuStrip.Name = "standardMenuStrip";
            this.standardMenuStrip.Size = new System.Drawing.Size(617, 24);
            this.standardMenuStrip.TabIndex = 1;
            this.standardMenuStrip.Text = "menuStrip1";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.manyPlotControl);
            this.tabPage2.Controls.Add(this.manyMenuStrip);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(623, 458);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Many Y Axes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // manyPlotControl
            // 
            this.manyPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manyPlotControl.Location = new System.Drawing.Point(3, 27);
            this.manyPlotControl.Name = "manyPlotControl";
            this.manyPlotControl.Plot = null;
            this.manyPlotControl.Size = new System.Drawing.Size(617, 428);
            this.manyPlotControl.TabIndex = 0;
            this.manyPlotControl.Text = "plotControl1";
            // 
            // manyMenuStrip
            // 
            this.manyMenuStrip.Location = new System.Drawing.Point(3, 3);
            this.manyMenuStrip.Name = "manyMenuStrip";
            this.manyMenuStrip.Size = new System.Drawing.Size(617, 24);
            this.manyMenuStrip.TabIndex = 1;
            this.manyMenuStrip.Text = "menuStrip1";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.datePlotControl);
            this.tabPage3.Controls.Add(this.dateMenuStrip);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(623, 458);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Date X Axis";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // datePlotControl
            // 
            this.datePlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.datePlotControl.Location = new System.Drawing.Point(3, 27);
            this.datePlotControl.Name = "datePlotControl";
            this.datePlotControl.Plot = null;
            this.datePlotControl.Size = new System.Drawing.Size(617, 428);
            this.datePlotControl.TabIndex = 1;
            this.datePlotControl.Text = "plotControl1";
            // 
            // dateMenuStrip
            // 
            this.dateMenuStrip.Location = new System.Drawing.Point(3, 3);
            this.dateMenuStrip.Name = "dateMenuStrip";
            this.dateMenuStrip.Size = new System.Drawing.Size(617, 24);
            this.dateMenuStrip.TabIndex = 0;
            this.dateMenuStrip.Text = "menuStrip1";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.stackedPlotControl);
            this.tabPage4.Controls.Add(this.stackedMenuStrip);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(623, 458);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Stacked Plot";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // stackedPlotControl
            // 
            this.stackedPlotControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackedPlotControl.Location = new System.Drawing.Point(3, 27);
            this.stackedPlotControl.Name = "stackedPlotControl";
            this.stackedPlotControl.Plot = null;
            this.stackedPlotControl.Size = new System.Drawing.Size(617, 428);
            this.stackedPlotControl.TabIndex = 0;
            this.stackedPlotControl.Text = "plotControl1";
            // 
            // stackedMenuStrip
            // 
            this.stackedMenuStrip.Location = new System.Drawing.Point(3, 3);
            this.stackedMenuStrip.Name = "stackedMenuStrip";
            this.stackedMenuStrip.Size = new System.Drawing.Size(617, 24);
            this.stackedMenuStrip.TabIndex = 1;
            this.stackedMenuStrip.Text = "menuStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 484);
            this.Controls.Add(this.tabControl1);
            this.MainMenuStrip = this.manyMenuStrip;
            this.Name = "Form1";
            this.Text = "Pybrary.Plot Demo";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private PlotControl standardPlotControl;
        private PlotControl manyPlotControl;
        private System.Windows.Forms.MenuStrip manyMenuStrip;
        private System.Windows.Forms.MenuStrip standardMenuStrip;
        private PlotControl datePlotControl;
        private System.Windows.Forms.MenuStrip dateMenuStrip;
        private PlotControl stackedPlotControl;
        private System.Windows.Forms.MenuStrip stackedMenuStrip;

    }
}

