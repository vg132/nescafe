using Microsoft.Extensions.Configuration;
using Nescafe.UI;
using System.Diagnostics;

namespace Nescafe;

public class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		Application.Run(new Launcher());
	}
}
