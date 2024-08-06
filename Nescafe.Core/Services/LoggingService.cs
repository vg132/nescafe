using Nescafe.Core;
using System.Collections.Concurrent;

namespace Nescafe.Services;

public enum NESEvents
{
	Other,
	Cartridge,
	Frame,
	Cpu,
	Ppu,
	PpuMemory,
	CpuMemory,
	Mapper
}

public static class LoggingService
{
	public static void LogEvent(NESEvents typeOfEvent, string message)
	{
		switch (typeOfEvent)
		{
			case NESEvents.Other:
				if (AppSettings.Instance.LoggingOther)
				{
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.Cartridge:
				if (AppSettings.Instance.LoggingCartridge)
				{
					System.Diagnostics.Debug.WriteLine(message);
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.Frame:
				if (AppSettings.Instance.LoggingFrame)
				{
					System.Diagnostics.Debug.WriteLine(message);
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.Cpu:
				if (AppSettings.Instance.LoggingCpu)
				{
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.Ppu:
				if (AppSettings.Instance.LoggingPpu)
				{
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.CpuMemory:
				if (AppSettings.Instance.LoggingCpuMemory)
				{
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.PpuMemory:
				if (AppSettings.Instance.LoggingPpuMemory)
				{
					_logEvents.Enqueue(message);
				}
				break;
			case NESEvents.Mapper:
				if (AppSettings.Instance.LoggingMapper)
				{
					_logEvents.Enqueue(message);
				}
				break;
		}
	}

	private static ConcurrentQueue<string> _logEvents = new ConcurrentQueue<string>();

	static LoggingService()
	{
		var logThread = new Thread(new ThreadStart(WriteToDisk));
		logThread.IsBackground = true;
		logThread.Start();
	}

	private static void WriteToDisk()
	{
		var logFile = Path.Combine(AppSettings.Instance.LoggingOutputFolder, $"nes_log_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.txt");
		using var streamWriter = new StreamWriter(new FileStream(logFile, FileMode.OpenOrCreate));
		while(true)
		{
			Thread.Sleep(100);
			string line;
			while (_logEvents.TryDequeue(out line))
			{
				streamWriter.WriteLine(line);
			}
			streamWriter.Flush();
		}
	}
}
