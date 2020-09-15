using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Outfrost {

	public static class JsonResources {

		public static List<T> Load<T>(string path) {
			var list = new List<T>();

			var assets = Resources.LoadAll<TextAsset>(path);
			foreach (var asset in assets) {
				try {
					T obj = JsonUtility.FromJson<T>(asset.text);
					list.Add(obj);
				}
				catch (ArgumentException e) {
					UnityEngine.Debug.LogError(
							"[JsonResources] Cannot deserialize "
							+ typeof(T).Name
							+ " from "
							+ asset.name
							+ ": "
							+ e.Message);
				}
			}

			return list;
		}

	}

}
