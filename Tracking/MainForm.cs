using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LowAPI;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Threading;
using Actions;

namespace Tracking
{

    

    public partial class MainForm : Form, ITrackingForm
	{
		//String LabelText;

		TrackingEngine engine = new TrackingEngine();

		//Events UserEvents;
		//public ActionsManager UserEvents;

		//bool record = false;
		//bool play = false;

		//InRecordForm RecordForm = new InRecordForm();

		//MainForm self;

		public MainForm()
		{
			InitializeComponent();

			engine.Init(this);

        }

        

        private void MainForm_Load(object sender, EventArgs e)
		{
			engine.onLoad();

        }



       

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			engine.onClose();

        }

        private void button1_Click(object sender, EventArgs e)
		{
			label1.Text = "";
			textBox7.Text = "";
			if (checkBox1.Checked)
			{
				CountDownForm countdown = new CountDownForm();
				countdown.DoCountDown(5);
			}

			this.Enabled = false;
			
			engine.startRecord();
		}

		private void button2_Click(object sender, EventArgs e)
		{

			engine.eStatus.play();

            this.Enabled = false;
			engine.UserEvents.Execute();
			this.Enabled = true;

	
		}

		private void button5_Click(object sender, EventArgs e)
		{
			InputTest.Test MyTest = new InputTest.Test();
			textBox3.Focus();
			Application.DoEvents();
			Thread.Sleep(10000);
			string ret = MyTest.DoTest();
			textBox4.Text = ret;
		}

		private void button6_Click(object sender, EventArgs e)
		{
			/*int x = int.Parse(textBox5.Text);
			int y = int.Parse(textBox6.Text);
			API_Functions.mouse_event(API_Conts.MOUSEEVENTF_ABSOLUTE | API_Conts.MOUSEEVENTF_MOVE, x, y, 0, 0);*/

			InRecordForm form = new InRecordForm();
			form.ShowDialog();

			/*Thread.Sleep(5000);

			for (int i = 0; i < textBox5.Text.Length; i++)
			{
				LowAPI.API_Structs.KEYBDINPUT input = new LowAPI.API_Structs.KEYBDINPUT();
				input.type = 1;
				input.dwFlags = 0;
				input.time = 0;
				input.wScan = 0;
				input.wVk = (ushort)LowAPI.API_Functions.VkKeyScan(textBox5.Text[i]);
				input.dwExtraInfo = IntPtr.Zero;

				LowAPI.API_Functions.keybd_event((byte)input.wVk, (byte)input.wScan, (int)input.dwFlags, 0);

				input.dwFlags = 2;
				input.dwExtraInfo = IntPtr.Zero;

				LowAPI.API_Functions.keybd_event((byte)input.wVk, (byte)input.wScan, (int)input.dwFlags, 0);
			}*/

			/*API_Functions.keybd_event(161, 54, 0, 0);
			Thread.Sleep(250);
			API_Functions.keybd_event(161, 54, 2, 0);*/
		}

		private void button3_Click(object sender, EventArgs e)
		{
			DialogResult ret = openFileDialog1.ShowDialog();
			if (ret == DialogResult.OK)
			{
				textBox7.Text = openFileDialog1.FileName;
				engine.UserEvents.ReadFromFile(textBox7.Text);
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				textBox7.Text = "";
				engine.UserEvents.WriteToFile(saveFileDialog1.FileName);
				engine.UserEvents.Items.Clear();
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			DialogResult ret = openFileDialog1.ShowDialog();
			if (ret == DialogResult.OK)
			{
				textBox7.Text = openFileDialog1.FileName;
				button3_Click(sender, e);
			}
		}

		public void TestPixelFromScreen()
		{
			IntPtr DesktopWindow = LowAPI.API_Functions.GetDesktopWindow();
			IntPtr DesktopWindowDC = LowAPI.API_Functions.GetWindowDC(DesktopWindow);
			Graphics DesktopDC = Graphics.FromHdc(DesktopWindowDC);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("Mailto:shay.peduim@gmail.com?subject=About tracking system");
		}

        public void enableDisplay()
        {
			this.Enabled = true;
		}

        public void disableDisplay()
        {
			this.Enabled = false;
		}

        public DialogResult InvokeDelegate(ShowInRecordDialogDelegate showInRecordDialogDelegate)
        {
			return (DialogResult)this.Invoke(showInRecordDialogDelegate);
		}

        public void setLabelText(string v)
        {
			this.Text = v;
		}

        public void setTextboxText(string v)
        {
			if (this.textBox1 != null)
            {
				this.textBox1.Text = v;
            }
        }

        public void setActionLabelText(string v)
        {
			if (this.label1 != null)
            {
				this.label1.Text = v;
            }
        }
    }

	






}