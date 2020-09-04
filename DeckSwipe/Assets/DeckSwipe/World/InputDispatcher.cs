using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.World {

	using KeyHandler = Action<KeyCode>;
	using KeyHandlerMap = Dictionary<KeyCode, LinkedList<Action<KeyCode>>>;

	public class InputDispatcher : MonoBehaviour {

		private readonly KeyHandlerMap keyDownHandlers = new KeyHandlerMap();
		private readonly KeyHandlerMap keyUpHandlers = new KeyHandlerMap();

		private void Update() {
			// Scan registered key inputs and invoke handlers
			foreach (var entry in keyDownHandlers) {
				if (Input.GetKeyDown(entry.Key)) {
					foreach (KeyHandler handler in entry.Value) {
						handler(entry.Key);
					}
				}
			}
			foreach (var entry in keyUpHandlers) {
				if (Input.GetKeyUp(entry.Key)) {
					foreach (KeyHandler handler in entry.Value) {
						handler(entry.Key);
					}
				}
			}
		}

		public void AddKeyDownHandler(KeyCode keyCode, KeyHandler handler) {
			LinkedList<KeyHandler> list;
			try {
				list = new LinkedList<KeyHandler>();
				keyDownHandlers.Add(keyCode, list);
			}
			catch (ArgumentException) {
				list = keyDownHandlers[keyCode];
			}

			list.AddLast(handler);
		}

		public void AddKeyUpHandler(KeyCode keyCode, KeyHandler handler) {
			LinkedList<KeyHandler> list;
			try {
				list = new LinkedList<KeyHandler>();
				keyUpHandlers.Add(keyCode, list);
			}
			catch (ArgumentException) {
				list = keyUpHandlers[keyCode];
			}

			list.AddLast(handler);
		}

	}

}
