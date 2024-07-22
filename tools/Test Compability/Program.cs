namespace Nescafe.Test_Compability;

public class Program
{
	private static Thread _nesThread;
	private static Thread _killThread;
	private static Core.Console _console;
	private static bool _isRunning = false;
	private static string _currentFile;
	private static List<string> _workingFiles = new List<string>();
	private static List<string> _notWorkingFiles = new List<string>();

	private static void TestRom(string path)
	{
		_currentFile = path;
		_console = new Core.Console();
		_isRunning = true;
		if (LoadROM(path))
		{
			while (_isRunning)
			{
				Thread.Sleep(500);
			}
		}
	}

	private static bool LoadROM(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				if (_console.LoadCartridge(path))
				{
					StartConsole();
					return true;
				}
				else
				{
					_notWorkingFiles.Add(_currentFile);
					Console.WriteLine($"Not loading: {path}");
				}
			}
		}
		catch
		{
			_notWorkingFiles.Add(_currentFile);
			Console.WriteLine($"Not working");
		}
		return false;
	}

	private static void StartConsole()
	{
		_nesThread = new Thread(new ThreadStart(StartNes));
		_nesThread.IsBackground = true;
		_nesThread.Start();

		_killThread = new Thread(new ThreadStart(StopConsole));
		_killThread.IsBackground = true;
		_killThread.Start();
	}

	private static void StopConsole()
	{
		Thread.Sleep(1500);
		_console.Stop();
		_isRunning = false;
		Console.WriteLine("Seems to be working");
		_workingFiles.Add(_currentFile);
	}

	private static void StartNes()
	{
		try
		{
			_console.Start();
		}
		catch
		{
			_notWorkingFiles.Add(_currentFile);
			Console.WriteLine($"Not working");
		}
		_isRunning = false;
	}

	static void Main(string[] args)
	{
		var files = Directory.GetFiles(@"D:\Projects\nescafe\Roms", "*.nes");
		foreach (var file in files)
		{
			TestRom(file);
		}
		_workingFiles.Insert(0, $"Working: {_workingFiles.Distinct().Count()}");
		File.WriteAllLines(@$"D:\Projects\nescafe\logs\working_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt", _workingFiles.Distinct());
		_notWorkingFiles.Insert(0, $"Working: {_notWorkingFiles.Distinct().Count()}");
		File.WriteAllLines(@$"D:\Projects\nescafe\logs\not_working_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt", _notWorkingFiles.Distinct());
	}
}
