# Prototype--Bytes-of-Past

## Screenshots
![main_menu](https://user-images.githubusercontent.com/57316283/149713447-c6c90c37-8a23-4085-82f5-7718f0cfd402.png)
![multiplayer](https://user-images.githubusercontent.com/57316283/149713459-f044c0ce-fbb0-428d-a228-80de0812866d.png)

## Development Tools

### Unity
2019.4.20f1 - Download via Unity Hub
  - <b>unityhub://2019.4.20f1/6dd1c08eedfa</b> <i>(Paste this link in browser)</i>
    - For Linux, workarounds are needed:
      - [How to install a specific Unity version](https://forum.unity.com/threads/how-to-install-a-specific-version-of-unity-on-linux.883738/#post-6827534)
      - **You cannot add existing project**, unless there is one in the list. Create a new project first, then you can add an existing project.
  - Include Android Build Support
  - Exclude Visual Studio Community if already installed

## Notes
### Canvas Scaler
- Set to `Scale with Screen Size`
- `Reference Resolution` should be `2340x1080` (based on this [latest poll](https://www.antutu.com/en/doc/124145.htm))
- Add and set the `Game Window resolution` to also `2340x1080`.
- Rect Transform: Set `Anchor Points` of objects to where it should hook. Set `Stretch` mode to how you want to stretch the object based on different screen sizes (horizontal stretch only, vertical stretch only, or both).

### Background
Background has a BG prefab that includes cloud animations.

### Scene Loader & Back Button
- Add a `Scene Loader` prefab to each scene. Then hide it in the editor to be able to see the actual scene. `Scene Loader` should be placed as the bottom-most layer to make the scene transitions visible.
- `BtnBack` for back button. Then, set the `Click` event on the button to the `SceneLoader.GoBack()` method.

## Used Assets
- [Mirror Networking](https://mirror-networking.com/)
- See .txt files in [Licenses](Assets/Resources/Licenses/) for other attributions.
