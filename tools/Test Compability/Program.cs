using Nescafe.Core.Mappers;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nescafe.Test_Compability;

public class Program
{
	private Thread _nesThread;
	private Thread _killThread;
	private Core.Console _console;
	private bool _isRunning = false;
	private string _currentFile;
	private static string _currentRom;
	private static ConcurrentStack<string> _workingFiles = new ConcurrentStack<string>();
	private static ConcurrentStack<string> _notWorkingFiles = new ConcurrentStack<string>();
	private static ConcurrentStack<Core.Console> _consolePool = new ConcurrentStack<Core.Console>();
	private static Stopwatch _stopwatch = new Stopwatch();

	private void TestRom(string path)
	{
		try
		{
			_currentFile = path;
			if (!_consolePool.TryPop(out _console))
			{
				_console = new Core.Console();
			}
			_isRunning = true;
			if (LoadROM(path))
			{
				while (_isRunning)
				{
					Thread.Sleep(500);
				}
			}
		}
		finally
		{
			_nesThread?.Interrupt();
			_killThread?.Interrupt();
			_consolePool.Push(_console);
		}
	}

	private bool LoadROM(string path)
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
					_notWorkingFiles.Push($"{_currentFile}\t{_console?.Cartridge?.MapperNumber}\tNot loading");
				}
			}
		}
		catch(MapperNotSupportedException ex)
		{
			_notWorkingFiles.Push($"{_currentFile}\t{ex.Mapper}\t{ex.Message}");
		}
		catch(Exception ex)
		{
			_notWorkingFiles.Push($"{_currentFile}\t{_console?.Cartridge?.MapperNumber}\t{ex.Message}");
		}
		return false;
	}

	private void StartConsole()
	{
		_nesThread = new Thread(new ThreadStart(StartNes));
		_nesThread.IsBackground = true;
		_nesThread.Start();

		_killThread = new Thread(new ThreadStart(StopConsole));
		_killThread.IsBackground = true;
		_killThread.Start();
	}

	private void StopConsole()
	{
		try
		{
			Thread.Sleep(750);
			_console.Stop();
			_isRunning = false;
			_workingFiles.Push(_currentFile);
		}
		catch { }
	}

	private void StartNes()
	{
		try
		{
			_console.Start();
		}
		catch(ThreadInterruptedException) { }
		catch (Exception ex)
		{
			_notWorkingFiles.Push($"{_currentFile}\t{_console?.Cartridge?.MapperNumber}\t{ex.Message}");
		}
		_isRunning = false;
	}

	static void ConsoleUpdate()
	{
		_stopwatch.Start();

		Console.CursorVisible = false;
		Console.SetCursorPosition(0, 2);
		Console.WriteLine("NES Tester Running.");
		Console.WriteLine("Available consoles: ");
		Console.WriteLine("Tested games: ");
		Console.WriteLine("Working: ");
		Console.WriteLine("Not working: ");
		Console.WriteLine("Currently testing: ");
		Console.WriteLine("Time: ");
		try
		{
			while (true)
			{
				Console.SetCursorPosition("Available consoles: ".Length, 3);
				Console.WriteLine($"{_consolePool.Count()}   ");
				Console.SetCursorPosition("Tested games: ".Length, 4);
				Console.WriteLine($"{_workingFiles.Count() + _notWorkingFiles.Count()}   ");
				Console.SetCursorPosition("Working: ".Length, 5);
				Console.WriteLine($"{_workingFiles.Count()}   ");
				Console.SetCursorPosition("Not working: ".Length, 6);
				Console.WriteLine($"{_notWorkingFiles.Count()}   ");
				Console.SetCursorPosition("Currently testing: ".Length, 7);
				Console.WriteLine($"{_currentRom}                                        ");
				Console.SetCursorPosition("Time: ".Length, 8);
				Console.WriteLine(_stopwatch.Elapsed.ToString("mm\\:ss"));
				Thread.Sleep(1000);
			}
		}
		catch { }
		_stopwatch.Stop();
	}

	static void Main(string[] args)
	{
		var files = Directory.GetFiles(@"E:\nescafe\Roms", "*.nes");
		var consoleUpdateThread = new Thread(new ThreadStart(ConsoleUpdate));
		consoleUpdateThread.IsBackground = true;
		consoleUpdateThread.Start();
		Parallel.ForEach(files, file =>
		{
			_currentRom = file;
			var p = new Program();
			p.TestRom(file);
		});
		consoleUpdateThread.Interrupt();

		var list = _workingFiles.ToList().Distinct().OrderBy(item => item).ToList();
		list.Insert(0, $"Total time: {_stopwatch.Elapsed.ToString("mm\\:ss")}, tested: {_workingFiles.Count() + _notWorkingFiles.Count()}, working: {_workingFiles.Distinct().Count()}");
		File.WriteAllLines(@$"E:\nescafe\tools\Test Compability\logs\working_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt", list);
		list = _notWorkingFiles.ToList().Distinct().OrderBy(item => item).ToList();
		list.Insert(0, $"Total time: {_stopwatch.Elapsed.ToString("mm\\:ss")}, tested: {_workingFiles.Count() + _notWorkingFiles.Count()}, not working: {_notWorkingFiles.Distinct().Count()}");
		File.WriteAllLines(@$"E:\nescafe\tools\Test Compability\logs\not_working_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt", list);
		Console.CursorVisible = true;
	}
}
