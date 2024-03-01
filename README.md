# DeckSwipe

This is a skeleton for a simple card game. There are 4 gameplay resources (predefined as Coal, Food, Health, Hope), each contributing to the chances of survival for the player's city. Choices that the player makes through swiping each card left or right will influence those resources in various ways. If any one of the resources depletes (reaches zero), the game is lost and reset. The player's objective is to make decisions such that depletion doesn't happen, and the city survives, for as long as they can manage.

The core mechanics and visuals are heavily based on Reigns, and its clone, Lapse: The Forgotten Future. The sample content is mostly inspired by Frostpunk and its neverending winter.

Created with Unity on Linux, primarily targetting Android.

![Screen capture of the game running on Android](screencap-android.gif)

## Contributing

This project is not actively maintained. Some PRs may end up merged, but I do not have a Unity installation for testing. If you wish to make use of the repo, I recommend forking and adapting it to your needs.

Changes accepted in PRs will be released under the license terms provided in [LICENSE](./LICENSE).

## License

All content published in this repository, be it software, in source code or binary form, or other works, is released under the MIT License, as documented in [LICENSE](./LICENSE), with the following exceptions:

* **TextMesh Pro**

	Bundled with Unity and distributed on Unity license terms

	https://unity3d.com/legal

	Files:

	* `DeckSwipe/Assets/Dependencies/TextMesh Pro/*`

