namespace Nescafe.UI.Debug;

partial class Logging
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
		groupBox1 = new GroupBox();
		chkCpu = new CheckBox();
		chkPpu = new CheckBox();
		chkFrame = new CheckBox();
		btnSelectOutputFolder = new Button();
		txtOutputFolder = new TextBox();
		label1 = new Label();
		btnCancel = new Button();
		btnSave = new Button();
		chkMapper = new CheckBox();
		chkCpuMemory = new CheckBox();
		chkPpuMemory = new CheckBox();
		chkCartridge = new CheckBox();
		chkOther = new CheckBox();
		groupBox1.SuspendLayout();
		SuspendLayout();
		// 
		// groupBox1
		// 
		groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		groupBox1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
		groupBox1.Controls.Add(chkOther);
		groupBox1.Controls.Add(chkCartridge);
		groupBox1.Controls.Add(chkPpuMemory);
		groupBox1.Controls.Add(chkCpuMemory);
		groupBox1.Controls.Add(chkMapper);
		groupBox1.Controls.Add(chkCpu);
		groupBox1.Controls.Add(chkPpu);
		groupBox1.Controls.Add(chkFrame);
		groupBox1.Controls.Add(btnSelectOutputFolder);
		groupBox1.Controls.Add(txtOutputFolder);
		groupBox1.Controls.Add(label1);
		groupBox1.Location = new Point(12, 12);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(401, 152);
		groupBox1.TabIndex = 0;
		groupBox1.TabStop = false;
		groupBox1.Text = "Settings";
		// 
		// chkCpu
		// 
		chkCpu.AutoSize = true;
		chkCpu.Location = new Point(6, 116);
		chkCpu.Name = "chkCpu";
		chkCpu.Size = new Size(48, 19);
		chkCpu.TabIndex = 5;
		chkCpu.Text = "Cpu";
		chkCpu.UseVisualStyleBackColor = true;
		// 
		// chkPpu
		// 
		chkPpu.AutoSize = true;
		chkPpu.Location = new Point(6, 91);
		chkPpu.Name = "chkPpu";
		chkPpu.Size = new Size(47, 19);
		chkPpu.TabIndex = 4;
		chkPpu.Text = "Ppu";
		chkPpu.UseVisualStyleBackColor = true;
		// 
		// chkFrame
		// 
		chkFrame.AutoSize = true;
		chkFrame.Location = new Point(6, 66);
		chkFrame.Name = "chkFrame";
		chkFrame.Size = new Size(59, 19);
		chkFrame.TabIndex = 3;
		chkFrame.Text = "Frame";
		chkFrame.UseVisualStyleBackColor = true;
		// 
		// btnSelectOutputFolder
		// 
		btnSelectOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		btnSelectOutputFolder.Location = new Point(282, 37);
		btnSelectOutputFolder.Name = "btnSelectOutputFolder";
		btnSelectOutputFolder.Size = new Size(113, 23);
		btnSelectOutputFolder.TabIndex = 2;
		btnSelectOutputFolder.Text = "Choose folder...";
		btnSelectOutputFolder.UseVisualStyleBackColor = true;
		btnSelectOutputFolder.Click += btnSelectOutputFolder_Click;
		// 
		// txtOutputFolder
		// 
		txtOutputFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		txtOutputFolder.Location = new Point(6, 37);
		txtOutputFolder.Name = "txtOutputFolder";
		txtOutputFolder.Size = new Size(270, 23);
		txtOutputFolder.TabIndex = 1;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(6, 19);
		label1.Name = "label1";
		label1.Size = new Size(79, 15);
		label1.TabIndex = 0;
		label1.Text = "Output folder";
		// 
		// btnCancel
		// 
		btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		btnCancel.Location = new Point(338, 170);
		btnCancel.Name = "btnCancel";
		btnCancel.Size = new Size(75, 23);
		btnCancel.TabIndex = 1;
		btnCancel.Text = "Cancel";
		btnCancel.UseVisualStyleBackColor = true;
		btnCancel.Click += btnCancel_Click;
		// 
		// btnSave
		// 
		btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		btnSave.Location = new Point(257, 170);
		btnSave.Name = "btnSave";
		btnSave.Size = new Size(75, 23);
		btnSave.TabIndex = 2;
		btnSave.Text = "Save";
		btnSave.UseVisualStyleBackColor = true;
		btnSave.Click += btnSave_Click;
		// 
		// chkMapper
		// 
		chkMapper.AutoSize = true;
		chkMapper.Location = new Point(100, 66);
		chkMapper.Name = "chkMapper";
		chkMapper.Size = new Size(67, 19);
		chkMapper.TabIndex = 6;
		chkMapper.Text = "Mapper";
		chkMapper.UseVisualStyleBackColor = true;
		// 
		// chkCpuMemory
		// 
		chkCpuMemory.AutoSize = true;
		chkCpuMemory.Location = new Point(100, 116);
		chkCpuMemory.Name = "chkCpuMemory";
		chkCpuMemory.Size = new Size(96, 19);
		chkCpuMemory.TabIndex = 7;
		chkCpuMemory.Text = "Cpu Memory";
		chkCpuMemory.UseVisualStyleBackColor = true;
		// 
		// chkPpuMemory
		// 
		chkPpuMemory.AutoSize = true;
		chkPpuMemory.Location = new Point(100, 91);
		chkPpuMemory.Name = "chkPpuMemory";
		chkPpuMemory.Size = new Size(95, 19);
		chkPpuMemory.TabIndex = 8;
		chkPpuMemory.Text = "Ppu Memory";
		chkPpuMemory.UseVisualStyleBackColor = true;
		// 
		// chkCartridge
		// 
		chkCartridge.AutoSize = true;
		chkCartridge.Location = new Point(245, 66);
		chkCartridge.Name = "chkCartridge";
		chkCartridge.Size = new Size(75, 19);
		chkCartridge.TabIndex = 9;
		chkCartridge.Text = "Cartridge";
		chkCartridge.UseVisualStyleBackColor = true;
		// 
		// chkOther
		// 
		chkOther.AutoSize = true;
		chkOther.Location = new Point(245, 91);
		chkOther.Name = "chkOther";
		chkOther.Size = new Size(56, 19);
		chkOther.TabIndex = 10;
		chkOther.Text = "Other";
		chkOther.UseVisualStyleBackColor = true;
		// 
		// Logging
		// 
		AcceptButton = btnSave;
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		CancelButton = btnCancel;
		ClientSize = new Size(425, 205);
		Controls.Add(btnSave);
		Controls.Add(btnCancel);
		Controls.Add(groupBox1);
		FormBorderStyle = FormBorderStyle.FixedDialog;
		MaximizeBox = false;
		Name = "Logging";
		ShowIcon = false;
		Text = "Logging";
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		ResumeLayout(false);
	}

	#endregion

	private GroupBox groupBox1;
	private Button btnSelectOutputFolder;
	private TextBox txtOutputFolder;
	private Label label1;
	private CheckBox chkCpu;
	private CheckBox chkPpu;
	private CheckBox chkFrame;
	private Button btnCancel;
	private Button btnSave;
	private CheckBox chkPpuMemory;
	private CheckBox chkCpuMemory;
	private CheckBox chkMapper;
	private CheckBox chkOther;
	private CheckBox chkCartridge;
}