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

        LowAPI.API_Functions.HookProc MouseHoocDelegate;
        int MouseHoocID;
        LowAPI.API_Functions.HookProc KeyboradHoocDelegate;
        int KeyboardHoocID;

        public InRecordForm RecordForm { get; set; }
        //public System.Windows.Forms.TextBox textBox1 { get; set; }
        public ActionsManager UserEvents { get; set; }
		//public MainForm self { get; set; }
		public ITrackingForm self { get; set; }

        public void Init(ITrackingForm formBase)
        {
			this.self = formBase;

			this.RecordForm = new InRecordForm();
			this.RecordForm.engine = this;

			this.UserEvents = new ActionsManager(this.eStatus, this.self);

        }

        public void onLoad()
        {
            this.RecordForm = new InRecordForm();
			this.RecordForm.engine = this;

			UserEvents = new ActionsManager(this.eStatus, this.self);

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

			self.setTextboxText(nCode + "," +
								wParam + "," +
								mouseHookStruct.mouseData + "," +
								mouseHookStruct.pt.x + "," +
								mouseHookStruct.pt.y + "," +
								mouseHookStruct.flags + "," +
								mouseHookStruct.dwExtraInfo);

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

				self.setTextboxText(nCode + "," +
								wParam + "," +
								mouseHookStruct.mouseData + "," +
								mouseHookStruct.pt.x + "," +
								mouseHookStruct.pt.y + "," +
								mouseHookStruct.flags + "," +
								mouseHookStruct.dwExtraInfo + ", " +
								Delay.milisecs);
            }

			return API_Functions.CallNextHookEx(MouseHoocID, nCode, wParam, lParam);
		}


		public DialogResult ShowInRecordDialog()
		{
			RecordForm.TopMost = true;
			RecordForm.TopLevel = true;
			DialogResult dialogResult = RecordForm.ShowDialog();
			RecordForm.BringToFront();

			return dialogResult;
		}


		private int KeyboardHookFunction(int nCode, Int32 wParam, IntPtr lParam)
		{
			LowAPI.API_Structs.KeyboardHookStruct keyboardHookStruct = (LowAPI.API_Structs.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(LowAPI.API_Structs.KeyboardHookStruct));

            self.setTextboxText(nCode + "," +
                            wParam + "," +
                            keyboardHookStruct.scanCode + "," +
                            keyboardHookStruct.vkCode + "," +
                            keyboardHookStruct.time + "," +
                            keyboardHookStruct.flags + "," +
                            keyboardHookStruct.dwExtraInfo );

            if (eStatus.playing)
			{
				if (keyboardHookStruct.scanCode == 69)
				{

					eStatus.stop();
					UserEvents.Pause.Reset();
					if (MessageBox.Show(new Form { TopMost = true }, "Play paused, continue ?", "Tracking message", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
					DialogResult Ret = (DialogResult)self.InvokeDelegate(new ShowInRecordDialogDelegate(ShowInRecordDialog));
					if (Ret == DialogResult.Cancel)
					{
						self.enableDisplay();
						self.setLabelText("");
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


				self.setTextboxText(nCode + "," +
									wParam + "," +
									keyboardHookStruct.scanCode + "," +
									keyboardHookStruct.vkCode + "," +
									keyboardHookStruct.time + "," +
									keyboardHookStruct.flags + "," +
									keyboardHookStruct.dwExtraInfo +
									Delay.milisecs);
            }

			return API_Functions.CallNextHookEx(KeyboardHoocID, nCode, wParam, lParam);
		}

	}
}
