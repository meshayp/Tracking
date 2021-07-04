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
	public partial class MainForm : Form
	{
		String LabelText;
		LowAPI.API_Functions.HookProc MouseHoocDelegate;
		int MouseHoocID;
		LowAPI.API_Functions.HookProc KeyboradHoocDelegate;
		int KeyboardHoocID;

		//Events UserEvents;
		public ActionsManager UserEvents;

		bool record = false;
		bool play = false;

		InRecordForm RecordForm = new InRecordForm();

		MainForm self;

		public MainForm()
		{
			InitializeComponent();
			//UserEvents = new Events();
			UserEvents = new ActionsManager();
			ActionsManager.ActionLabel = label1;
			self = this;
			RecordForm.Parent = this;
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
            Globals.CalcScreenBounds();

			MouseHoocDelegate = new API_Functions.HookProc(MouseHoocFunction);
			MouseHoocID = LowAPI.API_Functions.SetWindowsHookEx(
				API_Conts.WH_MOUSE_LL,
				MouseHoocDelegate,
                Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                0);

			if (MouseHoocID == 0)
			{
				int errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode);
			}

		
			KeyboradHoocDelegate = new API_Functions.HookProc(KeyboardHookFunction);
			KeyboardHoocID = API_Functions.SetWindowsHookEx(
					API_Conts.WH_KEYBOARD_LL,
					KeyboradHoocDelegate,
					Marshal.GetHINSTANCE(
					Assembly.GetExecutingAssembly().GetModules()[0]),
					0);
			if (KeyboardHoocID == 0)
			{
				int errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode);
			}
		}

		private int MouseHoocFunction(int nCode, int wParam, IntPtr lParam)
		{
			LowAPI.API_Structs.MouseLLHookStruct mouseHookStruct = (LowAPI.API_Structs.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(LowAPI.API_Structs.MouseLLHookStruct));

			textBox1.Text = nCode + "," +
							wParam + "," +
							mouseHookStruct.mouseData + "," +
							mouseHookStruct.pt.x + "," +
							mouseHookStruct.pt.y + "," +
							mouseHookStruct.flags + "," +
							mouseHookStruct.dwExtraInfo;

			if (Globals.Record )
			{
				//UserEvents.AddDelayEvent();
				//UserEvents.AddMouseEvent(nCode, wParam, mouseHookStruct);

				DelayAction Delay = new DelayAction();
				Delay.milisecs = TimeDiff.Get();
				UserEvents.Items.Add(Delay);

				MouseAction mouseAction = new MouseAction();
				mouseAction.SetMouseActionData(nCode, wParam, mouseHookStruct);
				UserEvents.Items.Add(mouseAction);
			}

			return API_Functions.CallNextHookEx(MouseHoocID, nCode, wParam, lParam);
		}


		delegate DialogResult ShowInRecordDialogDelegate();

		public DialogResult ShowInRecordDialog()
		{
			return RecordForm.ShowDialog();
		}


		private int KeyboardHookFunction(int nCode, Int32 wParam, IntPtr lParam)
		{
			LowAPI.API_Structs.KeyboardHookStruct keyboardHookStruct = (LowAPI.API_Structs.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(LowAPI.API_Structs.KeyboardHookStruct));

			textBox1.Text = nCode + "," +
							wParam + "," +
							keyboardHookStruct.scanCode + "," +
							keyboardHookStruct.vkCode + "," +
							keyboardHookStruct.time + "," +
							keyboardHookStruct.flags + "," +
							keyboardHookStruct.dwExtraInfo;

			if (Globals.Play)
			{
				if (keyboardHookStruct.scanCode == 69)
				{
					Globals.Pause.Reset();
					Globals.Play = false;
					if (MessageBox.Show("Play paused, continue ?", "Tracking message", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						Globals.Play = true;
						Globals.Pause.Set();
					}
					else
					{
						Globals.Pause.Set();
					}
				}
			}

			if (Globals.Record)
			{
				if (keyboardHookStruct.scanCode == 69)
				{
					Globals.Record = false;
					DialogResult Ret = (DialogResult)self.Invoke(new ShowInRecordDialogDelegate(ShowInRecordDialog));
					if (Ret == DialogResult.Cancel)
					{
						this.Enabled = true;
						label1.Text = "";
					}
					if (Ret == DialogResult.Retry)
					{
						CountDownForm countdown = new CountDownForm();
						countdown.DoCountDown(3);

						TimeDiff.init();
						Globals.Record = true;
						return API_Functions.CallNextHookEx(KeyboardHoocID, nCode, wParam, lParam);
					}
				}

				/*Globals.Record = keyboardHookStruct.scanCode != 69;
				if (!Globals.Record)
				{
					this.Enabled = true;
					label1.Text = "";
				}*/
			}

			if (Globals.Record)
			{
				DelayAction Delay = new DelayAction();
				Delay.milisecs = TimeDiff.Get();
				UserEvents.Items.Add(Delay);

				KeyboardAction keyboardAction = new KeyboardAction();
				keyboardAction.SetKeyboardActionData(nCode, wParam, keyboardHookStruct);
				UserEvents.Items.Add(keyboardAction);
			}

			return API_Functions.CallNextHookEx(KeyboardHoocID, nCode, wParam, lParam);
		}


		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			int retMouse = API_Functions.UnhookWindowsHookEx(MouseHoocID);
			if (retMouse == 0)
			{
				int errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode);
			}

			retMouse = API_Functions.UnhookWindowsHookEx(KeyboardHoocID);
			if (retMouse == 0)
			{
				int errorCode = Marshal.GetLastWin32Error();
				throw new Win32Exception(errorCode);
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			label1.Text = "";
			textBox7.Text = "";
			CountDownForm countdown = new CountDownForm();
			countdown.DoCountDown(5);

			UserEvents.Items.Clear();
			this.Enabled = false;
			TimeDiff.init();
			Globals.Play = false;
			Globals.Record = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//CountDownForm countdown = new CountDownForm();
			//countdown.DoCountDown();

			Globals.Record = false;
			Globals.Play = true;
			this.Enabled = false;
			UserEvents.Execute();
			this.Enabled = true;

			/*textBox2.Focus();
			int index = 0;
			play = true;
			while ((play) && (index < UserEvents.EventsList.Count))
			{
				switch (UserEvents.EventsList[index].Code)
				{
					case 0 :
					{
						MouseEvent WorkEvent = ((MouseEvent)UserEvents.EventsList[index].Data);
						LowAPI.API_Structs.MOUSEINPUT input = new LowAPI.API_Structs.MOUSEINPUT();
						input.type = 0;
						input.dwFlags = (uint)WorkEvent.flags;
						input.mouseData = (uint)WorkEvent.mouseData;
						input.time = 0;
						input.dwExtraInfo = IntPtr.Zero;

					
						API_Functions.mouse_event(WorkEvent.flags, WorkEvent.x, WorkEvent.y, WorkEvent.wParam, 0);
						break;
					}
					case 1:
					{
						KeyboardEvent WorkEvent = ((KeyboardEvent)UserEvents.EventsList[index].Data);

						LowAPI.API_Structs.KEYBDINPUT input = new LowAPI.API_Structs.KEYBDINPUT();
						input.type = 1;

						input.dwFlags = (uint)WorkEvent.flags;
						input.time = 0;
						input.wScan = (ushort)WorkEvent.wParam;
						input.wVk = (ushort)WorkEvent.vkCode;
						input.dwExtraInfo = IntPtr.Zero;

						
						API_Functions.keybd_event((byte)WorkEvent.vkCode, (byte)WorkEvent.scanCode, WorkEvent.flags, 0);

						break;
					}
					case 2:
					{
						DelayEvent WorkEvent = ((DelayEvent)UserEvents.EventsList[index].Data);
						Thread.Sleep((int)WorkEvent.ticks);
						break;
					}
				}

				Application.DoEvents();
				
				index++;
			}*/
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
				UserEvents.ReadFromFile(textBox7.Text);
			}
		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				textBox7.Text = "";
				UserEvents.WriteToFile(saveFileDialog1.FileName);
				UserEvents.Items.Clear();
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
	}

	public class TimeDiff
	{
		public static DateTime LastTime;
		public static void init()
		{
			LastTime = DateTime.Now;
		}
		public static long Get()
		{
			DateTime NewNow = DateTime.Now;
			TimeSpan Diff = NewNow - LastTime;
			LastTime = NewNow;

			return (long)Diff.TotalMilliseconds;
		}
	};

	public class MouseEvent
	{
		public int nCode;
		public int wParam; 
		public int mouseData;
		public int x;
		public int y;
		public int flags;
		public int dwExtraInfo;
	};

	public class KeyboardEvent
	{
		public int nCode;
		public int wParam;
		public int scanCode;
		public int vkCode;
		public int time;
		public int flags;
		public int dwExtraInfo;
	};

	public class DelayEvent
	{
		public long ticks;
	};

	public class Event
	{
		public int Code;
		public object Data;
	};

	public class Events
	{
		public List<Event> EventsList;

		public Events()
		{
			EventsList = new List<Event>();
		}

		public void AddMouseEvent(int nCode, int wParam, LowAPI.API_Structs.MouseLLHookStruct DataStruct)
		{
			MouseEvent newEvent = new MouseEvent();

			newEvent.dwExtraInfo = DataStruct.dwExtraInfo;
			newEvent.flags = API_Conts.MOUSEEVENTF_ABSOLUTE;
			newEvent.mouseData = 0;
			newEvent.nCode = nCode;
			newEvent.wParam = 0;
			switch(wParam)
			{
				case 512: newEvent.flags = newEvent.flags | API_Conts.MOUSEEVENTF_MOVE;      break;
				case 513: newEvent.flags = newEvent.flags | API_Conts.MOUSEEVENTF_LEFTDOWN;  break;
				case 514: newEvent.flags = newEvent.flags | API_Conts.MOUSEEVENTF_LEFTUP;    break;
				case 516: newEvent.flags = newEvent.flags | API_Conts.MOUSEEVENTF_RIGHTDOWN; break;
				case 517: newEvent.flags = newEvent.flags | API_Conts.MOUSEEVENTF_RIGHTUP;   break;
			}
			//newEvent.x = DataStruct.pt.x * (int)(65600 / 1280);
			//newEvent.y = DataStruct.pt.y * (int)(65600 / 1024);

            newEvent.x = DataStruct.pt.x * (int)(65600 / Globals.ScreenWidth);
            newEvent.y = DataStruct.pt.y * (int)(65600 / Globals.ScreenHeight);

			Event AddEvent = new Event();
			AddEvent.Code = 0;
			AddEvent.Data = (object)newEvent;

			EventsList.Add(AddEvent);
		}

		public void AddKeyboardEvent(int nCode, int wParam, LowAPI.API_Structs.KeyboardHookStruct DataStruct)
		{
			KeyboardEvent newEvent = new KeyboardEvent();

			newEvent.dwExtraInfo = DataStruct.dwExtraInfo;

			if ((DataStruct.flags & 128) != 0)
				newEvent.flags = 2;
			else newEvent.flags = 0;

			newEvent.nCode = nCode;
			newEvent.scanCode = DataStruct.scanCode;
			newEvent.time = DataStruct.time;
			newEvent.vkCode = DataStruct.vkCode;
			newEvent.wParam = wParam;

			Event AddEvent = new Event();
			AddEvent.Code = 1;
			AddEvent.Data = (object)newEvent;

			EventsList.Add(AddEvent);
		}

		public void AddDelayEvent()
		{
			DelayEvent newEvent = new DelayEvent();
			newEvent.ticks = TimeDiff.Get();

			Event AddEvent = new Event();
			AddEvent.Code = 2;
			AddEvent.Data = (object)newEvent;

			EventsList.Add(AddEvent);
		}
	};
}