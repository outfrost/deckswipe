using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.World {
	
	public class InputDispatcher : MonoBehaviour {
		
		public delegate void KeyDownHandler(KeyCode keyCode);
		
		private readonly Dictionary<KeyCode, LinkedList<KeyDownHandler>> keyDownHandlers = new Dictionary<KeyCode, LinkedList<KeyDownHandler>>();
		
		private void Update() {
			// Scan registered key inputs and invoke handlers
			foreach (var entry in keyDownHandlers) {
				if (Input.GetKeyDown(entry.Key)) {
					foreach (KeyDownHandler handler in entry.Value) {
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
		
	}
	
}
