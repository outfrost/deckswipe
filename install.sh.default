#!/bin/sh

# Installs an APK from a Unity project on an Android device via ADB over USB

# Don't edit environment-specific values in install.sh.default; instead,
# make a copy named install.sh and edit there.

# Unity project name
PROJ_NAME="DeckSwipe"

# Set this to the location of the adb binary
# provided in the Android SDK that you're using in Unity
ADB="/home/outfrost/Android/Sdk/platform-tools/adb"

IFS="$(printf ',\t\n')"

if abilist=$($ADB -d shell getprop ro.product.cpu.abilist) \
		&& { \
			[ -n "$abilist" ] \
			|| abilist=$($ADB -d shell getprop ro.product.cpu.abi) \
			&& [ -n "$abilist" ] ;\
		}; then
	matched=
	for arch in $abilist; do
		if [ -z "$matched" ]; then
			package="$PROJ_NAME/Build/android/$PROJ_NAME.$arch.apk"
			echo -n "Looking for $package ... "
			if [ -f "$package" ]; then
				echo "Got it"
				echo -n "Installing over USB ... "
				$ADB -d install -r "$package"
				matched=true
			else
				echo "Not found"
			fi
		fi
	done
	
	if [ -z "$matched" ]; then
		echo "Could not find a package to match any of the architectures" >&2
		echo "supported by the device" >&2
	fi
else
	echo "Unable to get supported architectures from device" >&2
fi
