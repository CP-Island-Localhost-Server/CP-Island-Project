## Requirements to Open the Project

### Unity Version
- **Unity 2022.3.57f1 LTS**

### Notes
- When opening the project, Unity might ask if you want to use the new input system. **Press NO.**
- **The Penguin data gets stored here in the Windows Registry:**  
  - **Built .exe client:** `HKEY_CURRENT_USER\SOFTWARE\OpenCPI\CP Island`  
  - **Unity Editor:** `HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\OpenCPI\CP Island`
- To launch the game in the Unity editor:
  - Open `Assets/Game/Core/Scenes/Boot.unity`
  - Hit the Play button.

### Future Unity Engine Versions Notes
- **Bake lightmaps in Unity 2022 ONLY** because of Enlighten. This keeps them closer to the original.
- Lightmaps in Unity 2023+ (with Progressive CPU/GPU) look weird—too bright or noisy.
- The Unity 6 branch is good for regular gameplay. Pre-rendered lightmaps in the `EventLightmaps` folder work fine in Unity 6.
- **DO NOT update to Unity 7 or newer**. They drop legacy input system support, and we'd have to recode everything.

### New and fixed features within the game
- What's new:
    - Discord RPC support. Here is an example of the Discord RPC:
      
      ![image](https://github.com/user-attachments/assets/ea786664-c5fa-47e9-8d23-5776ab85b840)

    - Added the unreleased igloo furniture ```grand father clock``` from version 1.6.1
    - Added 3 custom party hat recolors (Halloween Party Hat, Holiday Party Hat, and Anniversary 19 Party Hat)
    - Added 4 custom duck tube recolors (Blue, Green, Pink, and Purple)
    - Refined the lightmap baking process
    - The Classic Arcade machine has been moved from near Franky's Pizza in Island Central to the sewer in Island Central

- What has been fixed:
    - The spawn points have been moved so you will no longer spawn into the void and endlessly fall randomly like in the original
    - The ```shoulder pack``` blueprint will correctly work now
    - The ```modern coffee table``` and ```kitchen island ``` igloo furnitures can now be obtained instead of having it give you the ```teleporter``` igloo furniture
    - Fixed missing scripts from the Mt. Blizzard Halloween 2018 decorations (this was causing errors in the original client)
    - Fixed the Halloween 2018 Pumpkins flicker speed to match how they wanted it in the original (the editor and built client shows 2 different results. So the built client would make it too fast)
    - Added missing colliders to certain world and quest objects
    - Performance improvements
    - Added the missing Summer Splashdown chat phases and Rookie sound effects to the Regular sewer in Island Central
    - Fixed original errors within the ```Unlit Dynamic Object No FOG``` and Igloo ```CubeMap``` shaders
    - Fixed the Disney Store banners and for sale items, they originally stopped working on: ```January 1, 2020```. Now they will stop working on: ```December 31, 4065```
      
## System Requirements

### Windows
- Latest version of Visual Studio Community.
- [Git for Windows](https://git-scm.com/downloads/win) installed. **Restart your PC after installing Git.**

### macOS
- Latest version of Xcode for your macOS.

### Linux
- Make sure your system is updated:
  ```bash
  sudo apt update && sudo apt upgrade
  ```
- Install Git:
  ```bash
  sudo apt install git
  ```
- For other distros, check the [official Git installation guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

