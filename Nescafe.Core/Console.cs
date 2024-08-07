using Nescafe.Core.Mappers;
using Nescafe.Services;
using System.Diagnostics;
using System.Reflection;

namespace Nescafe.Core
{
	/// <summary>
	/// Represents a NES console.
	/// </summary>
	public class Console
	{
		private bool _useVGPpu = true;

		/// <summary>
		/// This Console's CPU.
		/// </summary>
		public readonly Cpu Cpu;

		/// <summary>
		/// This Console's PPU
		/// </summary>
		public readonly IPpu Ppu;

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
		public readonly object FrameLock = new object();

		// Used internally to determine if we've reached a new frame
		private bool _frameEvenOdd;
		private long _frameCount;

		public int CurrentFPS { get; private set; }

		private IDictionary<int, Type> _mappers;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Nescafe.Console"/> class.
		/// </summary>
		public Console()
		{
			Controller = new Controller();

			CpuMemory = new CpuMemory(this);
			PpuMemory = new PpuMemory(this);

			Cpu = new Cpu(this);
			Ppu = _useVGPpu ? new VGPpu(this) : new Ppu(this);

			InitializeMappers();
		}

		private void InitializeMappers()
		{
			_mappers = new Dictionary<int, Type>();
			var types = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(item => !item.IsAbstract && item.IsSubclassOf(typeof(Mapper)));
			foreach (var type in types)
			{
				var mapper = type.GetCustomAttribute<MapperAttribute>();
				if (mapper != null && !_mappers.ContainsKey(mapper.Id))
				{
					_mappers.Add(mapper.Id, type);
				}
			}
		}

		public void Reset()
		{
			lock (FrameLock)
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
			LoggingService.LogEvent(NESEvents.Cartridge, $"Loading ROM {path}");
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
			if (_mappers.ContainsKey(Cartridge.MapperNumber))
			{
				LoggingService.LogEvent(NESEvents.Cartridge, $"iNES Mapper {Cartridge.MapperNumber} supported!");
				Mapper = (Mapper)Activator.CreateInstance(_mappers[Cartridge.MapperNumber], this);
			}
			else
			{
				LoggingService.LogEvent(NESEvents.Cartridge, $"iNES Mapper {Cartridge.MapperNumber} not supported");
#if DEBUG
				throw new MapperNotSupportedException(Cartridge.MapperNumber);
#else
					return false;
#endif
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
			if (DrawAction != null)
			{
				DrawAction(Ppu.BitmapData);
			}
			_frameEvenOdd = !_frameEvenOdd;
		}

		private void GoUntilFrame()
		{
			if (_useVGPpu)
			{
				Ppu.Step();
			}
			else
			{
				var orig = _frameEvenOdd;
				var cpuSyncCounter = 0;
				while (orig == _frameEvenOdd)
				{
					lock (FrameLock)
					{
						Ppu.Step();
						Mapper.Step();
						if (++cpuSyncCounter == 3)
						{
							Cpu.Step();
							cpuSyncCounter = 0;
						}
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
			try
			{
				_frameCount = 0;
				_stop = false;
				IsRunning = true;
				OnRunning?.Invoke(this);
				var s = new Stopwatch();
				while (!_stop)
				{
					long avarageFrameTime = 0;
					var frameRate = AppSettings.Instance.CpuSpeed;
					s.Restart();
					for (var i = 0; i < frameRate; i++)
					{
						var frameWatch = Stopwatch.StartNew();
						if (!Pause)
						{
							GoUntilFrame();
						}
						frameWatch.Stop();
						avarageFrameTime += frameWatch.ElapsedTicks;
						PreciseSleep.Sleep((int)((1000.0 / frameRate) - frameWatch.ElapsedMilliseconds));
					}
					s.Stop();
					CurrentFPS = (int)((frameRate * 1000) / s.ElapsedMilliseconds);
					LoggingService.LogEvent(NESEvents.Frame, $"{frameRate} frames in {s.ElapsedMilliseconds}ms, avarage frame time: {new TimeSpan(avarageFrameTime / 60)}");
				}
				IsRunning = false;
				LoggingService.LogEvent(NESEvents.Other, "Console Stopped");
			}
			catch (Exception ex)
			{
				LoggingService.LogEvent(NESEvents.Other, ex.Message);
#if DEBUG
				throw new Exception("Error when running game", ex);
#endif
			}
		}
	}
}