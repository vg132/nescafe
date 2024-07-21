using Nescafe.Services;
using Nescafe.UI.Input;
using System.Diagnostics;

namespace Nescafe.UI;

public partial class Launcher : Form
{
	private readonly Core.Console _console;

	private Renderer _renderer;
	private Thread _nesThread;
	private Keyboard _input;

	public Launcher()
	{
		InitializeComponent();

		glControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
		glControl1.Dock = DockStyle.Fill;

		// We don't want this to be visible but need it for key events to work. Bug in GLControl...
		textBoxFixForKeyEvents.Top = -100;

		SetFormSize(AppSettings.Instance.VideoSize);
		CheckCorrectVideoSizeMenuItem();
		CheckCorrectCpuSpeedMenuItem();
		SetupMruList();
		SetupStateMenuItems();

		_console = new Core.Console();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_renderer = new Renderer(glControl1);
		glControl1.DisableNativeInput();
		SetupConsole();
	}

	//protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	//{
	//	var ret = base.ProcessCmdKey(ref msg, keyData);
	//	Debug.WriteLine($"Key! {msg.WParam} {msg.LParam}, {keyData}");
	//	return ret;
	//}

	private void SetupConsole()
	{
		_console.DrawAction = _renderer.UpdateScreen;
		_console.OnRunning += Console_OnRunning;

		_input = new Keyboard(this, _console);
	}

	private void Console_OnRunning(Core.Console obj)
	{
		SetupStateMenuItems();
	}

	private void StopConsole()
	{
		_console.Stop();
	}

	private void StartConsole()
	{
		_nesThread?.Interrupt();
		_nesThread = new Thread(new ThreadStart(StartNes));
		_nesThread.IsBackground = true;
		_nesThread.Start();
	}

	private void StartNes()
	{
		_console.Start();
	}

	private void openToolStripMenuItem_Click(object sender, EventArgs e)
	{
		var openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "NES ROMs | *.nes";
		openFileDialog.RestoreDirectory = true;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			LoadROM(openFileDialog.FileName);
		}
	}

	private void LoadROM(string path)
	{
		StopConsole();
		if (File.Exists(path))
		{
			if (_console.LoadCartridge(path))
			{
				AppSettings.Instance.MruListAdd(path);
				Text = "NEScafé - " + Path.GetFileNameWithoutExtension(path);
				StartConsole();
			}
			else
			{
				MessageBox.Show("Could not load ROM");
			}
		}
		else
		{
			AppSettings.Instance.MruListRemove(path);
		}
		SetupMruList();
	}

	#region Menu items

	private void videoSizeMenuItem_Click(object sender, EventArgs e)
	{
		var size = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
		SetFormSize(size);
		AppSettings.Instance.VideoSize = size;
		CheckCorrectVideoSizeMenuItem();
	}

	private void SetFormSize(int size) => ClientSize = new Size(256 * size, (240 * size) + menuStrip1.ClientSize.Height);

	private void exitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Application.Exit();
	}

	private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_console.Pause = !_console.Pause;
		pauseToolStripMenuItem.Checked = _console.Pause;
	}

	private void resetToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_console.Pause = false;
		pauseToolStripMenuItem.Checked = _console.Pause;
		_console.Reset();
	}

	private void cpuSpeedMenuItem_Click(object sender, EventArgs e)
	{
		var cpuSpeed = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
		AppSettings.Instance.CpuSpeed = cpuSpeed;
		CheckCorrectCpuSpeedMenuItem();
	}

	private void mruListMenuItem_Click(object sender, EventArgs e)
	{
		var path = ((ToolStripMenuItem)sender).Tag.ToString();
		LoadROM(path);
	}

	private void saveStateMenuItem_Click(object sender, EventArgs e)
	{
		var slot = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
		StateService.SaveState(_console, slot);
		SetupStateMenuItems();
	}

	private void loadStateMenuItem_Click(object sender, EventArgs e)
	{
		var slot = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());
		StateService.LoadState(_console, slot);
	}

	#endregion

	private void SetupStateMenuItems()
	{
		if (_console?.IsRunning == true)
		{
			saveStateMenuItem.Enabled = true;
			loadStateMenuItem.Enabled = true;
			loadState1MenuItem.Enabled = StateService.HasState(_console, 1);
			loadState2MenuItem.Enabled = StateService.HasState(_console, 2);
			loadState3MenuItem.Enabled = StateService.HasState(_console, 3);
			loadState4MenuItem.Enabled = StateService.HasState(_console, 4);
			loadState5MenuItem.Enabled = StateService.HasState(_console, 5);
		}
		else
		{
			loadStateMenuItem.Enabled = false;
			saveStateMenuItem.Enabled = false;
		}
	}

	private void CheckCorrectVideoSizeMenuItem()
	{
		videoSize1MenuItem.Checked = AppSettings.Instance.VideoSize == 1;
		videoSize2MenuItem.Checked = AppSettings.Instance.VideoSize == 2;
		videoSize3MenuItem.Checked = AppSettings.Instance.VideoSize == 3;
		videoSize4MenuItem.Checked = AppSettings.Instance.VideoSize == 4;
		videoSize5MenuItem.Checked = AppSettings.Instance.VideoSize == 5;
		videoSize6MenuItem.Checked = AppSettings.Instance.VideoSize == 6;
	}

	private void CheckCorrectCpuSpeedMenuItem()
	{
		cpuSpeed50MenuItem.Checked = AppSettings.Instance.CpuSpeed == 30;
		cpuSpeed75MenuItem.Checked = AppSettings.Instance.CpuSpeed == 45;
		cpuSpeed100MenuItem.Checked = AppSettings.Instance.CpuSpeed == 60;
		cpuSpeed125MenuItem.Checked = AppSettings.Instance.CpuSpeed == 75;
		cpuSpeed150MenuItem.Checked = AppSettings.Instance.CpuSpeed == 90;
		cpuSpeed175MenuItem.Checked = AppSettings.Instance.CpuSpeed == 105;
		cpuSpeed200MenuItem.Checked = AppSettings.Instance.CpuSpeed == 120;
	}

	private void SetupMruList()
	{
		if (!AppSettings.Instance.MruList.Any())
		{
			mruListMenuItem.Enabled = false;
		}
		else
		{
			mruListMenuItem.Enabled = true;
			mruListMenuItem.DropDownItems.Clear();
			var counter = 0;
			foreach (var item in AppSettings.Instance.MruList)
			{
				var menuItem = new ToolStripMenuItem
				{
					Tag = item,
					Text = $"{++counter}. {Path.GetFileNameWithoutExtension(item)}",
					ToolTipText = item,
				};
				menuItem.Click += mruListMenuItem_Click;
				mruListMenuItem.DropDownItems.Add(menuItem);
			}
		}
	}
}