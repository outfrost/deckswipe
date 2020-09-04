using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.World {
	
	public class InputDispatcher : MonoBehaviour {
		
		public delegate void KeyDownHandler(KeyCode keyCode);
		
		public delegate void KeyUpHandler(KeyCode keyCode);
		
		private readonly Dictionary<KeyCode, LinkedList<KeyDownHandler>> keyDownHandlers = new Dictionary<KeyCode, LinkedList<KeyDownHandler>>();
		
		private readonly Dictionary<KeyCode, LinkedList<KeyUpHandler>> keyUpHandlers = new Dictionary<KeyCode, LinkedList<KeyUpHandler>>();
		
		private void Update() {
			// Scan registered key inputs and invoke handlers
			foreach (var entry in keyDownHandlers) {
				if (Input.GetKeyDown(entry.Key)) {
					foreach (KeyDownHandler handler in entry.Value) {
						handler(entry.Key);
					}
				}
			}
			foreach (var entry in keyUpHandlers) {
				if (Input.GetKeyUp(entry.Key)) {
					foreach (KeyUpHandler handler in entry.Value) {
						handler(entry.Key);
					}
				}
			}
			
		}
		
		public void AddKeyDownHandler(KeyCode keyCode, KeyDownHandler handler) {
			LinkedList<KeyDownHandler> list;
			try {
				list = new LinkedList<KeyDownHandler>();
				keyDownHandlers.Add(keyCode, list);
			}
			catch (ArgumentException) {
				list = keyDownHandlers[keyCode];
			}
			
			list.AddLast(handler);
		}
		
		public void AddKeyUpHandler(KeyCode keyCode, KeyUpHandler handler) {
			LinkedList<KeyUpHandler> list;
			try {
				list = new LinkedList<KeyUpHandler>();
				keyUpHandlers.Add(keyCode, list);
			}
			catch (ArgumentException) {
				list = keyUpHandlers[keyCode];
			}
			
			list.AddLast(handler);
		}

	}
	
}
