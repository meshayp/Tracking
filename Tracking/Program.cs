using Actions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tracking
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				TrackingEngine engine = new TrackingEngine();
				engine.onLoad();
				engine.UserEvents.ReadFromFile(args[0]);
				engine.eStatus.play();
                engine.UserEvents.Execute();
				engine.eStatus.stop();
				engine.onClose();
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
			}
		}
	}
}