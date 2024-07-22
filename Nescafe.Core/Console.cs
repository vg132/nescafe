using Nescafe.Core.Mappers;
using System.Diagnostics;

namespace Nescafe.Core
{
	/// <summary>
	/// Represents a NES console.
	/// </summary>
	public class Console
	{
		/// <summary>
		/// This Console's CPU.
		/// </summary>
		public readonly Cpu Cpu;

		/// <summary>
		/// This Console's PPU
		/// </summary>
		public readonly Ppu Ppu;

		/// <summary>
		/// This Console's CPU Memory.
		/// </summary>
		public readonly CpuMemory CpuMemory;

		/// <summary>
		/// This Console's PPU Memory.
		/// </summary>
		public readonly PpuMemory PpuMemory;

		/// <summary>
		/// This Console's Controller
		/// </summary>
		/// <remarks>
		/// This is currently set up to only work as controller 1.
		/// </remarks>
		public readonly Controller Controller;

		/// <summary>
		/// Gets or sets the console's Cartridge.
		/// </summary>
		/// <value>The Cartridge currently loaded in this console</value>
		public Cartridge Cartridge { get; private set; }

		/// <summary>
		/// Gets or sets the mapper for the cartridge currently loaded in this console.
		/// </summary>
		/// <value>The mapper for the cartridge currently loaded in this console.</value>
		public Mapper Mapper { get; private set; }

		/// <summary>
		/// Gets or sets the Action called when the Console is ready to draw a frame.
		/// </summary>
		/// <value>The Action called when the Console is ready to draw a frame.</value>
		public Action<byte[]> DrawAction { get; set; }

		/// <summary>
		/// Event that is triggered when the console starts running a game
		/// </summary>
		public event Action<Console> OnRunning;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:Nescafe.Console"/> should stop.
		/// </summary>
		/// <value><c>true</c> if the console has been stopped; otherwise, <c>false</c>.</value>
		private bool _stop { get; set; }
		public bool IsRunning { get; set; }

		public bool Pause { get; set; }
		public readonly object CpuCycleLock = new object();

		// Used internally to determine if we've reached a new frame
		bool _frameEvenOdd;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nescafe.Console"/> class.
		/// </summary>
		public Console()
		{
			Controller = new Controller();

			CpuMemory = new CpuMemory(this);
			PpuMemory = new PpuMemory(this);

			Cpu = new Cpu(this);
			Ppu = new Ppu(this);
		}

		public void Reset()
		{
			lock (CpuCycleLock)
			{
				CpuMemory.Reset();
				PpuMemory.Reset();

				Cpu.Reset();
				Ppu.Reset();

				_frameEvenOdd = false;
			}
		}

		/// <summary>
		/// Loads a cartridge into the console.
		/// </summary>
		/// <remarks>
		/// Logs information about the cartridge to stdout while loading including
		/// any errors that would cause the method to return <c>false</c>.
		/// </remarks>
		/// <returns><c>true</c>, if cartridge was loaded successfully, <c>false</c> otherwise.</returns>
		/// <param name="path">Path to the iNES cartridge file to load</param>
		public bool LoadCartridge(string path)
		{
			System.Console.WriteLine("Loading ROM " + path);
			if (Cartridge != null)
			{
				Cartridge.Eject();
			}
			Cartridge = new Cartridge(path);
			if (Cartridge.Invalid)
			{
				return false;
			}

			// Set mapper
			System.Console.Write("iNES Mapper Number: " + Cartridge.MapperNumber.ToString());
			switch (Cartridge.MapperNumber)
			{
				case 0:
					System.Console.WriteLine(" (NROM) Supported!");
					Mapper = new NromMapper(this);
					break;
				case 1:
					System.Console.WriteLine(" (MMC1) Supported!");
					Mapper = new Mmc1Mapper(this);
					break;
				case 2:
					System.Console.WriteLine(" (UxROM) Supported!");
					Mapper = new UxRomMapper(this);
					break;
				case 3:
					System.Console.WriteLine(" (CNROM) Supported!");
					Mapper = new CnRomMapper(this);
					break;
				case 4:
					System.Console.WriteLine(" (MMC3) Supported!");
					Mapper = new Mmc3Mapper(this);
					break;
				case 68:
					System.Console.WriteLine(" (MMC6) Supported!");
					Mapper = new Mmc6Mapper(this);
					break;
				case 172:
					System.Console.WriteLine(" Supported!");
					Mapper = new Mapper172(this);
				default:
					System.Console.WriteLine(" mapper is not supported");
					return false;
			}

			Reset();
			return true;
		}

		/// <summary>
		/// Forces the console to call <see cref="T:Nescafe.Console.DrawAction"/>
		/// with current data from the PPU.
		/// </summary>
		public void DrawFrame()
		{
			if(DrawAction!=null)
			{
				DrawAction(Ppu.BitmapData);
			}
			_frameEvenOdd = !_frameEvenOdd;
		}

		void GoUntilFrame()
		{
			var orig = _frameEvenOdd;
			while (orig == _frameEvenOdd)
			{
				lock (CpuCycleLock)
				{
					var cpuCycles = Cpu.Step();

					// 3 PPU cycles for each CPU cycle
					for (var i = 0; i < cpuCycles * 3; i++)
					{
						Ppu.Step();
						Mapper.Step();
					}
				}
			}
		}

		public void Stop()
		{
			_stop = true;
			while (IsRunning)
			{
				PreciseSleep.Sleep(5);
			}
		}

		/// <summary>
		/// Starts running the console and drawing frames.
		/// </summary>
		public void Start()
		{
			_stop = false;
			IsRunning = true;
			OnRunning?.Invoke(this);
			var s = new Stopwatch();
			while (!_stop)
			{
				var frameRate = 60;//AppSettings.Instance.CpuSpeed;
				s.Restart();
				for (var i = 0; i < frameRate; i++)
				{
					var frameWatch = Stopwatch.StartNew();
					if(!Pause)
					{
						GoUntilFrame();
					}
					frameWatch.Stop();
					PreciseSleep.Sleep((int)((1000.0 / frameRate) - frameWatch.ElapsedMilliseconds));
				}
				s.Stop();
				Debug.WriteLine($"{frameRate} frames in {s.ElapsedMilliseconds}ms");
			}
			IsRunning = false;
			Debug.WriteLine("Console Stopped");
		}
	}
}