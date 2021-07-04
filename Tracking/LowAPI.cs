
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace LowAPI
{
	namespace API_Structs
	{
		[StructLayout(LayoutKind.Sequential)]
		public class POINT
		{
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class MouseHookStruct
		{
			public POINT pt;
			public int hwnd;
			public int wHitTestCode;
			public int dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class MouseLLHookStruct
		{
			public POINT pt;
			public int mouseData;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class KeyboardHookStruct
		{
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		public struct Rect
		{
			public int left;
			public int top;
			public int right;
			public int bottom;

			public int Width
			{
				get
				{
					return right - left;
				}
			}

			public int Height
			{
				get
				{
					return bottom - top;
				}
			}
		}

		public struct WindowPlacement
		{
			public int length;
			public int flags;
			public int showCmd;
			public Point minPosition;
			public Point maxPosition;
			public Rectangle normalPosition;
		}

		[StructLayout(LayoutKind.Sequential, Size = 28)]
		public struct MOUSEINPUT
		{
			public int type;
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential, Size=20)]
		public struct KEYBDINPUT
		{
			public int type;
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential, Size=16)]
		public struct HARDWAREINPUT
		{
			public int type;
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}
	}

	public class API_Conts
	{
		public static int MOUSEEVENTF_MOVE       = 0x0001; /* mouse move */
		public static int MOUSEEVENTF_LEFTDOWN   = 0x0002; /* left button down */
		public static int MOUSEEVENTF_LEFTUP     = 0x0004; /* left button up */
		public static int MOUSEEVENTF_RIGHTDOWN  = 0x0008; /* right button down */
		public static int MOUSEEVENTF_RIGHTUP    = 0x0010; /* right button up */
		public static int MOUSEEVENTF_MIDDLEDOWN = 0x0020; /* middle button down */
		public static int MOUSEEVENTF_MIDDLEUP   = 0x0040; /* middle button up */
		public static int MOUSEEVENTF_WHEEL      = 0x0800; /* wheel button rolled */
		public static int MOUSEEVENTF_ABSOLUTE   = 0x8000; /* absolute move */

		public static int WH_MOUSE_LL = 14;
		public static int WH_KEYBOARD_LL = 13;
		public static int WH_MOUSE = 7;
		public static int WH_KEYBOARD = 2;
		public static int WM_MOUSEMOVE = 0x200;
		public static int WM_LBUTTONDOWN = 0x201;
		public static int WM_RBUTTONDOWN = 0x204;
		public static int WM_MBUTTONDOWN = 0x207;
		public static int WM_LBUTTONUP = 0x202;
		public static int WM_RBUTTONUP = 0x205;
		public static int WM_MBUTTONUP = 0x208;
		public static int WM_LBUTTONDBLCLK = 0x203;
		public static int WM_RBUTTONDBLCLK = 0x206;
		public static int WM_MBUTTONDBLCLK = 0x209;
		public static int WM_MOUSEWHEEL = 0x020A;
		public static int WM_KEYDOWN = 0x100;
		public static int WM_KEYUP = 0x101;
		public static int WM_SYSKEYDOWN = 0x104;
		public static int WM_SYSKEYUP = 0x105;
		public static byte VK_SHIFT = 0x10;
		public static byte VK_CAPITAL = 0x14;
		public static byte VK_NUMLOCK = 0x90;

		public static int GWL_EXSTYLE = -20;
		public static int WS_EX_TOOLWINDOW = 0x00000080;
		public static int WS_EX_APPWINDOW = 0x00040000;
	};


	public class API_Functions
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessage(IntPtr hwnd, int wMsg,
			IntPtr wParam,
			IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int SendMessageBlah(IntPtr hwnd, int wMsg,
			IntPtr wParam,
			IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		public static extern int SendMessageFaul(IntPtr hwnd, int wMsg,
			int wParam,
			StringBuilder lParam);

		private const int WM_GETTEXT = 0x000D;
		private const int WM_GETTEXTLENGTH = 0x000E;

		public static string GetText(IntPtr hwnd)
		{
			/*		int lngLength;
					StringBuilder strBuffer = new StringBuilder();
					int lngRet;

					lngLength = SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero) + 1;

					strBuffer.Capacity = lngLength;

					lngRet = SendMessageFaul(hwnd, WM_GETTEXT, strBuffer.Capacity, strBuffer);
					if (lngRet > 0)
						return strBuffer.ToString().Substring(0, lngRet);
					return
						null;*/

			int textLen = LowAPI.API_Functions.GetWindowTextLength(hwnd);
			StringBuilder winText = new StringBuilder(textLen + 1);
			LowAPI.API_Functions.GetWindowText(hwnd, winText, textLen);

			return  hwnd.ToString() + "  " + winText.ToString();
		}

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetWindowTextLength(IntPtr window);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(API_Structs.POINT Point);

        [DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [DllImport("user32.dll")]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32")]
		public static extern int ReleaseCapture(IntPtr hwnd);

		[DllImport("user32.dll")]
		static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

		static public System.Drawing.Color GetPixelColor(int x, int y)
		{
			IntPtr hdc = GetDC(IntPtr.Zero);
			uint pixel = GetPixel(hdc, x, y);
			ReleaseDC(IntPtr.Zero, hdc);
			Color color = Color.FromArgb((int)(pixel & 0x000000FF),
						 (int)(pixel & 0x0000FF00) >> 8,
						 (int)(pixel & 0x00FF0000) >> 16);
			return color;
		}


		[DllImport("user32.dll")]
		public static extern short VkKeyScan(char ch);

		[System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetCursorPos")]
		public extern static Int32 SetCursorPos(Int32 x, Int32 y);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, ref API_Structs.KEYBDINPUT pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, ref API_Structs.MOUSEINPUT pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, ref API_Structs.HARDWAREINPUT pInputs, int cbSize);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		public delegate int HookProc(int nCode, int wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto,  CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int SetWindowsHookEx(int idHook,	HookProc lpfn,	IntPtr hMod,  int dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto,CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		public static extern int UnhookWindowsHookEx(int idHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

		[DllImport("user32")]
		public static extern int ToAscii(int uVirtKey,	int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

		[DllImport("user32")]
		public static extern int GetKeyboardState(byte[] pbKeyState);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern short GetKeyState(int vKey);

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(IntPtr window);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string className, string windowName);

		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		public static extern IntPtr FindWindowWin32(string className, string windowName);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr window, int message, int wparam, int lparam);

		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr window, int message, int wparam, int lparam);

		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr window);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern IntPtr GetLastActivePopup(IntPtr window);

/*		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr window, [In][Out]StringBuilder text, int copyCount);*/

		[DllImport("user32.dll")]
		public static extern bool SetWindowText(IntPtr window, [MarshalAs(UnmanagedType.LPTStr)]string text);

/*		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr window);*/

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr window, int index, int value);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr window, int index);

		public delegate bool EnumWindowsProc(IntPtr window, int i);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, int i);

		[DllImport("user32.dll")]
		public static extern bool EnumThreadWindows(int threadId, EnumWindowsProc callback, int i);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(EnumWindowsProc callback, int i);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr window, ref int processId);

		[DllImport("user32.dll")]
		public static extern int GetWindowThreadProcessId(IntPtr window, IntPtr ptr);

		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr window, ref API_Structs.WindowPlacement position);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPlacement(IntPtr window, ref API_Structs.WindowPlacement position);

		[DllImport("user32.dll")]
		public static extern bool IsChild(IntPtr parent, IntPtr window);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr window);

		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr window);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hwnd);


		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, ref API_Structs.Rect rectangle);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hwnd, ref API_Structs.Rect rectangle);

		[DllImport("gdi32.dll")]
		public static extern UInt64 BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, System.Int32 dwRop);
	}

}