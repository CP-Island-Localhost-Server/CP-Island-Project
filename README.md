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

