using System.IO;
using TMPro;
using UnityEngine;

namespace Outfrost {
	
	public delegate void Callback();
	
	public static class Util {
		
		public static bool IsPrefab(GameObject gameObject) {
			return gameObject.scene.rootCount == 0;
		}
		
		public static float OrthoCameraWorldDiagonalSize(Camera camera) {
			float height = camera.orthographicSize * 2.0f;
			float width = height * camera.aspect;
			return Mathf.Sqrt(width * width + height * height);
		}
		
		public static void SetTextAlpha(TextMeshPro text, float alpha) {
			Color textColor = text.color;
			textColor.a = Mathf.Clamp01(alpha);
			text.color = textColor;
		}
		
		public static bool IsFacingCamera(GameObject gameObject, Camera camera) {
			return Vector3.Dot(gameObject.transform.forward, camera.transform.forward) > 0.0f;
		}
		
		public static bool IsFacingCamera(GameObject gameObject) {
			return IsFacingCamera(gameObject, Camera.main);
		}
		
		public static byte[] BytesFromStream(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int bytesRead;
				while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					memoryStream.Write(buffer, 0, bytesRead);
				}
				return memoryStream.ToArray();
			}
		}
		
	}
	
}