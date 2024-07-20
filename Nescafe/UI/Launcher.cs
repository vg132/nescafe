namespace Nescafe.UI;

public partial class Launcher : Form
{
	private readonly Core.Console _console;

	private Renderer _renderer;
	private Thread _nesThread;

	public Launcher()
	{
		InitializeComponent();

		//_screen = new Screen();
		//new Task(() =>
		//{
		//	_screen.Start();
		//}).Start();

		//_console = new Core.Console();
		//_console.DrawAction = _screen.UpdateScreen;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_renderer = new Renderer(glControl1);
	}

	private void StopConsole()
	{
		_console.Stop = true;
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
			if (_console.LoadCartridge(openFileDialog.FileName))
			{
				Text = "NEScafé - " + openFileDialog.SafeFileName;
				StartConsole();
			}
			else
			{
				MessageBox.Show("Could not load ROM, see standard output for details");
			}
		}
	}

	private void toolStripMenuItem5_Click(object sender, EventArgs e)
	{

	}

	private void videoSizeToolStripMenuItem_Click(object sender, EventArgs e)
	{

	}

	private void toolStripMenuItem9_Click(object sender, EventArgs e)
	{
		
	}
}
