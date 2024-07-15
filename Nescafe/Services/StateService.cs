using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

namespace Nescafe.Services
{
	public static class StateService
	{
		public static void SaveBatteryMemory(Console console)
		{
			var fileName = $"battery\\{console.Cartridge.Id}.bat";
			Directory.CreateDirectory("battery");
			var base64Bat = SerializeToBase64(console.Cartridge.PrgRam);
			File.WriteAllBytes(fileName, CompressString(base64Bat));
		}

		public static byte[] LoadBatteryMemory(string cartridgeId)
		{
			var fileName = $"battery\\{cartridgeId}.bat";
			if (File.Exists(fileName))
			{
				var byteState = File.ReadAllBytes(fileName);
				var base64State = DecompressString(byteState);
				return DeserializeFromBase64(base64State) as byte[];
			}
			return null;
		}

		public static void SaveState(Console console, int slot)
		{
			var state = new ConsoleState
			{
				MapperState = console.Mapper.SaveState(),
				CpuState = console.Cpu.SaveState(),
				PpuState = console.Ppu.SaveState()
			};

			var fileName = $"state\\{console.Cartridge.Id}_{slot}.state";
			Directory.CreateDirectory("state");
			var base64State = SerializeToBase64(state);
			File.WriteAllBytes(fileName, CompressString(base64State));
		}

		public static bool LoadState(Console console, int slot)
		{
			var fileName = $"state\\{console.Cartridge.Id}_{slot}.state";
			if (File.Exists(fileName))
			{
				var byteState = File.ReadAllBytes(fileName);
				var base64State = DecompressString(byteState);
				var state = DeserializeFromBase64(base64State) as ConsoleState;
				if (state != null)
				{
					console.Mapper.LoadState(state.MapperState);
					console.Cpu.LoadState(state.CpuState);
					console.Ppu.LoadState(state.PpuState);
					return true;
				}
			}
			return false;
		}

		public static string GenerateHash(byte[] inputBytes)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				var hex = new StringBuilder(hashBytes.Length * 2);
				foreach (byte b in hashBytes)
				{
					hex.AppendFormat("{0:x2}", b);
				}
				return hex.ToString();
			}
		}

		private static string SerializeToBase64(object obj)
		{
			byte[] serializedData;
			var formatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				formatter.Serialize(memoryStream, obj);
				serializedData = memoryStream.ToArray();
			}
			return Convert.ToBase64String(serializedData);
		}

		private static object DeserializeFromBase64(string base64String)
		{
			var serializedData = Convert.FromBase64String(base64String);
			var formatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream(serializedData))
			{
				return formatter.Deserialize(memoryStream);
			}
		}

		private static byte[] CompressString(string text)
		{
			var buffer = Encoding.UTF8.GetBytes(text);
			using (var memoryStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(buffer, 0, buffer.Length);
				}
				return memoryStream.ToArray();
			}
		}

		private static string DecompressString(byte[] compressedData)
		{
			using (var memoryStream = new MemoryStream(compressedData))
			{
				using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					using (var streamReader = new StreamReader(gzipStream))
					{
						return streamReader.ReadToEnd();
					}
				}
			}
		}

		[Serializable]
		class ConsoleState
		{
			public object MapperState { get; set; }
			public object CpuState { get; set; }
			public object PpuState { get; set; }
		}
	}
}
