
using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;
using Tracking;
using System.IO;

namespace Actions
{
    public class Globals
    {
        public static bool Play;
        public static bool Record;

        public static string Seperator = "|";

		public static ManualResetEvent Pause = new ManualResetEvent(true);

        public static int ScreenWidth = 0;
        public static int ScreenHeight = 0;

        public static void CalcScreenBounds()
        {
            int tempWidth;
            int tempHeight;

            for (int i=0; i < Screen.AllScreens.Length; i++)
            {
                tempWidth = 0;
                if (Screen.AllScreens[i].Bounds.X > 0)
                    tempWidth = Screen.AllScreens[i].Bounds.Width + Screen.AllScreens[i].Bounds.X;
                else tempWidth = Screen.AllScreens[i].Bounds.Width;

                if (tempWidth > ScreenWidth)
                    ScreenWidth = tempWidth;

                tempHeight = 0;
                if (Screen.AllScreens[i].Bounds.Y > 0)
                    tempHeight = Screen.AllScreens[i].Bounds.Height + Screen.AllScreens[i].Bounds.Y;
                else tempHeight = Screen.AllScreens[i].Bounds.Height;

                if (tempHeight > ScreenHeight)
                    ScreenHeight = tempHeight;
            }

            ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
        }
    }

    public class ActionsFactory
    {
        Hashtable ActionsHash;

        public ActionsFactory()
        {
            ActionsHash = new Hashtable();

            AddAction(new DelayAction());
            AddAction(new RunActionFileAction());
            AddAction(new IncreaceNumberAction());
            AddAction(new AssignVarAction());
            AddAction(new ReadRegistryAction());
            AddAction(new WriteRegistryAction());
			AddAction(new MouseAction());
			AddAction(new KeyboardAction());
			AddAction(new WaitWhileColorAction());
			AddAction(new WaitForColorAction());
			AddAction(new KeyboardStringAction());
			AddAction(new ShowMessageAction());
			AddAction(new RunDirectoryAction());
        }

        public void AddAction(Action action)
        {
            ActionsHash.Add(action.GetName(), action);
        }

        public Action ParseLine(String line)
        {
            String[] args = line.Split(new string[] { Globals.Seperator }, StringSplitOptions.None);
            Action action = null;

            if (ActionsHash.ContainsKey(args[0]))
            {
                action = ((Action)ActionsHash[args[0]]).NewInstance();
                action.Parse(args);
            }

            return action;
        }
    }

    public class Variables
    {
        public static Hashtable Vars = new Hashtable();

        public static void Set(String name, String value)
        {
            if (Vars.ContainsKey(name))
            {
                Vars[name] = value;
            }
            else
            {
                Vars.Add(name, value);
            }
        }

        public static String Get(String name)
        {
            if (Vars.ContainsKey(name))
            {
                return (String)Vars[name];
            }
            else
            {
                return "";
            }
        }

        public static String GetString(String arg)
        {
            foreach (DictionaryEntry de in Vars)
            {
                arg = arg.Replace((String)de.Key, (String)de.Value);
            }

            return arg;
        }
    }

    public class ActionsManager
    {
        public List<Action> Items;
        ActionsFactory actionsFactory;
		static public Label ActionLabel = null;

        public ActionsManager()
        {
            Items = new List<Action>();
            actionsFactory = new ActionsFactory();
        }

        public void WriteToFile(String FileName)
        {
            StreamWriter file = new StreamWriter(FileName);
            for (int i = 0; i < Items.Count; i++)
            {
                file.WriteLine(Items[i].ToString());
            }
            file.Close();
        }

        public void ReadFromFile(String FileName)
        {
			Items.Clear();
            StreamReader file = new StreamReader(FileName);
            String newLine;// = file.ReadLine();
            //while (!file.EndOfStream)
			do
			{
				newLine = file.ReadLine();
				Items.Add(actionsFactory.ParseLine(newLine));
			}
			while (!file.EndOfStream);
            file.Close();
        }

