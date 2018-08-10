using UnityEngine;

public class Draggable : MonoBehaviour {
	
	private void OnMouseDrag()
	{
		Debug.Log(Input.mousePosition);
	}
	
}
