using UnityEngine;

public class Draggable : MonoBehaviour {
	
	private Vector3 snapPosition;
	private Vector3 dragStartPosition;
	private Vector3 dragStartPointerPosition;
	
	public void SetSnapPosition(Vector3 pos) {
		snapPosition = pos;
	}
	
	private void Start() {
		SetSnapPosition(transform.position);
	}
	
	private void OnMouseDown() {
		dragStartPosition = transform.position;
		dragStartPointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	private void OnMouseDrag() {
		Vector3 positionDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragStartPointerPosition;
		positionDelta.z = dragStartPosition.z;
		transform.position = dragStartPosition + positionDelta;
	}
	
	private void OnMouseUp() {
		transform.position = snapPosition;
	}
	
}