        public int Execute()
        {
            for (int i = 0; ((i < Items.Count)&&(Globals.Play)); i++)
            {
				Globals.Pause.WaitOne();

				if (ActionLabel != null)
				{
					ActionLabel.Text = Items[i].ToString();
					Application.DoEvents();
				}
                Items[i].Execute();
            }

            return 0;
        }
    }

    public interface Action
    {
        String GetName();
        int Parse(String[] args);
        int Execute();
        String ToString();
        Action NewInstance();
    }

    public class DelayAction : Action
    {
        public String GetName()
        {
            return "Delay";
        }

        public long milisecs;

        public int Parse(String[] args)
        {
            //milisecs = long.Parse(  Variables.GetString(args[1]) );
			//milisecs -= 75;

			milisecs = long.Parse(args[1]);

            return 0;
        }

        public int Execute()
        {
			milisecs -= 10;
			if (milisecs > 0)
				Thread.Sleep((int)milisecs*2);

            return 0;
        }

        public override String ToString()
        {
            return GetName() + Globals.Seperator + milisecs.ToString();
        }

        public Action NewInstance()
        {
            return new DelayAction();
        }
    }

    public class RunActionFileAction : Action
    {
        public String GetName()
        {
            return "RunActionFile";
        }

        String FileName;

        public int Parse(String[] args)
        {
            FileName = Variables.GetString(args[1]);

            return 0;
        }

        public int Execute()
        {
            ActionsManager actionsManager = new ActionsManager();
            actionsManager.ReadFromFile(Variables.GetString(FileName));
            actionsManager.Execute();
            return 0;
        }

        public override String ToString()
        {
            return GetName() + Globals.Seperator + FileName;
        }

        public Action NewInstance()
        {
            return new RunActionFileAction();
        }
    }

    public class IncreaceNumberAction : Action
    {
        public String GetName()
        {
            return "AddNumber";
        }

        String VarName;
        String number;
		long TempNumber;

        public int Parse(String[] args)
        {
            VarName = args[1];
            //number = int.Parse( Variables.GetString(args[2]));
			number = args[2];

            return 0;
        }

        public int Execute()
        {
            int ret = 0;
            try
            {
				TempNumber = long.Parse(Variables.GetString(number));
                Variables.Set(VarName, (long.Parse(Variables.Get(VarName)) + TempNumber).ToString());
            }
            catch
            {
                ret = 1;
            }

            return ret;
        }

        public override String ToString()
        {
            return GetName() + Globals.Seperator + VarName + Globals.Seperator + number;
        }

        public Action NewInstance()
        {
			return new IncreaceNumberAction();
        }
    }

    public class AssignVarAction : Action
    {
        public String GetName()
        {
            return "AssignVar";
        }

        String VarName;
        String Value;
		String TempVal;

        public int Parse(String[] args)
        {
            VarName = args[1];
            //Value = Variables.GetString(args[2]);
			Value = args[2];

            return 0;
        }

        public int Execute()
        {
			TempVal = Variables.GetString(Value);
            Variables.Set(VarName, TempVal);
           
            return 0;
        }

        public override String ToString()
        {
            return GetName() + Globals.Seperator + VarName + Globals.Seperator + Value;
        }

        public Action NewInstance()
        {
            return new AssignVarAction();
        }
    }

    public class ReadRegistryAction : Action
    {
        public String GetName()
        {
            return "ReadRegistry";
        }

        String VarName;
        String RegistryPath;
        String ValueName;

		String TempRegistryPath;
		String TempValueName;

        public int Parse(String[] args)
        {
            VarName = args[1];
            //RegistryPath = Variables.GetString(args[2]);
            //ValueName = Variables.GetString(args[3]);
			RegistryPath = args[2];
			ValueName = args[3];

            return 0;
        }

        public int Execute()
        {
			TempRegistryPath = Variables.GetString(RegistryPath);
			TempValueName = Variables.GetString(ValueName);

			String value = (String)Registry.GetValue(TempRegistryPath, TempValueName, "");
			Variables.Set(VarName, value);

            return 0;
        }

