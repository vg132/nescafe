using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;

namespace Nescafe
{
	public static class StateSerializer
	{
		public static void SaveState(string id, int slot, object state)
		{
			var fileName = $"state\\{id}_{slot}.state";
			Directory.CreateDirectory("state");
			var base64State = SerializeToBase64(state);
			File.WriteAllBytes(fileName, CompressString(base64State));
		}

		public static object LoadState(string id, int slot)
		{
			var fileName = $"state\\{id}_{slot}.state";
			if (File.Exists(fileName))
			{
				var byteState = File.ReadAllBytes(fileName);
				var base64State = DecompressString(byteState);
				return DeserializeFromBase64(base64State);
			}
			return null;
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
	}
}
