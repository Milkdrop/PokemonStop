# STAYHOME Client-Side
## Application (Server) repository: https://github.com/iRaduS/hackthevirus
This is the repository for the Android client game (Could easily be ported to iOS if one ports the location daemon (which is the PokeService sub-project).

## What does every folder do?
The Repository represents a Unity project, that includes an already-built Build.apk file for anyone to install. The Build.apk file should be compatible with API Level 21+, although the location daemon has been thoroughly tested API 26+ (Oreo). The location daemon should still work for smaller API levels, but no status notification will be shown.

Under /Assets, you will find all assets and unity scripts used for the app. Under /PokeService you have the Android Studio project for the location daemon, the source being at /PokeService/app/src/main/java/com/hashtagh/pokeservice.

The Unity project represents the game that runs on the phone, and the location daemon (the /PokeService project) is a background service automatically included in the Unity-built game, that pings the servers with your location for XP calculation.

## Requires

- Unity 2019.1.14f1
- Android Studio 3.1.4

## How to use the application
Simply install the Build.apk file on your Android device.
