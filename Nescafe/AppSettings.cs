using System.Text.Json;

namespace Nescafe;

public class AppSettings
{
	private class Settings
	{
		public Settings()
		{

		}

		public int VideoSize { get; set; } = 3;
		public int CpuSpeed { get; set; } = 60;

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

	private static string SettingsFilePath => Path.Combine(Application.CommonAppDataPath, "appSettings.json");

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

	#endregion
}
