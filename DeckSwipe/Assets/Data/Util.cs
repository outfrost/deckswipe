using TMPro;
using UnityEngine;

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
	
}
