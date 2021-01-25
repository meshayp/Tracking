using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Actions;
using System.IO;

namespace Tracking
{
	public partial class InRecordForm : Form
	{
		public MainForm Parent;

		/*public InRecordForm(MainForm parent)
		{
			Parent = parent;
		}*/

		public InRecordForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Retry;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			ShowMessageAction message = new ShowMessageAction();
			message.Title = textBox1.Text;
			message.Content = textBox2.Text;
			Parent.UserEvents.Items.Add(message);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			int counter = 1;
			String FileName = textBox4.Text;
			while (File.Exists(FileName))
			{
				FileName = textBox4.Text;
				FileName = FileName.Insert(textBox4.Text.IndexOf('.'), counter.ToString());
				counter++;
			}

			Parent.saveFileDialog1.FileName = FileName;
			if (Parent.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Parent.UserEvents.WriteToFile(Parent.saveFileDialog1.FileName);
				Parent.UserEvents.Items.Clear();
			}
		}

		bool bMouseDown = false;
		int MouseXPos;
		int MouseYPos;
		Byte ColorR;
		Byte ColorG;
		Byte ColorB;

		private void InRecordForm_MouseDown(object sender, MouseEventArgs e)
		{
			LowAPI.API_Functions.SetCapture(this.Handle);
			Cursor = Cursors.Hand;
			bMouseDown = true;
		}

		private void InRecordForm_MouseUp(object sender, MouseEventArgs e)
		{
			LowAPI.API_Functions.ReleaseCapture(this.Handle);
			Cursor = Cursors.Default;
			bMouseDown = false;
		}

		private void InRecordForm_MouseMove(object sender, MouseEventArgs e)
		{
			if (bMouseDown)
			{
				MouseXPos = e.X + this.Left + 4;
				MouseYPos = e.Y + this.Top + 29;
				Color color = LowAPI.API_Functions.GetPixelColor(MouseXPos, MouseYPos);
				ColorR = color.R;
				ColorG = color.G;
				ColorB = color.B;
				textBox3.Text = "(R,G,B)=(" + ColorR + "," + ColorG + "," + ColorG + ")" + "     (X,Y)=("+MouseXPos+","+MouseYPos+")";
				textBox3.Update();

				LowAPI.API_Structs.POINT pt = new LowAPI.API_Structs.POINT();
				pt.x = 100;
				pt.y = 100;
				IntPtr wnd = IntPtr.Zero;
				wnd = LowAPI.API_Functions.WindowFromPoint(pt);
				if (wnd != IntPtr.Zero)
				{
					string str = LowAPI.API_Functions.GetText(wnd);
					textBox5.Text = str;
				}
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{
			WaitForColorAction ForColor = new WaitForColorAction();

			ForColor.x = MouseXPos;
			ForColor.y = MouseYPos;
			ForColor.R = ColorR;
			ForColor.G = ColorG;
			ForColor.B = ColorB;

			Parent.UserEvents.Items.Add(ForColor);

		}

		private void button6_Click(object sender, EventArgs e)
		{
			WaitWhileColorAction WhileColor = new WaitWhileColorAction();

			WhileColor.x = MouseXPos;
			WhileColor.y = MouseYPos;
			WhileColor.R = ColorR;
			WhileColor.G = ColorG;
			WhileColor.B = ColorB;

			Parent.UserEvents.Items.Add(WhileColor);
		}

		private void InRecordForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			
		}

		private void InRecordForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			//this.DialogResult = DialogResult.Abort;
		}
	}
}