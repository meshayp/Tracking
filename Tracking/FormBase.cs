using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tracking
{
    public class FormBase
    {
        private MainForm mainForm;

        public FormBase(MainForm mainForm)
        {
            this.mainForm = mainForm;
        }

        public void enableDisplay()
        {
            mainForm.Enabled = true;
        }
        public void disableDisplay()
        {
            mainForm.Enabled = false;
        }

        internal DialogResult Invoke(ShowInRecordDialogDelegate showInRecordDialogDelegate)
        {
            return (DialogResult)mainForm.Invoke(showInRecordDialogDelegate);
        }

        internal void setLabelText(string v)
        {
            mainForm.Text = v;
        }

        internal void setTextboxText(string v)
        {
            if (mainForm.textBox1 != null)
            {
                mainForm.textBox1.Text = v;
            }
        }

        internal void setActionLabelText(string v)
        {
            if (mainForm.label1 != null)
            {
                mainForm.label1.Text = v;
            }
        }
    }

    delegate DialogResult ShowInRecordDialogDelegate();
}
