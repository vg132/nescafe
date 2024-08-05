using Nescafe.Core;

namespace Nescafe.UI.Debug;
public partial class Logging : Form
{
	public Logging()
	{
		InitializeComponent();

		chkCpu.Checked = AppSettings.Instance.LoggingCpu;
		chkFrame.Checked = AppSettings.Instance.LoggingFrame;
		chkPpu.Checked = AppSettings.Instance.LoggingPpu;
		chkCpuMemory.Checked = AppSettings.Instance.LoggingCpuMemory;
		chkPpuMemory.Checked = AppSettings.Instance.LoggingPpuMemory;
		chkCartridge.Checked = AppSettings.Instance.LoggingCartridge;
		chkOther.Checked = AppSettings.Instance.LoggingOther;
		chkMapper.Checked = AppSettings.Instance.LoggingMapper;

		txtOutputFolder.Text = AppSettings.Instance.LoggingOutputFolder;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		AppSettings.Instance.LoggingPpu = chkPpu.Checked;
		AppSettings.Instance.LoggingFrame = chkFrame.Checked;
		AppSettings.Instance.LoggingOutputFolder = txtOutputFolder.Text;
		AppSettings.Instance.LoggingCpu = chkCpu.Checked;
		AppSettings.Instance.LoggingCpuMemory = chkCpuMemory.Checked;
		AppSettings.Instance.LoggingPpuMemory = chkPpuMemory.Checked;
		AppSettings.Instance.LoggingMapper = chkMapper.Checked;
		AppSettings.Instance.LoggingOther = chkOther.Checked;
		AppSettings.Instance.LoggingCartridge = chkCartridge.Checked;

		Directory.CreateDirectory(txtOutputFolder.Text);

		Close();
	}

	private void btnSelectOutputFolder_Click(object sender, EventArgs e)
	{
		using var folderBrowserDialog = new FolderBrowserDialog();
		Directory.CreateDirectory(txtOutputFolder.Text);
		folderBrowserDialog.InitialDirectory = txtOutputFolder.Text;
		var result = folderBrowserDialog.ShowDialog();
		if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
		{
			txtOutputFolder.Text = folderBrowserDialog.SelectedPath;
		}
	}
}
