namespace Nescafe.UI;

partial class Launcher
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		menuStrip1 = new MenuStrip();
		fileToolStripMenuItem = new ToolStripMenuItem();
		openToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem1 = new ToolStripSeparator();
		saveStateMenuItem = new ToolStripMenuItem();
		saveState1MenuItem = new ToolStripMenuItem();
		saveState2MenuItem = new ToolStripMenuItem();
		saveState3MenuItem = new ToolStripMenuItem();
		saveState4MenuItem = new ToolStripMenuItem();
		saveState5MenuItem = new ToolStripMenuItem();
		loadStateMenuItem = new ToolStripMenuItem();
		loadState1MenuItem = new ToolStripMenuItem();
		loadState2MenuItem = new ToolStripMenuItem();
		loadState3MenuItem = new ToolStripMenuItem();
		loadState4MenuItem = new ToolStripMenuItem();
		loadState5MenuItem = new ToolStripMenuItem();
		toolStripMenuItem4 = new ToolStripSeparator();
		mruListMenuItem = new ToolStripMenuItem();
		toolStripMenuItem2 = new ToolStripSeparator();
		exitToolStripMenuItem = new ToolStripMenuItem();
		gToolStripMenuItem = new ToolStripMenuItem();
		pauseToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem3 = new ToolStripSeparator();
		resetToolStripMenuItem = new ToolStripMenuItem();
		settingsToolStripMenuItem = new ToolStripMenuItem();
		videoSizeToolStripMenuItem = new ToolStripMenuItem();
		cpuSpeed50MenuItem = new ToolStripMenuItem();
		cpuSpeed75MenuItem = new ToolStripMenuItem();
		cpuSpeed100MenuItem = new ToolStripMenuItem();
		cpuSpeed125MenuItem = new ToolStripMenuItem();
		cpuSpeed150MenuItem = new ToolStripMenuItem();
		cpuSpeed175MenuItem = new ToolStripMenuItem();
		cpuSpeed200MenuItem = new ToolStripMenuItem();
		toolStripMenuItem6 = new ToolStripMenuItem();
		fpsToolStripMenuItem = new ToolStripMenuItem();
		videoSizeToolStripMenuItem1 = new ToolStripMenuItem();
		videoSize1MenuItem = new ToolStripMenuItem();
		videoSize2MenuItem = new ToolStripMenuItem();
		videoSize3MenuItem = new ToolStripMenuItem();
		videoSize4MenuItem = new ToolStripMenuItem();
		videoSize5MenuItem = new ToolStripMenuItem();
		videoSize6MenuItem = new ToolStripMenuItem();
		fullScreenToolStripMenuItem = new ToolStripMenuItem();
		debugToolStripMenuItem = new ToolStripMenuItem();
		paletteViewerToolStripMenuItem = new ToolStripMenuItem();
		memoryViewerToolStripMenuItem = new ToolStripMenuItem();
		spriteViewerToolStripMenuItem = new ToolStripMenuItem();
		loggingToolStripMenuItem = new ToolStripMenuItem();
		glControl1 = new OpenTK.WinForms.GLControl();
		textBoxFixForKeyEvents = new TextBox();
		statusStrip1 = new StatusStrip();
		toolStripStatusLabelFPS = new ToolStripStatusLabel();
		timerFPS = new System.Windows.Forms.Timer(components);
		menuStrip1.SuspendLayout();
		statusStrip1.SuspendLayout();
		SuspendLayout();
		// 
		// menuStrip1
		// 
		menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, gToolStripMenuItem, settingsToolStripMenuItem, debugToolStripMenuItem });
		menuStrip1.Location = new Point(0, 0);
		menuStrip1.Name = "menuStrip1";
		menuStrip1.Size = new Size(800, 24);
		menuStrip1.TabIndex = 0;
		menuStrip1.Text = "menuStrip1";
		// 
		// fileToolStripMenuItem
		// 
		fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, toolStripMenuItem1, saveStateMenuItem, loadStateMenuItem, toolStripMenuItem4, mruListMenuItem, toolStripMenuItem2, exitToolStripMenuItem });
		fileToolStripMenuItem.Name = "fileToolStripMenuItem";
		fileToolStripMenuItem.Size = new Size(37, 20);
		fileToolStripMenuItem.Text = "&File";
		// 
		// openToolStripMenuItem
		// 
		openToolStripMenuItem.Name = "openToolStripMenuItem";
		openToolStripMenuItem.Size = new Size(128, 22);
		openToolStripMenuItem.Text = "&Open...";
		openToolStripMenuItem.Click += openToolStripMenuItem_Click;
		// 
		// toolStripMenuItem1
		// 
		toolStripMenuItem1.Name = "toolStripMenuItem1";
		toolStripMenuItem1.Size = new Size(125, 6);
		// 
		// saveStateMenuItem
		// 
		saveStateMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveState1MenuItem, saveState2MenuItem, saveState3MenuItem, saveState4MenuItem, saveState5MenuItem });
		saveStateMenuItem.Name = "saveStateMenuItem";
		saveStateMenuItem.Size = new Size(128, 22);
		saveStateMenuItem.Text = "Save state";
		// 
		// saveState1MenuItem
		// 
		saveState1MenuItem.Name = "saveState1MenuItem";
		saveState1MenuItem.Size = new Size(110, 22);
		saveState1MenuItem.Tag = "1";
		saveState1MenuItem.Text = "Slot #1";
		saveState1MenuItem.Click += saveStateMenuItem_Click;
		// 
		// saveState2MenuItem
		// 
		saveState2MenuItem.Name = "saveState2MenuItem";
		saveState2MenuItem.Size = new Size(110, 22);
		saveState2MenuItem.Tag = "2";
		saveState2MenuItem.Text = "Slot #2";
		saveState2MenuItem.Click += saveStateMenuItem_Click;
		// 
		// saveState3MenuItem
		// 
		saveState3MenuItem.Name = "saveState3MenuItem";
		saveState3MenuItem.Size = new Size(110, 22);
		saveState3MenuItem.Tag = "3";
		saveState3MenuItem.Text = "Slot #3";
		saveState3MenuItem.Click += saveStateMenuItem_Click;
		// 
		// saveState4MenuItem
		// 
		saveState4MenuItem.Name = "saveState4MenuItem";
		saveState4MenuItem.Size = new Size(110, 22);
		saveState4MenuItem.Tag = "4";
		saveState4MenuItem.Text = "Slot #4";
		saveState4MenuItem.Click += saveStateMenuItem_Click;
		// 
		// saveState5MenuItem
		// 
		saveState5MenuItem.Name = "saveState5MenuItem";
		saveState5MenuItem.Size = new Size(110, 22);
		saveState5MenuItem.Tag = "5";
		saveState5MenuItem.Text = "Slot #5";
		saveState5MenuItem.Click += saveStateMenuItem_Click;
		// 
		// loadStateMenuItem
		// 
		loadStateMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadState1MenuItem, loadState2MenuItem, loadState3MenuItem, loadState4MenuItem, loadState5MenuItem });
		loadStateMenuItem.Name = "loadStateMenuItem";
		loadStateMenuItem.Size = new Size(128, 22);
		loadStateMenuItem.Text = "Load state";
		// 
		// loadState1MenuItem
		// 
		loadState1MenuItem.Name = "loadState1MenuItem";
		loadState1MenuItem.Size = new Size(110, 22);
		loadState1MenuItem.Tag = "1";
		loadState1MenuItem.Text = "Slot #1";
		loadState1MenuItem.Click += loadStateMenuItem_Click;
		// 
		// loadState2MenuItem
		// 
		loadState2MenuItem.Name = "loadState2MenuItem";
		loadState2MenuItem.Size = new Size(110, 22);
		loadState2MenuItem.Tag = "2";
		loadState2MenuItem.Text = "Slot #2";
		loadState2MenuItem.Click += loadStateMenuItem_Click;
		// 
		// loadState3MenuItem
		// 
		loadState3MenuItem.Name = "loadState3MenuItem";
		loadState3MenuItem.Size = new Size(110, 22);
		loadState3MenuItem.Tag = "3";
		loadState3MenuItem.Text = "Slot #3";
		loadState3MenuItem.Click += loadStateMenuItem_Click;
		// 
		// loadState4MenuItem
		// 
		loadState4MenuItem.Name = "loadState4MenuItem";
		loadState4MenuItem.Size = new Size(110, 22);
		loadState4MenuItem.Tag = "4";
		loadState4MenuItem.Text = "Slot #4";
		loadState4MenuItem.Click += loadStateMenuItem_Click;
		// 
		// loadState5MenuItem
		// 
		loadState5MenuItem.Name = "loadState5MenuItem";
		loadState5MenuItem.Size = new Size(110, 22);
		loadState5MenuItem.Tag = "5";
		loadState5MenuItem.Text = "Slot #5";
		loadState5MenuItem.Click += loadStateMenuItem_Click;
		// 
		// toolStripMenuItem4
		// 
		toolStripMenuItem4.Name = "toolStripMenuItem4";
		toolStripMenuItem4.Size = new Size(125, 6);
		// 
		// mruListMenuItem
		// 
		mruListMenuItem.Name = "mruListMenuItem";
		mruListMenuItem.Size = new Size(128, 22);
		mruListMenuItem.Text = "&Recent";
		// 
		// toolStripMenuItem2
		// 
		toolStripMenuItem2.Name = "toolStripMenuItem2";
		toolStripMenuItem2.Size = new Size(125, 6);
		// 
		// exitToolStripMenuItem
		// 
		exitToolStripMenuItem.Name = "exitToolStripMenuItem";
		exitToolStripMenuItem.Size = new Size(128, 22);
		exitToolStripMenuItem.Text = "E&xit";
		exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
		// 
		// gToolStripMenuItem
		// 
		gToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { pauseToolStripMenuItem, toolStripMenuItem3, resetToolStripMenuItem });
		gToolStripMenuItem.Name = "gToolStripMenuItem";
		gToolStripMenuItem.Size = new Size(50, 20);
		gToolStripMenuItem.Text = "&Game";
		// 
		// pauseToolStripMenuItem
		// 
		pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
		pauseToolStripMenuItem.Size = new Size(105, 22);
		pauseToolStripMenuItem.Text = "Pause";
		pauseToolStripMenuItem.Click += pauseToolStripMenuItem_Click;
		// 
		// toolStripMenuItem3
		// 
		toolStripMenuItem3.Name = "toolStripMenuItem3";
		toolStripMenuItem3.Size = new Size(102, 6);
		// 
		// resetToolStripMenuItem
		// 
		resetToolStripMenuItem.Name = "resetToolStripMenuItem";
		resetToolStripMenuItem.Size = new Size(105, 22);
		resetToolStripMenuItem.Text = "Reset";
		resetToolStripMenuItem.Click += resetToolStripMenuItem_Click;
		// 
		// settingsToolStripMenuItem
		// 
		settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { videoSizeToolStripMenuItem, videoSizeToolStripMenuItem1 });
		settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
		settingsToolStripMenuItem.Size = new Size(61, 20);
		settingsToolStripMenuItem.Text = "&Settings";
		// 
		// videoSizeToolStripMenuItem
		// 
		videoSizeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cpuSpeed50MenuItem, cpuSpeed75MenuItem, cpuSpeed100MenuItem, cpuSpeed125MenuItem, cpuSpeed150MenuItem, cpuSpeed175MenuItem, cpuSpeed200MenuItem, toolStripMenuItem6, fpsToolStripMenuItem });
		videoSizeToolStripMenuItem.Name = "videoSizeToolStripMenuItem";
		videoSizeToolStripMenuItem.Size = new Size(127, 22);
		videoSizeToolStripMenuItem.Text = "Speed";
		// 
		// cpuSpeed50MenuItem
		// 
		cpuSpeed50MenuItem.Name = "cpuSpeed50MenuItem";
		cpuSpeed50MenuItem.Size = new Size(150, 22);
		cpuSpeed50MenuItem.Tag = "30";
		cpuSpeed50MenuItem.Text = "50% (30 fps)";
		cpuSpeed50MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed75MenuItem
		// 
		cpuSpeed75MenuItem.Name = "cpuSpeed75MenuItem";
		cpuSpeed75MenuItem.Size = new Size(150, 22);
		cpuSpeed75MenuItem.Tag = "45";
		cpuSpeed75MenuItem.Text = "75% (45 fps)";
		cpuSpeed75MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed100MenuItem
		// 
		cpuSpeed100MenuItem.Name = "cpuSpeed100MenuItem";
		cpuSpeed100MenuItem.Size = new Size(150, 22);
		cpuSpeed100MenuItem.Tag = "60";
		cpuSpeed100MenuItem.Text = "100 % (60 fps)";
		cpuSpeed100MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed125MenuItem
		// 
		cpuSpeed125MenuItem.Name = "cpuSpeed125MenuItem";
		cpuSpeed125MenuItem.Size = new Size(150, 22);
		cpuSpeed125MenuItem.Tag = "75";
		cpuSpeed125MenuItem.Text = "125% (75 fps)";
		cpuSpeed125MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed150MenuItem
		// 
		cpuSpeed150MenuItem.Name = "cpuSpeed150MenuItem";
		cpuSpeed150MenuItem.Size = new Size(150, 22);
		cpuSpeed150MenuItem.Tag = "90";
		cpuSpeed150MenuItem.Text = "150% (90 fps)";
		cpuSpeed150MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed175MenuItem
		// 
		cpuSpeed175MenuItem.Name = "cpuSpeed175MenuItem";
		cpuSpeed175MenuItem.Size = new Size(150, 22);
		cpuSpeed175MenuItem.Tag = "105";
		cpuSpeed175MenuItem.Text = "175% (105 fps)";
		cpuSpeed175MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// cpuSpeed200MenuItem
		// 
		cpuSpeed200MenuItem.Name = "cpuSpeed200MenuItem";
		cpuSpeed200MenuItem.Size = new Size(150, 22);
		cpuSpeed200MenuItem.Tag = "120";
		cpuSpeed200MenuItem.Text = "200% (120 fps)";
		cpuSpeed200MenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// toolStripMenuItem6
		// 
		toolStripMenuItem6.Name = "toolStripMenuItem6";
		toolStripMenuItem6.Size = new Size(150, 22);
		toolStripMenuItem6.Tag = "180";
		toolStripMenuItem6.Text = "300% (180 fps)";
		toolStripMenuItem6.Click += cpuSpeedMenuItem_Click;
		// 
		// fpsToolStripMenuItem
		// 
		fpsToolStripMenuItem.Name = "fpsToolStripMenuItem";
		fpsToolStripMenuItem.Size = new Size(150, 22);
		fpsToolStripMenuItem.Tag = "240";
		fpsToolStripMenuItem.Text = "400% (240 fps)";
		fpsToolStripMenuItem.Click += cpuSpeedMenuItem_Click;
		// 
		// videoSizeToolStripMenuItem1
		// 
		videoSizeToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { videoSize1MenuItem, videoSize2MenuItem, videoSize3MenuItem, videoSize4MenuItem, videoSize5MenuItem, videoSize6MenuItem, fullScreenToolStripMenuItem });
		videoSizeToolStripMenuItem1.Name = "videoSizeToolStripMenuItem1";
		videoSizeToolStripMenuItem1.Size = new Size(127, 22);
		videoSizeToolStripMenuItem1.Text = "&Video Size";
		// 
		// videoSize1MenuItem
		// 
		videoSize1MenuItem.Name = "videoSize1MenuItem";
		videoSize1MenuItem.Size = new Size(151, 22);
		videoSize1MenuItem.Tag = "1";
		videoSize1MenuItem.Text = "1x (256x240)";
		videoSize1MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// videoSize2MenuItem
		// 
		videoSize2MenuItem.Name = "videoSize2MenuItem";
		videoSize2MenuItem.Size = new Size(151, 22);
		videoSize2MenuItem.Tag = "2";
		videoSize2MenuItem.Text = "2x (512x480)";
		videoSize2MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// videoSize3MenuItem
		// 
		videoSize3MenuItem.Name = "videoSize3MenuItem";
		videoSize3MenuItem.Size = new Size(151, 22);
		videoSize3MenuItem.Tag = "3";
		videoSize3MenuItem.Text = "3x (768x720)";
		videoSize3MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// videoSize4MenuItem
		// 
		videoSize4MenuItem.Name = "videoSize4MenuItem";
		videoSize4MenuItem.Size = new Size(151, 22);
		videoSize4MenuItem.Tag = "4";
		videoSize4MenuItem.Text = "4x (1024x960)";
		videoSize4MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// videoSize5MenuItem
		// 
		videoSize5MenuItem.Name = "videoSize5MenuItem";
		videoSize5MenuItem.Size = new Size(151, 22);
		videoSize5MenuItem.Tag = "5";
		videoSize5MenuItem.Text = "5x (1280x1200)";
		videoSize5MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// videoSize6MenuItem
		// 
		videoSize6MenuItem.Name = "videoSize6MenuItem";
		videoSize6MenuItem.Size = new Size(151, 22);
		videoSize6MenuItem.Tag = "6";
		videoSize6MenuItem.Text = "6x (1536x1440)";
		videoSize6MenuItem.Click += videoSizeMenuItem_Click;
		// 
		// fullScreenToolStripMenuItem
		// 
		fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
		fullScreenToolStripMenuItem.Size = new Size(151, 22);
		fullScreenToolStripMenuItem.Text = "Full Screen";
		// 
		// debugToolStripMenuItem
		// 
		debugToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { paletteViewerToolStripMenuItem, memoryViewerToolStripMenuItem, spriteViewerToolStripMenuItem, loggingToolStripMenuItem });
		debugToolStripMenuItem.Name = "debugToolStripMenuItem";
		debugToolStripMenuItem.Size = new Size(54, 20);
		debugToolStripMenuItem.Text = "&Debug";
		// 
		// paletteViewerToolStripMenuItem
		// 
		paletteViewerToolStripMenuItem.Name = "paletteViewerToolStripMenuItem";
		paletteViewerToolStripMenuItem.Size = new Size(166, 22);
		paletteViewerToolStripMenuItem.Text = "Palette Viewer...";
		paletteViewerToolStripMenuItem.Click += paletteViewerToolStripMenuItem_Click;
		// 
		// memoryViewerToolStripMenuItem
		// 
		memoryViewerToolStripMenuItem.Name = "memoryViewerToolStripMenuItem";
		memoryViewerToolStripMenuItem.Size = new Size(166, 22);
		memoryViewerToolStripMenuItem.Text = "Memory Viewer...";
		memoryViewerToolStripMenuItem.Click += memoryViewerToolStripMenuItem_Click;
		// 
		// spriteViewerToolStripMenuItem
		// 
		spriteViewerToolStripMenuItem.Name = "spriteViewerToolStripMenuItem";
		spriteViewerToolStripMenuItem.Size = new Size(166, 22);
		spriteViewerToolStripMenuItem.Text = "Sprite Viewer...";
		spriteViewerToolStripMenuItem.Click += spriteViewerToolStripMenuItem_Click;
		// 
		// loggingToolStripMenuItem
		// 
		loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
		loggingToolStripMenuItem.Size = new Size(166, 22);
		loggingToolStripMenuItem.Text = "Logging...";
		loggingToolStripMenuItem.Click += loggingToolStripMenuItem_Click;
		// 
		// glControl1
		// 
		glControl1.Anchor = AnchorStyles.None;
		glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
		glControl1.APIVersion = new Version(4, 0, 0, 0);
		glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
		glControl1.IsEventDriven = true;
		glControl1.Location = new Point(583, 299);
		glControl1.Name = "glControl1";
		glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
		glControl1.SharedContext = null;
		glControl1.Size = new Size(147, 98);
		glControl1.TabIndex = 1;
		glControl1.Text = "glControl1";
		// 
		// textBoxFixForKeyEvents
		// 
		textBoxFixForKeyEvents.Location = new Point(137, 128);
		textBoxFixForKeyEvents.Name = "textBoxFixForKeyEvents";
		textBoxFixForKeyEvents.Size = new Size(100, 23);
		textBoxFixForKeyEvents.TabIndex = 2;
		// 
		// statusStrip1
		// 
		statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelFPS });
		statusStrip1.Location = new Point(0, 428);
		statusStrip1.Name = "statusStrip1";
		statusStrip1.Size = new Size(800, 22);
		statusStrip1.SizingGrip = false;
		statusStrip1.TabIndex = 3;
		statusStrip1.Text = "statusStrip1";
		// 
		// toolStripStatusLabelFPS
		// 
		toolStripStatusLabelFPS.Name = "toolStripStatusLabelFPS";
		toolStripStatusLabelFPS.Size = new Size(37, 17);
		toolStripStatusLabelFPS.Text = "FPS: -";
		// 
		// timerFPS
		// 
		timerFPS.Enabled = true;
		timerFPS.Interval = 1000;
		timerFPS.Tick += timerFPS_Tick;
		// 
		// Launcher
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(statusStrip1);
		Controls.Add(textBoxFixForKeyEvents);
		Controls.Add(glControl1);
		Controls.Add(menuStrip1);
		FormBorderStyle = FormBorderStyle.FixedSingle;
		KeyPreview = true;
		MainMenuStrip = menuStrip1;
		Name = "Launcher";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "NEScafé";
		menuStrip1.ResumeLayout(false);
		menuStrip1.PerformLayout();
		statusStrip1.ResumeLayout(false);
		statusStrip1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private MenuStrip menuStrip1;
	private ToolStripMenuItem fileToolStripMenuItem;
	private ToolStripMenuItem openToolStripMenuItem;
	private ToolStripSeparator toolStripMenuItem1;
	private ToolStripMenuItem exitToolStripMenuItem;
	private ToolStripMenuItem saveStateMenuItem;
	private ToolStripMenuItem saveState1MenuItem;
	private ToolStripMenuItem saveState2MenuItem;
	private ToolStripMenuItem saveState3MenuItem;
	private ToolStripMenuItem saveState4MenuItem;
	private ToolStripMenuItem saveState5MenuItem;
	private ToolStripMenuItem loadStateMenuItem;
	private ToolStripMenuItem loadState1MenuItem;
	private ToolStripMenuItem loadState2MenuItem;
	private ToolStripMenuItem loadState3MenuItem;
	private ToolStripMenuItem loadState4MenuItem;
	private ToolStripMenuItem loadState5MenuItem;
	private ToolStripSeparator toolStripMenuItem2;
	private ToolStripMenuItem gToolStripMenuItem;
	private ToolStripMenuItem pauseToolStripMenuItem;
	private ToolStripSeparator toolStripMenuItem3;
	private ToolStripMenuItem resetToolStripMenuItem;
	private ToolStripMenuItem settingsToolStripMenuItem;
	private ToolStripMenuItem videoSizeToolStripMenuItem;
	private ToolStripMenuItem cpuSpeed100MenuItem;
	private ToolStripMenuItem cpuSpeed125MenuItem;
	private ToolStripMenuItem cpuSpeed150MenuItem;
	private ToolStripMenuItem cpuSpeed175MenuItem;
	private ToolStripMenuItem cpuSpeed200MenuItem;
	private ToolStripMenuItem cpuSpeed50MenuItem;
	private ToolStripMenuItem cpuSpeed75MenuItem;
	private ToolStripMenuItem videoSizeToolStripMenuItem1;
	private ToolStripMenuItem videoSize1MenuItem;
	private ToolStripMenuItem videoSize2MenuItem;
	private ToolStripMenuItem videoSize3MenuItem;
	private ToolStripMenuItem videoSize4MenuItem;
	private ToolStripMenuItem videoSize5MenuItem;
	private ToolStripMenuItem videoSize6MenuItem;
	private ToolStripMenuItem fullScreenToolStripMenuItem;
	private OpenTK.WinForms.GLControl glControl1;
	private ToolStripSeparator toolStripMenuItem4;
	private ToolStripMenuItem mruListMenuItem;
	private TextBox textBoxFixForKeyEvents;
	private ToolStripMenuItem debugToolStripMenuItem;
	private ToolStripMenuItem paletteViewerToolStripMenuItem;
	private ToolStripMenuItem memoryViewerToolStripMenuItem;
	private ToolStripMenuItem spriteViewerToolStripMenuItem;
	private ToolStripMenuItem loggingToolStripMenuItem;
	private StatusStrip statusStrip1;
	private ToolStripStatusLabel toolStripStatusLabelFPS;
	private System.Windows.Forms.Timer timerFPS;
	private ToolStripMenuItem toolStripMenuItem6;
	private ToolStripMenuItem fpsToolStripMenuItem;
}