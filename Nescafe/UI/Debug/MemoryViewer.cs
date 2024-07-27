using RPHexEditor;

namespace Nescafe.UI.Debug;
public partial class MemoryViewer : Form
{
	private readonly Core.Console _console;

	public MemoryViewer(Core.Console console)
	{
		InitializeComponent();
		_console = console;
		rpHexEditoruc1.ByteDataSource = new MemoryByteData(_console.CpuMemory.State.InternalRam);
	}
}
