using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Outfrost {

	public class JsonFile<T>
			where T: new() {

		private readonly string path;
		private SemaphoreSlim fileAccess = new SemaphoreSlim(1, 1);

		public JsonFile(string path) => this.path = path;

		public async Task Create(bool prettyPrint = false) {
			await Write(JsonUtility.ToJson(new T(), prettyPrint));
		}

		public async Task Serialize(T obj, bool prettyPrint = false) {
			await Write(JsonUtility.ToJson(obj, prettyPrint));
		}

		public async Task<T> Deserialize(bool create = false) {
			if (!File.Exists(path)) {
				if (create) {
					await Create();
				}
				else {
					return default(T);
				}
			}

			string json;
			await fileAccess.WaitAsync();
			using (FileStream fileStream = File.OpenRead(path)) {
				StreamReader reader = new StreamReader(fileStream);
				json = await reader.ReadToEndAsync();
			}
			fileAccess.Release();

			T obj;
			try {
				obj = JsonUtility.FromJson<T>(json);
			}
			catch (ArgumentException e) {
				UnityEngine.Debug.LogError("[JsonFile] Cannot deserialize from " + path + ": " + e.Message);
				obj = default(T);
			}

			return obj;
		}

		private async Task Write(string json) {
			await fileAccess.WaitAsync();
			using (FileStream fileStream = File.Create(path)) {
				StreamWriter writer = new StreamWriter(fileStream);
				await writer.WriteAsync(json);
				await writer.WriteAsync('\n');
				await writer.FlushAsync();
			}
			fileAccess.Release();
		}

	}

}
