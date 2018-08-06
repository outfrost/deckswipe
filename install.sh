#!/bin/sh

for f in DeckSwipe/Build/android/*.apk
do
	~/Android/Sdk/platform-tools/adb -d install -r "$f"
done
