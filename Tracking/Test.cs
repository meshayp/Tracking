
using System;
using System.Threading;
using System.Runtime.InteropServices;
using LowAPI;

namespace InputTest
{

	// It might be better to write this code as a single call to SendInput passing
	// all the key downs and keys ups in one array.  If you do this, SendInput
	// guarantees that no other keys get inbetween the whole sequence.  As is stands,
	// a "real" key transition could in theory sneak in the middle - and if it did
	// it would inherit the states of shift, control, etc this program had set up.

	public class Test
	{
		

		/*[DllImport("user32.dll")]
		static extern IntPtr GetMessageExtraInfo();*/

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);


		const int INPUT_MOUSE = 0;
		const int INPUT_KEYBOARD = 1;
		const int INPUT_HARDWARE = 2;
		const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		const uint KEYEVENTF_KEYUP = 0x0002;
		const uint KEYEVENTF_UNICODE = 0x0004;
		const uint KEYEVENTF_SCANCODE = 0x0008;
		const uint XBUTTON1 = 0x0001;
		const uint XBUTTON2 = 0x0002;
		const uint MOUSEEVENTF_MOVE = 0x0001;
		const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		const uint MOUSEEVENTF_LEFTUP = 0x0004;
		const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		const uint MOUSEEVENTF_XDOWN = 0x0080;
		const uint MOUSEEVENTF_XUP = 0x0100;
		const uint MOUSEEVENTF_WHEEL = 0x0800;
		const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

		public enum VK : ushort
		{
			SHIFT = 0x10,
			CONTROL = 0x11,
			MENU = 0x12,
			ESCAPE = 0x1B,
			BACK = 0x08,
			TAB = 0x09,
			RETURN = 0x0D,
			PRIOR = 0x21,
			NEXT = 0x22,
			END = 0x23,
			HOME = 0x24,
			LEFT = 0x25,
			UP = 0x26,
			RIGHT = 0x27,
			DOWN = 0x28,
			SELECT = 0x29,
			PRINT = 0x2A,
			EXECUTE = 0x2B,
			SNAPSHOT = 0x2C,
			INSERT = 0x2D,
			DELETE = 0x2E,
			HELP = 0x2F,
			NUMPAD0 = 0x60,
			NUMPAD1 = 0x61,
			NUMPAD2 = 0x62,
			NUMPAD3 = 0x63,
			NUMPAD4 = 0x64,
			NUMPAD5 = 0x65,
			NUMPAD6 = 0x66,
			NUMPAD7 = 0x67,
			NUMPAD8 = 0x68,
			NUMPAD9 = 0x69,
			MULTIPLY = 0x6A,
			ADD = 0x6B,
			SEPARATOR = 0x6C,
			SUBTRACT = 0x6D,
			DECIMAL = 0x6E,
			DIVIDE = 0x6F,
			F1 = 0x70,
			F2 = 0x71,
			F3 = 0x72,
			F4 = 0x73,
			F5 = 0x74,
			F6 = 0x75,
			F7 = 0x76,
			F8 = 0x77,
			F9 = 0x78,
			F10 = 0x79,
			F11 = 0x7A,
			F12 = 0x7B,
			OEM_1 = 0xBA,   // ',:' for US
			OEM_PLUS = 0xBB,   // '+' any country
			OEM_COMMA = 0xBC,   // ',' any country
			OEM_MINUS = 0xBD,   // '-' any country
			OEM_PERIOD = 0xBE,   // '.' any country
			OEM_2 = 0xBF,   // '/?' for US
			OEM_3 = 0xC0,   // '`~' for US
			MEDIA_NEXT_TRACK = 0xB0,
			MEDIA_PREV_TRACK = 0xB1,
			MEDIA_STOP = 0xB2,
			MEDIA_PLAY_PAUSE = 0xB3,
			LWIN = 0x5B,
			RWIN = 0x5C
		}

		
		[StructLayout(LayoutKind.Sequential)]
		public struct INPUT
		{
			public int type;
			public int wVk;
			public int wScan;
			public uint dwFlags;
			public int time;
			public int dwExtraInfo;
		};

		public string DoTest()
		{
			INPUT structInput;
			structInput = new INPUT();
			structInput.type = INPUT_KEYBOARD;
			

			uint intReturn  = 0;

		
			string ret = "";
			int size = Marshal.SizeOf(structInput);
			Random rand = new Random();

			structInput.wScan = 30;
			structInput.time = 0;
			structInput.dwFlags = 0;
			structInput.dwExtraInfo = 0;
			structInput.wVk = 65;

			LowAPI.API_Functions.keybd_event(65, 30, 0, 0);

			//intReturn = SendInput(1, ref structInput, 28);
			ret = intReturn.ToString();

			structInput.wScan = 30;
			structInput.dwFlags = 0;
			//intReturn = SendInput(1, ref structInput, 28);
			LowAPI.API_Functions.keybd_event(65, 30, (int)KEYEVENTF_KEYUP, 0);
			ret = intReturn.ToString();

			ret += "," + intReturn.ToString();
			

			if (intReturn == 0)
			{
				int errorCode = Marshal.GetLastWin32Error();
				ret += "," + errorCode;
			}

			return ret;
		}

		public void DoTest2()
		{

		}
	}

}