        public override String ToString()
        {
            return GetName() + Globals.Seperator + VarName + Globals.Seperator + RegistryPath + Globals.Seperator + ValueName;
        }

        public Action NewInstance()
        {
            return new ReadRegistryAction();
        }
    }

	public class WriteRegistryAction : Action
	{
		public String GetName()
		{
			return "WriteRegistry";
		}

		String VarName;
		String RegistryPath;
		String ValueName;

		String TempRegistryPath;
		String TempValueName;

		public int Parse(String[] args)
		{
			VarName = args[1];
			//RegistryPath = Variables.GetString(args[2]);
			//ValueName = Variables.GetString(args[3]);

			RegistryPath = args[2];
			ValueName = args[3];

			return 0;
		}

		public int Execute()
		{
			TempRegistryPath = Variables.GetString(RegistryPath);
			TempValueName = Variables.GetString(ValueName);

			Registry.SetValue(TempRegistryPath, TempValueName, Variables.Get(VarName));

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator + VarName + Globals.Seperator + RegistryPath + Globals.Seperator + ValueName;
		}

		public Action NewInstance()
		{
			return new WriteRegistryAction();
		}
	}

	public class MouseAction : Action
	{
		public String GetName()
		{
			return "Mouse";
		}

		Tracking.MouseEvent mouseEvent;

		public void SetMouseActionData(int nCode, int wParam, LowAPI.API_Structs.MouseLLHookStruct DataStruct)
		{
			mouseEvent = new Tracking.MouseEvent();

			mouseEvent.dwExtraInfo = DataStruct.dwExtraInfo;
			mouseEvent.flags = LowAPI.API_Conts.MOUSEEVENTF_ABSOLUTE;
			mouseEvent.mouseData = 0;
			mouseEvent.nCode = nCode;
			mouseEvent.wParam = 0;
			switch (wParam)
			{
				case 512: mouseEvent.flags = mouseEvent.flags | LowAPI.API_Conts.MOUSEEVENTF_MOVE; break;
				case 513: mouseEvent.flags = mouseEvent.flags | LowAPI.API_Conts.MOUSEEVENTF_LEFTDOWN; break;
				case 514: mouseEvent.flags = mouseEvent.flags | LowAPI.API_Conts.MOUSEEVENTF_LEFTUP; break;
				case 516: mouseEvent.flags = mouseEvent.flags | LowAPI.API_Conts.MOUSEEVENTF_RIGHTDOWN; break;
				case 517: mouseEvent.flags = mouseEvent.flags | LowAPI.API_Conts.MOUSEEVENTF_RIGHTUP; break;
			}
			mouseEvent.x = DataStruct.pt.x; // *(int)(65600 / 1280);
			mouseEvent.y = DataStruct.pt.y; // *(int)(65600 / 1024);
		}
	
		public int Parse(String[] args)
		{
			mouseEvent = new Tracking.MouseEvent();

			mouseEvent.dwExtraInfo = int.Parse(args[1]);
			mouseEvent.flags = int.Parse(args[2]);
			mouseEvent.mouseData = int.Parse(args[3]);
			mouseEvent.nCode = int.Parse(args[4]);
			mouseEvent.wParam = int.Parse(args[5]);
			mouseEvent.flags = int.Parse(args[6]);
			mouseEvent.x = int.Parse(args[7]);
			mouseEvent.y = int.Parse(args[8]);

			return 0;
		}

