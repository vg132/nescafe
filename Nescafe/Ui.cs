﻿using Nescafe.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

//0,9375
//
namespace Nescafe
{
	class Ui : Form
	{
		Bitmap _frame;
		Console _console;

		Thread _nesThread;

		Graphics g;

		FormWindowState _state;

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (_state != WindowState)
			{
				g = CreateGraphics();
				g.InterpolationMode = InterpolationMode.NearestNeighbor;
				g.Clear(Color.Black);
			}
			_state = WindowState;
		}

		protected override void OnResizeEnd(EventArgs e)
		{
			base.OnResizeEnd(e);
			g = CreateGraphics();
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.Clear(Color.Black);
		}

		public Ui()
		{
			Text = "NEScafé";
			Size = new Size(512, 480);
			FormBorderStyle = FormBorderStyle.Sizable;

			g = CreateGraphics();
			g.InterpolationMode = InterpolationMode.NearestNeighbor;

			CenterToScreen();
			InitMenus();

			this._console = new Console();
			_console.DrawAction = Draw;

			_frame = new Bitmap(256, 240, PixelFormat.Format8bppIndexed);
			InitPalette();

			KeyDown += new KeyEventHandler(OnKeyDown);
			KeyUp += new KeyEventHandler(OnKeyUp);

			_nesThread = new Thread(new ThreadStart(StartNes));
			_nesThread.IsBackground = true;
		}

		void StopConsole()
		{
			_console.Stop = true;

			if (_nesThread.ThreadState == ThreadState.Running)
			{
				_nesThread.Join();
			}
		}

		void StartConsole()
		{
			_nesThread = new Thread(new ThreadStart(StartNes));
			_nesThread.IsBackground = true;
			_nesThread.Start();
		}

