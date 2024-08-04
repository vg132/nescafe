using Nescafe.Core;

namespace Nescafe.UI.Debug;
public partial class PaletteViewer : Form
{
	private readonly Core.Console _console;

	private const int PaletteSize = 32;
	private byte[] _paletteData = new byte[PaletteSize];

	public PaletteViewer(Core.Console console)
	{
		InitializeComponent();
		_console = console;

		UpdatePaletteFromPpuVram();
		ShowInfo(0);
	}

	private void PnlPalettePreview_OnPaint(object sender, PaintEventArgs e)
	{
		var g = e.Graphics;
		for (var y = 0; y < 8; y++)
		{
			for (var x = 0; x < 4; x++)
			{
				var color = Palette.GetColor(_paletteData[y * 4 + x] & 0x3F); // Mask to ensure valid color index
				using (var brush = new SolidBrush(color))
				{
					g.FillRectangle(brush, x * 32, y * 32, 32, 32);
				}
			}
		}
	}

	private void ShowInfo(int index)
	{
		lblIndex.Text = $"${index.ToString("X2")}";
		lblValue.Text = $"${_paletteData[index].ToString("X2")}";
		var color = Palette.GetColor(_paletteData[index]);
		lblColorHEX.Text = $"#{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
		lblColorRGB.Text = $"{color.R}, {color.G}, {color.B}";

		var g = pnlColorPreview.CreateGraphics();
		using (var brush = new SolidBrush(color))
		{
			g.FillRectangle(brush, 0, 0, 32, 32);
		}
	}

	public void UpdatePaletteFromPpuVram()
	{
		for (int i = 0; i < PaletteSize; i++)
		{
			_paletteData[i] = _console.PpuMemory.Read((ushort)(0x3F00 + i));
		}
		pnlPalettePreview.Invalidate();
	}

	private void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		UpdatePaletteFromPpuVram();
	}

	private void PnlPalettePreview_Click(object sender, EventArgs e)
	{
		var mouseEvent = e as MouseEventArgs;
		var indexX = mouseEvent.X / 32;
		var indexY = mouseEvent.Y / 32;
		var index = (indexY * 4) + indexX;
		ShowInfo(index);
	}

	private void PnlPalettePreview_MouseMove(object sender, EventArgs e)
	{
		var mouseEvent = e as MouseEventArgs;
		var indexX = mouseEvent.X / 32;
		var indexY = mouseEvent.Y / 32;
	}
}
