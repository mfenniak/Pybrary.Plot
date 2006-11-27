using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Pybrary.Plot
{
    public partial class ManualNumericAxisForm : Form
    {
        private NumericAxis axis;

        public ManualNumericAxisForm(NumericAxis axis)
        {
            this.axis = axis;

            InitializeComponent();

            minAutoButton.Checked = (axis.UserMinimum == null);
            minManualButton.Checked = !minAutoButton.Checked;
            maxAutoButton.Checked = (axis.UserMaximum == null);
            maxManualButton.Checked = !maxAutoButton.Checked;
            if (axis.UserMinimum.HasValue)
                minValueBox.Text = axis.FormatLabel(axis.UserMinimum.Value);
            if (axis.UserMaximum.HasValue)
                maxValueBox.Text = axis.FormatLabel(axis.UserMaximum.Value);
            UpdateEnabling();
        }

        private void UpdateEnabling()
        {
            minValueBox.Enabled = minManualButton.Checked;
            maxValueBox.Enabled = maxManualButton.Checked;
        }

        private void maxAutoButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabling();
        }

        private void maxManualButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabling();
        }

        private void minManualButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabling();
        }

        private void minAutoButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabling();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (minManualButton.Checked)
                    axis.UserMinimum = Convert.ToDouble(minValueBox.Text);
                else
                    axis.UserMinimum = null;

                if (maxManualButton.Checked)
                    axis.UserMaximum = Convert.ToDouble(maxValueBox.Text);
                else
                    axis.UserMaximum = null;

                if (axis.UserMaximum.HasValue && axis.UserMinimum.HasValue && axis.UserMinimum.Value > axis.UserMaximum.Value)
                {
                    // Probably entered max in min field, and vice versa.  Auto-swap.
                    double value = axis.UserMaximum.Value;
                    axis.UserMaximum = axis.UserMinimum;
                    axis.UserMinimum = value;
                }

                Dispose();
            }
            catch (FormatException)
            {
                MessageBox.Show(this, "You must enter number values.", "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}