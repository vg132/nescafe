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
		menuStrip1 = new MenuStrip();
		fileToolStripMenuItem = new ToolStripMenuItem();
		openToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem1 = new ToolStripSeparator();
		saveStateToolStripMenuItem = new ToolStripMenuItem();
		slot1ToolStripMenuItem = new ToolStripMenuItem();
		slot2ToolStripMenuItem = new ToolStripMenuItem();
		slot3ToolStripMenuItem = new ToolStripMenuItem();
		slot4ToolStripMenuItem = new ToolStripMenuItem();
		slot5ToolStripMenuItem = new ToolStripMenuItem();
		loadStateToolStripMenuItem = new ToolStripMenuItem();
		slot1ToolStripMenuItem1 = new ToolStripMenuItem();
		slot2ToolStripMenuItem1 = new ToolStripMenuItem();
		slot3ToolStripMenuItem1 = new ToolStripMenuItem();
		slot4ToolStripMenuItem1 = new ToolStripMenuItem();
		slot5ToolStripMenuItem1 = new ToolStripMenuItem();
		toolStripMenuItem2 = new ToolStripSeparator();
		exitToolStripMenuItem = new ToolStripMenuItem();
		gToolStripMenuItem = new ToolStripMenuItem();
		pauseToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem3 = new ToolStripSeparator();
		resetToolStripMenuItem = new ToolStripMenuItem();
		settingsToolStripMenuItem = new ToolStripMenuItem();
		videoSizeToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem9 = new ToolStripMenuItem();
		toolStripMenuItem10 = new ToolStripMenuItem();
		normalToolStripMenuItem = new ToolStripMenuItem();
		toolStripMenuItem5 = new ToolStripMenuItem();
		toolStripMenuItem6 = new ToolStripMenuItem();
		toolStripMenuItem7 = new ToolStripMenuItem();
		toolStripMenuItem8 = new ToolStripMenuItem();
		videoSizeToolStripMenuItem1 = new ToolStripMenuItem();
		xToolStripMenuItem = new ToolStripMenuItem();
		xToolStripMenuItem1 = new ToolStripMenuItem();
		xToolStripMenuItem2 = new ToolStripMenuItem();
		toolStripMenuItem11 = new ToolStripMenuItem();
		xToolStripMenuItem3 = new ToolStripMenuItem();
		xToolStripMenuItem4 = new ToolStripMenuItem();
		fullScreenToolStripMenuItem = new ToolStripMenuItem();
		glControl1 = new OpenTK.WinForms.GLControl();
		menuStrip1.SuspendLayout();
		SuspendLayout();
		// 
		// menuStrip1
		// 
		menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, gToolStripMenuItem, settingsToolStripMenuItem });
		menuStrip1.Location = new Point(0, 0);
		menuStrip1.Name = "menuStrip1";
		menuStrip1.Size = new Size(800, 24);
		menuStrip1.TabIndex = 0;
		menuStrip1.Text = "menuStrip1";
		// 
		// fileToolStripMenuItem
		// 
		fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, toolStripMenuItem1, saveStateToolStripMenuItem, loadStateToolStripMenuItem, toolStripMenuItem2, exitToolStripMenuItem });
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
		// saveStateToolStripMenuItem
		// 
		saveStateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { slot1ToolStripMenuItem, slot2ToolStripMenuItem, slot3ToolStripMenuItem, slot4ToolStripMenuItem, slot5ToolStripMenuItem });
		saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
		saveStateToolStripMenuItem.Size = new Size(128, 22);
		saveStateToolStripMenuItem.Text = "Save state";
		// 
		// slot1ToolStripMenuItem
		// 
		slot1ToolStripMenuItem.Name = "slot1ToolStripMenuItem";
		slot1ToolStripMenuItem.Size = new Size(110, 22);
		slot1ToolStripMenuItem.Text = "Slot #1";
		// 
		// slot2ToolStripMenuItem
		// 
		slot2ToolStripMenuItem.Name = "slot2ToolStripMenuItem";
		slot2ToolStripMenuItem.Size = new Size(110, 22);
		slot2ToolStripMenuItem.Text = "Slot #2";
		// 
		// slot3ToolStripMenuItem
		// 
		slot3ToolStripMenuItem.Name = "slot3ToolStripMenuItem";
		slot3ToolStripMenuItem.Size = new Size(110, 22);
		slot3ToolStripMenuItem.Text = "Slot #3";
		// 
		// slot4ToolStripMenuItem
		// 
		slot4ToolStripMenuItem.Name = "slot4ToolStripMenuItem";
		slot4ToolStripMenuItem.Size = new Size(110, 22);
		slot4ToolStripMenuItem.Text = "Slot #4";
		// 
		// slot5ToolStripMenuItem
		// 
		slot5ToolStripMenuItem.Name = "slot5ToolStripMenuItem";
		slot5ToolStripMenuItem.Size = new Size(110, 22);
		slot5ToolStripMenuItem.Text = "Slot #5";
		// 
		// loadStateToolStripMenuItem
		// 
		loadStateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { slot1ToolStripMenuItem1, slot2ToolStripMenuItem1, slot3ToolStripMenuItem1, slot4ToolStripMenuItem1, slot5ToolStripMenuItem1 });
		loadStateToolStripMenuItem.Name = "loadStateToolStripMenuItem";
		loadStateToolStripMenuItem.Size = new Size(128, 22);
		loadStateToolStripMenuItem.Text = "Load state";
		// 
		// slot1ToolStripMenuItem1
		// 
		slot1ToolStripMenuItem1.Name = "slot1ToolStripMenuItem1";
		slot1ToolStripMenuItem1.Size = new Size(110, 22);
		slot1ToolStripMenuItem1.Text = "Slot #1";
		// 
		// slot2ToolStripMenuItem1
		// 
		slot2ToolStripMenuItem1.Name = "slot2ToolStripMenuItem1";
		slot2ToolStripMenuItem1.Size = new Size(110, 22);
		slot2ToolStripMenuItem1.Text = "Slot #2";
		// 
		// slot3ToolStripMenuItem1
		// 
		slot3ToolStripMenuItem1.Name = "slot3ToolStripMenuItem1";
		slot3ToolStripMenuItem1.Size = new Size(110, 22);
		slot3ToolStripMenuItem1.Text = "Slot #3";
		// 
		// slot4ToolStripMenuItem1
		// 
		slot4ToolStripMenuItem1.Name = "slot4ToolStripMenuItem1";
		slot4ToolStripMenuItem1.Size = new Size(110, 22);
		slot4ToolStripMenuItem1.Text = "Slot #4";
		// 
		// slot5ToolStripMenuItem1
		// 
		slot5ToolStripMenuItem1.Name = "slot5ToolStripMenuItem1";
		slot5ToolStripMenuItem1.Size = new Size(110, 22);
		slot5ToolStripMenuItem1.Text = "Slot #5";
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
		videoSizeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem9, toolStripMenuItem10, normalToolStripMenuItem, toolStripMenuItem5, toolStripMenuItem6, toolStripMenuItem7, toolStripMenuItem8 });
		videoSizeToolStripMenuItem.Name = "videoSizeToolStripMenuItem";
		videoSizeToolStripMenuItem.Size = new Size(127, 22);
		videoSizeToolStripMenuItem.Text = "Speed";
		videoSizeToolStripMenuItem.Click += videoSizeToolStripMenuItem_Click;
		// 
		// toolStripMenuItem9
		// 
		toolStripMenuItem9.Name = "toolStripMenuItem9";
		toolStripMenuItem9.Size = new Size(148, 22);
		toolStripMenuItem9.Text = "50%";
		toolStripMenuItem9.Click += toolStripMenuItem9_Click;
		// 
		// toolStripMenuItem10
		// 
		toolStripMenuItem10.Name = "toolStripMenuItem10";
		toolStripMenuItem10.Size = new Size(148, 22);
		toolStripMenuItem10.Text = "75%";
		// 
		// normalToolStripMenuItem
		// 
		normalToolStripMenuItem.Name = "normalToolStripMenuItem";
		normalToolStripMenuItem.Size = new Size(148, 22);
		normalToolStripMenuItem.Text = "100 % Normal";
		// 
		// toolStripMenuItem5
		// 
		toolStripMenuItem5.Name = "toolStripMenuItem5";
		toolStripMenuItem5.Size = new Size(148, 22);
		toolStripMenuItem5.Text = "125%";
		toolStripMenuItem5.Click += toolStripMenuItem5_Click;
		// 
		// toolStripMenuItem6
		// 
		toolStripMenuItem6.Name = "toolStripMenuItem6";
		toolStripMenuItem6.Size = new Size(148, 22);
		toolStripMenuItem6.Text = "150%";
		// 
		// toolStripMenuItem7
		// 
		toolStripMenuItem7.Name = "toolStripMenuItem7";
		toolStripMenuItem7.Size = new Size(148, 22);
		toolStripMenuItem7.Text = "200%";
		// 
		// toolStripMenuItem8
		// 
		toolStripMenuItem8.Name = "toolStripMenuItem8";
		toolStripMenuItem8.Size = new Size(148, 22);
		toolStripMenuItem8.Text = "300%";
		// 
		// videoSizeToolStripMenuItem1
		// 
		videoSizeToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { xToolStripMenuItem, xToolStripMenuItem1, xToolStripMenuItem2, toolStripMenuItem11, xToolStripMenuItem3, xToolStripMenuItem4, fullScreenToolStripMenuItem });
		videoSizeToolStripMenuItem1.Name = "videoSizeToolStripMenuItem1";
		videoSizeToolStripMenuItem1.Size = new Size(127, 22);
		videoSizeToolStripMenuItem1.Text = "&Video Size";
		// 
		// xToolStripMenuItem
		// 
		xToolStripMenuItem.Name = "xToolStripMenuItem";
		xToolStripMenuItem.Size = new Size(131, 22);
		xToolStripMenuItem.Text = "1x";
		// 
		// xToolStripMenuItem1
		// 
		xToolStripMenuItem1.Name = "xToolStripMenuItem1";
		xToolStripMenuItem1.Size = new Size(131, 22);
		xToolStripMenuItem1.Text = "2x";
		// 
		// xToolStripMenuItem2
		// 
		xToolStripMenuItem2.Name = "xToolStripMenuItem2";
		xToolStripMenuItem2.Size = new Size(131, 22);
		xToolStripMenuItem2.Text = "3x";
		// 
		// toolStripMenuItem11
		// 
		toolStripMenuItem11.Name = "toolStripMenuItem11";
		toolStripMenuItem11.Size = new Size(131, 22);
		toolStripMenuItem11.Text = "4x";
		// 
		// xToolStripMenuItem3
		// 
		xToolStripMenuItem3.Name = "xToolStripMenuItem3";
		xToolStripMenuItem3.Size = new Size(131, 22);
		xToolStripMenuItem3.Text = "5x";
		// 
		// xToolStripMenuItem4
		// 
		xToolStripMenuItem4.Name = "xToolStripMenuItem4";
		xToolStripMenuItem4.Size = new Size(131, 22);
		xToolStripMenuItem4.Text = "6x";
		// 
		// fullScreenToolStripMenuItem
		// 
		fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
		fullScreenToolStripMenuItem.Size = new Size(131, 22);
		fullScreenToolStripMenuItem.Text = "Full Screen";
		// 
		// glControl1
		// 
		glControl1.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
		glControl1.APIVersion = new Version(3, 3, 0, 0);
		glControl1.Dock = DockStyle.Fill;
		glControl1.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
		glControl1.IsEventDriven = true;
		glControl1.Location = new Point(0, 24);
		glControl1.Name = "glControl1";
		glControl1.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
		glControl1.SharedContext = null;
		glControl1.Size = new Size(800, 426);
		glControl1.TabIndex = 1;
		glControl1.Text = "glControl1";
		// 
		// Launcher
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(glControl1);
		Controls.Add(menuStrip1);
		MainMenuStrip = menuStrip1;
		Name = "Launcher";
		Text = "Form1";
		menuStrip1.ResumeLayout(false);
		menuStrip1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private MenuStrip menuStrip1;
	private ToolStripMenuItem fileToolStripMenuItem;
	private ToolStripMenuItem openToolStripMenuItem;
	private ToolStripSeparator toolStripMenuItem1;
	private ToolStripMenuItem exitToolStripMenuItem;
	private ToolStripMenuItem saveStateToolStripMenuItem;
	private ToolStripMenuItem slot1ToolStripMenuItem;
	private ToolStripMenuItem slot2ToolStripMenuItem;
	private ToolStripMenuItem slot3ToolStripMenuItem;
	private ToolStripMenuItem slot4ToolStripMenuItem;
	private ToolStripMenuItem slot5ToolStripMenuItem;
	private ToolStripMenuItem loadStateToolStripMenuItem;
	private ToolStripMenuItem slot1ToolStripMenuItem1;
	private ToolStripMenuItem slot2ToolStripMenuItem1;
	private ToolStripMenuItem slot3ToolStripMenuItem1;
	private ToolStripMenuItem slot4ToolStripMenuItem1;
	private ToolStripMenuItem slot5ToolStripMenuItem1;
	private ToolStripSeparator toolStripMenuItem2;
	private ToolStripMenuItem gToolStripMenuItem;
	private ToolStripMenuItem pauseToolStripMenuItem;
	private ToolStripSeparator toolStripMenuItem3;
	private ToolStripMenuItem resetToolStripMenuItem;
	private ToolStripMenuItem settingsToolStripMenuItem;
	private ToolStripMenuItem videoSizeToolStripMenuItem;
	private ToolStripMenuItem normalToolStripMenuItem;
	private ToolStripMenuItem toolStripMenuItem5;
	private ToolStripMenuItem toolStripMenuItem6;
	private ToolStripMenuItem toolStripMenuItem7;
	private ToolStripMenuItem toolStripMenuItem8;
	private ToolStripMenuItem toolStripMenuItem9;
	private ToolStripMenuItem toolStripMenuItem10;
	private ToolStripMenuItem videoSizeToolStripMenuItem1;
	private ToolStripMenuItem xToolStripMenuItem;
	private ToolStripMenuItem xToolStripMenuItem1;
	private ToolStripMenuItem xToolStripMenuItem2;
	private ToolStripMenuItem toolStripMenuItem11;
	private ToolStripMenuItem xToolStripMenuItem3;
	private ToolStripMenuItem xToolStripMenuItem4;
	private ToolStripMenuItem fullScreenToolStripMenuItem;
	private OpenTK.WinForms.GLControl glControl1;
}