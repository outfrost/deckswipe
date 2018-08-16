using UnityEngine;

public static class Util {

	public static bool IsPrefab(GameObject gameObject) {
		return gameObject.scene.rootCount == 0;
	}
	
}
