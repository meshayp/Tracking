using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Tracking
{
	public partial class CountDownForm : Form
	{
		public CountDownForm()
		{
			InitializeComponent();
		}

		public void DoCountDown(int seconds)
		{
			label1.Text = seconds.ToString();
			Application.DoEvents();
			this.Show();
			for (int i=seconds; i > 0; i--)
			{
				label1.Text = i.ToString();
				Application.DoEvents();
				Thread.Sleep(1000);
			}
			this.Hide();
		}
	}
}