		public int Execute()
		{
			LowAPI.API_Structs.MOUSEINPUT input = new LowAPI.API_Structs.MOUSEINPUT();
			input.type = 0;
			input.dwFlags = (uint)mouseEvent.flags;
			input.mouseData = (uint)mouseEvent.mouseData;
			input.time = 0;
			input.dwExtraInfo = IntPtr.Zero;
			input.dx =  (int) (65536.0 / Globals.ScreenWidth * mouseEvent.x); //* (int)(65600 / Globals.ScreenWidth);
			input.dy = (int) (65536.0 / Globals.ScreenHeight * mouseEvent.y); //* (int)(65600 / Globals.ScreenHeight);

			LowAPI.API_Functions.mouse_event(mouseEvent.flags, input.dx, input.dy, mouseEvent.wParam, 0);

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator +
			mouseEvent.dwExtraInfo.ToString() + Globals.Seperator +
			mouseEvent.flags.ToString() + Globals.Seperator +
			mouseEvent.mouseData.ToString() + Globals.Seperator +
			mouseEvent.nCode.ToString() + Globals.Seperator +
			mouseEvent.wParam.ToString() + Globals.Seperator +
			mouseEvent.flags.ToString() + Globals.Seperator +
			mouseEvent.x.ToString() + Globals.Seperator +
			mouseEvent.y.ToString();
		}

		public Action NewInstance()
		{
			return new MouseAction();
		}
	}

	public class KeyboardAction : Action
	{
		public String GetName()
		{
			return "Keyboard";
		}

		Tracking.KeyboardEvent keyboardEvent;

		public void SetKeyboardActionData(int nCode, int wParam, LowAPI.API_Structs.KeyboardHookStruct DataStruct)
		{
			keyboardEvent = new Tracking.KeyboardEvent();

			keyboardEvent.dwExtraInfo = DataStruct.dwExtraInfo;

			if ((DataStruct.flags & 128) != 0)
				keyboardEvent.flags = 2;
			else keyboardEvent.flags = 0;

			keyboardEvent.nCode = nCode;
			keyboardEvent.scanCode = DataStruct.scanCode;
			keyboardEvent.time = DataStruct.time;
			keyboardEvent.vkCode = DataStruct.vkCode;
			keyboardEvent.wParam = wParam;
		}
		
		public int Parse(String[] args)
		{
			keyboardEvent = new Tracking.KeyboardEvent();

			keyboardEvent.dwExtraInfo = int.Parse(args[1]);
			keyboardEvent.flags = int.Parse(args[2]);
			keyboardEvent.nCode = int.Parse(args[3]);
			keyboardEvent.scanCode = int.Parse(args[4]);
			keyboardEvent.time = int.Parse(args[5]);
			keyboardEvent.vkCode = int.Parse(args[6]);
			keyboardEvent.wParam = int.Parse(args[7]);
			return 0;
		}

		public int Execute()
		{
			LowAPI.API_Structs.KEYBDINPUT input = new LowAPI.API_Structs.KEYBDINPUT();
			input.type = 1;
			input.dwFlags = (uint)keyboardEvent.flags;
			input.time = 0;
			input.wScan = (ushort)keyboardEvent.wParam;
			input.wVk = (ushort)keyboardEvent.vkCode;
			input.dwExtraInfo = IntPtr.Zero;

			LowAPI.API_Functions.keybd_event((byte)keyboardEvent.vkCode, (byte)keyboardEvent.scanCode, keyboardEvent.flags, 0);

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator +
			keyboardEvent.dwExtraInfo.ToString() + Globals.Seperator +
			keyboardEvent.flags.ToString() + Globals.Seperator +
			keyboardEvent.nCode.ToString() + Globals.Seperator +
			keyboardEvent.scanCode.ToString() + Globals.Seperator +
			keyboardEvent.time.ToString() + Globals.Seperator +
			keyboardEvent.vkCode.ToString() + Globals.Seperator +
			keyboardEvent.wParam.ToString();
		}

		public Action NewInstance()
		{
			return new KeyboardAction();
		}
	}

	public class WaitWhileColorAction : Action
	{
		public String GetName()
		{
			return "WaitWhileColor";
		}

		public int x;
		public int y;
		public int R;
		public int G;
		public int B;
		
		public int Parse(String[] args)
		{
			x = int.Parse(args[1]);
			y = int.Parse(args[2]);
			R = int.Parse(args[3]);
			G = int.Parse(args[4]);
			B = int.Parse(args[5]);

			return 0;
		}

