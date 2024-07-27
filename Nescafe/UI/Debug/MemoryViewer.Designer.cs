using RPHexEditor;
using System.ComponentModel.Design;

namespace Nescafe.UI.Debug;

partial class MemoryViewer
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
		rpHexEditoruc1 = new RPHexEditorUC();
		SuspendLayout();
		// 
		// rpHexEditoruc1
		// 
		rpHexEditoruc1.BorderStyle = BorderStyle.Fixed3D;
		rpHexEditoruc1.Location = new Point(12, 12);
		rpHexEditoruc1.Name = "rpHexEditoruc1";
		rpHexEditoruc1.Size = new Size(776, 426);
		rpHexEditoruc1.TabIndex = 0;
		// 
		// MemoryViewer
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(rpHexEditoruc1);
		Name = "MemoryViewer";
		Text = "MemoryViewer";
		ResumeLayout(false);
	}

	#endregion

	private RPHexEditorUC rpHexEditoruc1;
}