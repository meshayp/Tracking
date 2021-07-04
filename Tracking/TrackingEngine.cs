using Actions;
using LowAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tracking
{
	
    class TrackingEngine
    {
		public EngineStatus eStatus = new EngineStatus();

		/*public void play(bool status)
        {
			eStatus.play();
        }

        public void record(bool status)
        {
			eStatus.record();
        }*/

        LowAPI.API_Functions.HookProc MouseHoocDelegate;
        int MouseHoocID;
        LowAPI.API_Functions.HookProc KeyboradHoocDelegate;
        int KeyboardHoocID;

        public InRecordForm RecordForm { get; set; }
        public System.Windows.Forms.TextBox textBox1 { get; set; }
        public ActionsManager UserEvents { get; set; }
        public MainForm self { get; set; }

        public void onLoad()
        {
            this.RecordForm = new InRecordForm();
			this.RecordForm.engine = this;

            UserEvents = new ActionsManager();
			UserEvents.eStatus = this.eStatus;

			Globals.CalcScreenBounds();

			TimeDiff.init();

            MouseHoocDelegate = new API_Functions.HookProc(MouseHoocFunction);
            MouseHoocID = LowAPI.API_Functions.SetWindowsHookEx(
                API_Conts.WH_MOUSE_LL,
                MouseHoocDelegate,
                        Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);

            if (MouseHoocID == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }


            KeyboradHoocDelegate = new API_Functions.HookProc(KeyboardHookFunction);
            KeyboardHoocID = API_Functions.SetWindowsHookEx(
                    API_Conts.WH_KEYBOARD_LL,
                    KeyboradHoocDelegate,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            if (KeyboardHoocID == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        internal void startRecord()
        {
			this.UserEvents.Items.Clear();
			TimeDiff.init();
			eStatus.record();
		}

		public void stopRecord()
        {
			eStatus.stop();

		}

        public void onClose()
        {
            int retMouse = API_Functions.UnhookWindowsHookEx(KeyboardHoocID);
            if (retMouse == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }

            retMouse = API_Functions.UnhookWindowsHookEx(MouseHoocID);
            if (retMouse == 0)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }



        private int MouseHoocFunction(int nCode, Int32 wParam, IntPtr lParam)
		{
			LowAPI.API_Structs.MouseLLHookStruct mouseHookStruct = (LowAPI.API_Structs.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(LowAPI.API_Structs.MouseLLHookStruct));

			if (textBox1 != null)
			{
				textBox1.Text = nCode + "," +
								wParam + "," +
								mouseHookStruct.mouseData + "," +
								mouseHookStruct.pt.x + "," +
								mouseHookStruct.pt.y + "," +
								mouseHookStruct.flags + "," +
								mouseHookStruct.dwExtraInfo;
			}

			if (eStatus.recording)
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

			if (textBox1 != null)
			{
				textBox1.Text = nCode + "," +
								wParam + "," +
								keyboardHookStruct.scanCode + "," +
								keyboardHookStruct.vkCode + "," +
								keyboardHookStruct.time + "," +
								keyboardHookStruct.flags + "," +
								keyboardHookStruct.dwExtraInfo;
			}

			if (eStatus.playing)
			{
				if (keyboardHookStruct.scanCode == 69)
				{

					UserEvents.Pause.Reset();
					eStatus.stop(); 
					if (MessageBox.Show("Play paused, continue ?", "Tracking message", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						eStatus.play(); 
					}

					UserEvents.Pause.Set();
				}
			}

			if (eStatus.recording)
			{
				if (keyboardHookStruct.scanCode == 69)
				{
					eStatus.stop();
					DialogResult Ret = (DialogResult)self.Invoke(new ShowInRecordDialogDelegate(ShowInRecordDialog));
					if (Ret == DialogResult.Cancel)
					{
						self.Enabled = true;
						self.label1.Text = "";
					}
					if (Ret == DialogResult.Retry)
					{
						CountDownForm countdown = new CountDownForm();
						countdown.DoCountDown(3);

						TimeDiff.init();
						eStatus.record(); 
						return API_Functions.CallNextHookEx(KeyboardHoocID, nCode, wParam, lParam);
					}
				}

			}

			if (eStatus.recording)
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

	}
}
