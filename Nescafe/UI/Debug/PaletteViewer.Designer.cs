namespace Nescafe.UI.Debug;

partial class PaletteViewer
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
		pnlPalettePreview = new Panel();
		menuStrip1 = new MenuStrip();
		fileToolStripMenuItem = new ToolStripMenuItem();
		closeToolStripMenuItem = new ToolStripMenuItem();
		viewToolStripMenuItem = new ToolStripMenuItem();
		refreshToolStripMenuItem = new ToolStripMenuItem();
		grpColorInfo = new GroupBox();
		lblIndex = new Label();
		lblValue = new Label();
		lblColorHEX = new Label();
		lblColorRGB = new Label();
		label5 = new Label();
		label4 = new Label();
		label3 = new Label();
		label2 = new Label();
		label1 = new Label();
		pnlColorPreview = new Panel();
		menuStrip1.SuspendLayout();
		grpColorInfo.SuspendLayout();
		SuspendLayout();
		// 
		// pnlPalettePreview
		// 
		pnlPalettePreview.Location = new Point(12, 27);
		pnlPalettePreview.Name = "pnlPalettePreview";
		pnlPalettePreview.Size = new Size(128, 256);
		pnlPalettePreview.TabIndex = 0;
		pnlPalettePreview.Click += PnlPalettePreview_Click;
		pnlPalettePreview.Paint += PnlPalettePreview_OnPaint;
		pnlPalettePreview.MouseMove += PnlPalettePreview_MouseMove;
		// 
		// menuStrip1
		// 
		menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, viewToolStripMenuItem });
		menuStrip1.Location = new Point(0, 0);
		menuStrip1.Name = "menuStrip1";
		menuStrip1.Size = new Size(384, 24);
		menuStrip1.TabIndex = 1;
		menuStrip1.Text = "menuStrip1";
		// 
		// fileToolStripMenuItem
		// 
		fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { closeToolStripMenuItem });
		fileToolStripMenuItem.Name = "fileToolStripMenuItem";
		fileToolStripMenuItem.Size = new Size(37, 20);
		fileToolStripMenuItem.Text = "&File";
		// 
		// closeToolStripMenuItem
		// 
		closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		closeToolStripMenuItem.Size = new Size(103, 22);
		closeToolStripMenuItem.Text = "&Close";
		closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
		// 
		// viewToolStripMenuItem
		// 
		viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { refreshToolStripMenuItem });
		viewToolStripMenuItem.Name = "viewToolStripMenuItem";
		viewToolStripMenuItem.Size = new Size(44, 20);
		viewToolStripMenuItem.Text = "&View";
		// 
		// refreshToolStripMenuItem
		// 
		refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		refreshToolStripMenuItem.Size = new Size(113, 22);
		refreshToolStripMenuItem.Text = "Refresh";
		refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
		// 
		// grpColorInfo
		// 
		grpColorInfo.Controls.Add(lblIndex);
		grpColorInfo.Controls.Add(lblValue);
		grpColorInfo.Controls.Add(lblColorHEX);
		grpColorInfo.Controls.Add(lblColorRGB);
		grpColorInfo.Controls.Add(label5);
		grpColorInfo.Controls.Add(label4);
		grpColorInfo.Controls.Add(label3);
		grpColorInfo.Controls.Add(label2);
		grpColorInfo.Controls.Add(label1);
		grpColorInfo.Controls.Add(pnlColorPreview);
		grpColorInfo.Location = new Point(146, 27);
		grpColorInfo.Name = "grpColorInfo";
		grpColorInfo.Size = new Size(227, 256);
		grpColorInfo.TabIndex = 2;
		grpColorInfo.TabStop = false;
		grpColorInfo.Text = "Info";
		// 
		// lblIndex
		// 
		lblIndex.AutoSize = true;
		lblIndex.Location = new Point(118, 54);
		lblIndex.Name = "lblIndex";
		lblIndex.Size = new Size(38, 15);
		lblIndex.TabIndex = 9;
		lblIndex.Text = "label9";
		// 
		// lblValue
		// 
		lblValue.AutoSize = true;
		lblValue.Location = new Point(118, 74);
		lblValue.Name = "lblValue";
		lblValue.Size = new Size(38, 15);
		lblValue.TabIndex = 8;
		lblValue.Text = "label8";
		// 
		// lblColorHEX
		// 
		lblColorHEX.AutoSize = true;
		lblColorHEX.Location = new Point(118, 114);
		lblColorHEX.Name = "lblColorHEX";
		lblColorHEX.Size = new Size(38, 15);
		lblColorHEX.TabIndex = 7;
		lblColorHEX.Text = "label7";
		// 
		// lblColorRGB
		// 
		lblColorRGB.AutoSize = true;
		lblColorRGB.Location = new Point(118, 94);
		lblColorRGB.Name = "lblColorRGB";
		lblColorRGB.Size = new Size(38, 15);
		lblColorRGB.TabIndex = 6;
		lblColorRGB.Text = "label6";
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Location = new Point(6, 114);
		label5.Name = "label5";
		label5.Size = new Size(61, 15);
		label5.TabIndex = 5;
		label5.Text = "Color HEX";
		// 
		// label4
		// 
		label4.AutoSize = true;
		label4.Location = new Point(6, 94);
		label4.Name = "label4";
		label4.Size = new Size(61, 15);
		label4.TabIndex = 4;
		label4.Text = "Color RGB";
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Location = new Point(6, 74);
		label3.Name = "label3";
		label3.Size = new Size(35, 15);
		label3.TabIndex = 3;
		label3.Text = "Value";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(6, 54);
		label2.Name = "label2";
		label2.Size = new Size(36, 15);
		label2.TabIndex = 2;
		label2.Text = "Index";
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(6, 19);
		label1.Name = "label1";
		label1.Size = new Size(36, 15);
		label1.TabIndex = 1;
		label1.Text = "Color";
		// 
		// pnlColorPreview
		// 
		pnlColorPreview.BorderStyle = BorderStyle.Fixed3D;
		pnlColorPreview.Location = new Point(118, 19);
		pnlColorPreview.Name = "pnlColorPreview";
		pnlColorPreview.Size = new Size(32, 32);
		pnlColorPreview.TabIndex = 0;
		// 
		// PaletteViewer
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(384, 295);
		Controls.Add(grpColorInfo);
		Controls.Add(pnlPalettePreview);
		Controls.Add(menuStrip1);
		MainMenuStrip = menuStrip1;
		Name = "PaletteViewer";
		Text = "Palette Viewer";
		menuStrip1.ResumeLayout(false);
		menuStrip1.PerformLayout();
		grpColorInfo.ResumeLayout(false);
		grpColorInfo.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Panel pnlPalettePreview;
	private MenuStrip menuStrip1;
	private ToolStripMenuItem fileToolStripMenuItem;
	private ToolStripMenuItem closeToolStripMenuItem;
	private ToolStripMenuItem viewToolStripMenuItem;
	private ToolStripMenuItem refreshToolStripMenuItem;
	private GroupBox grpColorInfo;
	private Panel pnlColorPreview;
	private Label label5;
	private Label label4;
	private Label label3;
	private Label label2;
	private Label label1;
	private Label lblIndex;
	private Label lblValue;
	private Label lblColorHEX;
	private Label lblColorRGB;
}