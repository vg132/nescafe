//namespace Nescafe;

//internal class Program
//{
//	private static Thread _nesThread;
//	private static Core.Console _console;

//	static void Main(string[] args)
//	{
//		Console.WriteLine("Hello, World!");
//		_console = new Core.Console();
//		_console.DrawAction = Draw;
//		_nesThread = new Thread(new ThreadStart(StartNes));
//		_nesThread.IsBackground = true;

//		_console.LoadCartridge(@"D:\Projects\nescafe\test_roms\blargg_nes_cpu_test5\official.nes");
//		StartConsole();
//		while (true) { }
//	}

//	unsafe static void Draw(byte[] screen)
//	{

//	}

//	private static void StopConsole()
//	{
//		_console.Stop = true;

//		if (_nesThread.ThreadState == ThreadState.Running)
//		{
//			_nesThread.Join();
//		}
//	}

//	private static void StartConsole()
//	{
//		_nesThread = new Thread(new ThreadStart(StartNes));
//		_nesThread.IsBackground = true;
//		_nesThread.Start();
//	}

//	private static void StartNes()
//	{
//		_console.Start();
//	}

//}