		public int Execute()
		{
			Color TestColor = LowAPI.API_Functions.GetPixelColor(x, y);
			while ((TestColor.R == R) && (TestColor.G == G) && (TestColor.B == B))
			{
				Thread.Sleep(500);
				TestColor = LowAPI.API_Functions.GetPixelColor(x, y);
			}

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator +
			x.ToString() + Globals.Seperator +
			y.ToString() + Globals.Seperator +
			R.ToString() + Globals.Seperator +
			G.ToString() + Globals.Seperator +
			B.ToString();
		}

		public Action NewInstance()
		{
			return new WaitWhileColorAction();
		}
	}

	public class WaitForColorAction : Action
	{
		public String GetName()
		{
			return "WaitForColor";
		}

		public int x;
		public int y;
		public int R;
		public int G;
		public int B;
		
		public int Parse(String[] args)
		{
			x = int.Parse(args[1]);
			y = int.Parse(args[2]);
			R = int.Parse(args[3]);
			G = int.Parse(args[4]);
			B = int.Parse(args[5]);

			return 0;
		}

		public int Execute()
		{
			Color TestColor = LowAPI.API_Functions.GetPixelColor(x, y);
			while ((TestColor.R != R) && (TestColor.G != G) && (TestColor.B != B))
			{
				Thread.Sleep(500);
				TestColor = LowAPI.API_Functions.GetPixelColor(x, y);
			}

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator +
			x.ToString() + Globals.Seperator +
			y.ToString() + Globals.Seperator +
			R.ToString() + Globals.Seperator +
			G.ToString() + Globals.Seperator +
			B.ToString();
		}

		public Action NewInstance()
		{
			return new WaitForColorAction();
		}
	}

	

	
	public class KeyboardStringAction : Action
	{
		public String GetName()
		{
			return "KeyboardString";
		}

		String str;
		
		public int Parse(String[] args)
		{
			str = args[1];

			return 0;
		}

		public int Execute()
		{
			String InputString = Variables.GetString(str);

			LowAPI.API_Structs.KEYBDINPUT input = new LowAPI.API_Structs.KEYBDINPUT();
			input.type = 1;
			input.time = 0;
			input.wScan = 0;

			for (int i = 0; i < InputString.Length; i++)
			{
				Thread.Sleep(20);

				input.dwFlags = 0;
				input.wVk = (ushort)LowAPI.API_Functions.VkKeyScan(InputString[i]);
				LowAPI.API_Functions.keybd_event((byte)input.wVk, (byte)input.wScan, (int)input.dwFlags, 0);

				Thread.Sleep(20);

				input.dwFlags = 2;
				LowAPI.API_Functions.keybd_event((byte)input.wVk, (byte)input.wScan, (int)input.dwFlags, 0);
			}

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator + str;
		}

		public Action NewInstance()
		{
			return new KeyboardStringAction();
		}
	}

	public class ShowMessageAction : Action
	{
		public String GetName()
		{
			return "ShowMessage";
		}

		public String Title;
		public String Content;

		public int Parse(String[] args)
		{
			Title = args[1];
			Content = args[2];

			return 0;
		}

		public int Execute()
		{
			MessageBox.Show(Variables.GetString(Content), Variables.GetString(Title));

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator + Title + Globals.Seperator + Content;
		}

		public Action NewInstance()
		{
			return new ShowMessageAction();
		}
	}

	public class RunDirectoryAction : Action
	{
		public String GetName()
		{
			return "RunDirectory";
		}

		public String Dir;

		public int Parse(String[] args)
		{
			Dir = args[1];

			return 0;
		}

		public int Execute()
		{
			if ( Dir[Dir.Length-1] != '\\')
			{
				Dir += '\\';
			}
			String[] files = Directory.GetFiles(Variables.GetString(Dir));
			ActionsManager actionManager = new ActionsManager();
			for (int i = 0; i < files.Length; i++ )
			{
				actionManager.ReadFromFile(files[i]);
				actionManager.Execute();
			}

			return 0;
		}

		public override String ToString()
		{
			return GetName() + Globals.Seperator + Dir;
		}

		public Action NewInstance()
		{
			return new RunDirectoryAction();
		}
	}

}
