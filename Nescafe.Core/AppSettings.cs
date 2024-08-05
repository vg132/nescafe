using System.Reflection;
using System.Text.Json;

namespace Nescafe.Core;

public class AppSettings
{
	private class Settings
	{
		public Settings()
		{

		}

		public int VideoSize { get; set; } = 3;
		public int CpuSpeed { get; set; } = 60;

		#region Logging

		public string LoggingOutputFolder { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public bool LoggingFrame { get; set; } = true;
		public bool LoggingCpu{ get; set; } = true;
		public bool LoggingPpu { get; set; } = true;
		public bool LoggingCpuMemory { get; set; } = true;
		public bool LoggingPpuMemory { get; set; } = true;
		public bool LoggingMapper { get; set; } = true;
		public bool LoggingOther { get; set; } = true;
		public bool LoggingCartridge { get; set; } = true;

		#endregion

		public List<string> MruList { get; set; } = new List<string>();
	}

	private static AppSettings _instance;
	private Settings _settings;

	private AppSettings()
	{
		if (File.Exists(SettingsFilePath))
		{
			_settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFilePath));
		}
		else
		{
			_settings = new Settings();
		}
	}

	public void Save() => File.WriteAllText(SettingsFilePath, JsonSerializer.Serialize<Settings>(_settings));

	public static AppSettings Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AppSettings();
			}
			return _instance;
		}
	}

	private static string SettingsFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "appSettings.json");

	#region Settings

	public int VideoSize
	{
		get => _settings.VideoSize;
		set
		{
			_settings.VideoSize = value;
			Save();
		}
	}

	public int CpuSpeed
	{
		get => _settings.CpuSpeed;
		set
		{
			_settings.CpuSpeed = value;
			Save();
		}
	}

	public void MruListAdd(string path)
	{
		_settings.MruList.Remove(path);
		_settings.MruList.Insert(0, path);
		_settings.MruList = _settings.MruList.Take(10).ToList();
		Save();
	}

	public void MruListRemove(string path)
	{
		_settings.MruList.Remove(path);
		_settings.MruList = _settings.MruList.Take(10).ToList();
		Save();
	}

	public List<string> MruList => _settings.MruList;

	#region Logging

	public bool LoggingPpu
	{
		get => _settings.LoggingPpu;
		set
		{
			_settings.LoggingPpu = value;
			Save();
		}
	}

	public bool LoggingPpuMemory
	{
		get => _settings.LoggingPpuMemory;
		set
		{
			_settings.LoggingPpuMemory = value;
			Save();
		}
	}


	public bool LoggingCpuMemory
	{
		get => _settings.LoggingCpuMemory;
		set
		{
			_settings.LoggingCpuMemory = value;
			Save();
		}
	}

	public bool LoggingCartridge
	{
		get => _settings.LoggingCartridge;
		set
		{
			_settings.LoggingCartridge = value;
			Save();
		}
	}

	public bool LoggingOther
	{
		get => _settings.LoggingOther;
		set
		{
			_settings.LoggingOther = value;
			Save();
		}
	}

	public bool LoggingMapper
	{
		get => _settings.LoggingMapper;
		set
		{
			_settings.LoggingMapper = value;
			Save();
		}
	}

	public bool LoggingFrame
	{
		get => _settings.LoggingFrame;
		set
		{
			_settings.LoggingFrame = value;
			Save();
		}
	}

	public bool LoggingCpu
	{
		get => _settings.LoggingCpu;
		set
		{
			_settings.LoggingCpu = value;
			Save();
		}
	}

	public string LoggingOutputFolder
	{
		get => _settings.LoggingOutputFolder;
		set
		{
			_settings.LoggingOutputFolder = value;
			Save();
		}
	}

	#endregion

	#endregion
}
