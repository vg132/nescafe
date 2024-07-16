using System;
using System.Windows.Forms;

namespace Nescafe.Core
{
	class Emulator
	{
		[STAThread]
		public static void Main()
		{
			Application.Run(new Ui());
		}
	}
}
