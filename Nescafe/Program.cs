using Nescafe.UI;

namespace Nescafe;

public class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Application.Run(new Launcher(args));


		//var files = Directory.GetFiles(@"D:\Projects\nescafe\Roms", "*.nes");
		//foreach (var file in files)
		//{
		//	try
		//	{
		//		args = new string[] { file };
		//		var timer = new System.Timers.Timer();
		//		var l = new Launcher(args);
		//		timer.AutoReset = false;
		//		timer.Interval = 1000;
		//		timer.Elapsed += (sender, e) =>
		//		{
		//			try
		//			{
		//				l.Exit();
		//			}
		//			catch
		//			{
		//			}
		//		};
		//		timer.Start();
		//		Application.Run(l);
		//		File.Move(file, Path.Combine("D:\\Projects\\nescafe\\Roms\\working", Path.GetFileName(file)));
		//	}
		//	catch(Exception ex)
		//	{
		//		Console.WriteLine("Error:" + ex.ToString());
		//		File.Move(file, Path.Combine("D:\\Projects\\nescafe\\Roms\\Not working", Path.GetFileName(file)));
		//	}
		//}
		//Console.WriteLine("Exit");
	}
}