		void LoadCartridge(object sender, EventArgs e)
		{
			StopConsole();

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

		void LaunchGitHubLink(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.github.com/vg132/nescafe");
		}

		void InitMenus()
		{
			var ms = new MenuStrip();

			// File menu
			var fileMenu = new ToolStripMenuItem("File");
			var fileLoadMenu = new ToolStripMenuItem("Load ROM", null, new EventHandler(LoadCartridge));
			fileMenu.DropDownItems.Add(fileLoadMenu);
			var screenshotMenu = new ToolStripMenuItem("Take Screenshot", null, new EventHandler(TakeScreenshot));
			fileMenu.DropDownItems.Add(screenshotMenu);
			var exitMenuItem = new ToolStripMenuItem("Exit", null, new EventHandler(ExitApp));
			fileMenu.DropDownItems.Add(exitMenuItem);
			ms.Items.Add(fileMenu);

			// Save menu
			var saveMenu = new ToolStripMenuItem("Save");
			var saveStateMenu = new ToolStripMenuItem("Save state", null, new EventHandler(SaveState));
			saveMenu.DropDownItems.Add(saveStateMenu);
			var loadStateMenu = new ToolStripMenuItem("Load state", null, new EventHandler(LoadState));
			saveMenu.DropDownItems.Add(loadStateMenu);
			var saveBattery = new ToolStripMenuItem("Save battery", null, new EventHandler(SaveBattery));
			saveMenu.DropDownItems.Add(saveBattery);
			ms.Items.Add(saveMenu);

			// Help menu
			var helpMenu = new ToolStripMenuItem("Help");
			var helpGithubMenu = new ToolStripMenuItem("GitHub", null, new EventHandler(LaunchGitHubLink));
			helpMenu.DropDownItems.Add(helpGithubMenu);
			ms.Items.Add(helpMenu);
			Controls.Add(ms);
		}

		private void SaveBattery(object sender, EventArgs e)
		{
			StateService.SaveBatteryMemory(_console);
		}

		private void SaveState(object sender, EventArgs e)
		{
			StateService.SaveState(_console, 0);
		}

		private void LoadState(object sender, EventArgs e)
		{
			StateService.LoadState(_console, 0);
		}

		void TakeScreenshot(object sender, EventArgs e)
		{
			var filename = "screenshot_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
			_frame.Save(filename);
		}

		void ExitApp(object sender, EventArgs e)
		{
			Application.Exit();
		}

		void StartNes()
		{
			_console.Start();
		}

		void InitPalette()
		{
			var palette = _frame.Palette;
			palette.Entries[0x0] = Color.FromArgb(84, 84, 84);
			palette.Entries[0x1] = Color.FromArgb(0, 30, 116);
			palette.Entries[0x2] = Color.FromArgb(8, 16, 144);
			palette.Entries[0x3] = Color.FromArgb(48, 0, 136);
			palette.Entries[0x4] = Color.FromArgb(68, 0, 100);
			palette.Entries[0x5] = Color.FromArgb(92, 0, 48);
			palette.Entries[0x6] = Color.FromArgb(84, 4, 0);
			palette.Entries[0x7] = Color.FromArgb(60, 24, 0);
			palette.Entries[0x8] = Color.FromArgb(32, 42, 0);
			palette.Entries[0x9] = Color.FromArgb(8, 58, 0);
			palette.Entries[0xa] = Color.FromArgb(0, 64, 0);
			palette.Entries[0xb] = Color.FromArgb(0, 60, 0);
			palette.Entries[0xc] = Color.FromArgb(0, 50, 60);
			palette.Entries[0xd] = Color.FromArgb(0, 0, 0);
			palette.Entries[0xe] = Color.FromArgb(0, 0, 0);
			palette.Entries[0xf] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x10] = Color.FromArgb(152, 150, 152);
			palette.Entries[0x11] = Color.FromArgb(8, 76, 196);
			palette.Entries[0x12] = Color.FromArgb(48, 50, 236);
			palette.Entries[0x13] = Color.FromArgb(92, 30, 228);
			palette.Entries[0x14] = Color.FromArgb(136, 20, 176);
			palette.Entries[0x15] = Color.FromArgb(160, 20, 100);
			palette.Entries[0x16] = Color.FromArgb(152, 34, 32);
			palette.Entries[0x17] = Color.FromArgb(120, 60, 0);
			palette.Entries[0x18] = Color.FromArgb(84, 90, 0);
			palette.Entries[0x19] = Color.FromArgb(40, 114, 0);
			palette.Entries[0x1a] = Color.FromArgb(8, 124, 0);
			palette.Entries[0x1b] = Color.FromArgb(0, 118, 40);
			palette.Entries[0x1c] = Color.FromArgb(0, 102, 120);
			palette.Entries[0x1d] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x1e] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x1f] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x20] = Color.FromArgb(236, 238, 236);
			palette.Entries[0x21] = Color.FromArgb(76, 154, 236);
			palette.Entries[0x22] = Color.FromArgb(120, 124, 236);
			palette.Entries[0x23] = Color.FromArgb(176, 98, 236);
			palette.Entries[0x24] = Color.FromArgb(228, 84, 236);
			palette.Entries[0x25] = Color.FromArgb(236, 88, 180);
			palette.Entries[0x26] = Color.FromArgb(236, 106, 100);
			palette.Entries[0x27] = Color.FromArgb(212, 136, 32);
			palette.Entries[0x28] = Color.FromArgb(160, 170, 0);
			palette.Entries[0x29] = Color.FromArgb(116, 196, 0);
			palette.Entries[0x2a] = Color.FromArgb(76, 208, 32);
			palette.Entries[0x2b] = Color.FromArgb(56, 204, 108);
			palette.Entries[0x2c] = Color.FromArgb(56, 180, 204);
			palette.Entries[0x2d] = Color.FromArgb(60, 60, 60);
			palette.Entries[0x2e] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x2f] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x30] = Color.FromArgb(236, 238, 236);
			palette.Entries[0x31] = Color.FromArgb(168, 204, 236);
			palette.Entries[0x32] = Color.FromArgb(188, 188, 236);
			palette.Entries[0x33] = Color.FromArgb(212, 178, 236);
			palette.Entries[0x34] = Color.FromArgb(236, 174, 236);
			palette.Entries[0x35] = Color.FromArgb(236, 174, 212);
			palette.Entries[0x36] = Color.FromArgb(236, 180, 176);
			palette.Entries[0x37] = Color.FromArgb(228, 196, 144);
			palette.Entries[0x38] = Color.FromArgb(204, 210, 120);
			palette.Entries[0x39] = Color.FromArgb(180, 222, 120);
			palette.Entries[0x3a] = Color.FromArgb(168, 226, 144);
			palette.Entries[0x3b] = Color.FromArgb(152, 226, 180);
			palette.Entries[0x3c] = Color.FromArgb(160, 214, 228);
			palette.Entries[0x3d] = Color.FromArgb(160, 162, 160);
			palette.Entries[0x3e] = Color.FromArgb(0, 0, 0);
			palette.Entries[0x3f] = Color.FromArgb(0, 0, 0);

			_frame.Palette = palette;
		}

		unsafe void Draw(byte[] screen)
		{
			var _frameData = _frame.LockBits(new Rectangle(0, 0, 256, 240), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

			var ptr = (byte*)_frameData.Scan0;
			for (var i = 0; i < 256 * 240; i++)
			{
				ptr[i] = screen[i];
			}
			_frame.UnlockBits(_frameData);

			var width = (int)Math.Round(Size.Height * 1.0666666666666666666666666666667, 0);
			var height = Size.Height;
			var x = 0;
			if (width < Size.Width)
			{
				x = (Size.Width - width) / 2;
			}
			g.DrawImage(_frame, x, 0, width, height);
		}

		void OnKeyDown(object sender, KeyEventArgs e)
		{
			SetControllerButton(true, e);
		}

		void OnKeyUp(object sender, KeyEventArgs e)
		{
			SetControllerButton(false, e);
		}

		void SetControllerButton(bool state, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Z:
					_console.Controller.setButtonState(Controller.Button.A, state);
					break;
				case Keys.X:
					_console.Controller.setButtonState(Controller.Button.B, state);
					break;
				case Keys.Left:
					_console.Controller.setButtonState(Controller.Button.Left, state);
					break;
				case Keys.Right:
					_console.Controller.setButtonState(Controller.Button.Right, state);
					break;
				case Keys.Up:
					_console.Controller.setButtonState(Controller.Button.Up, state);
					break;
				case Keys.Down:
					_console.Controller.setButtonState(Controller.Button.Down, state);
					break;
				case Keys.Q:
				case Keys.Enter:
					_console.Controller.setButtonState(Controller.Button.Start, state);
					break;
				case Keys.W:
				case Keys.Escape:
					_console.Controller.setButtonState(Controller.Button.Select, state);
					break;
			}
		}
	}
}
