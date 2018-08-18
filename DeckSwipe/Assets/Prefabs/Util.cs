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
	
}
