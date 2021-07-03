# Holoware
***Holoware / メイドインホロ*** is a WarioWare-inspired *hololive* fangame featuring its various virtual YouTuber talents. 

This project was developed in **Unity 2019.4.11f1** using C#.

### Creating microgames

The following scripts are an integral part of developing microgames:
- **[Microgame.cs](Assets/Scripts/System/Microgame.cs)**
- **[BGMManager.cs](Assets/Scripts/System/BGMManager.cs)**
- **[SFXManager.cs](Assets/Scripts/System/SFXManager.cs)**
- **[CharacterAvatar.cs](Assets/Scripts/UI/CharacterAvatar.cs)**

Comments are provided in those scripts to help with the process. Please study the existing microgames to get a sense of their structure. I recommend starting off by duplicating an existing microgame.

Scripts in the [Assets/Scripts/Utilities](Assets/Scripts/Utilities) folder may make the process easier, but they are not necessary.

All assets exclusive to one microgame should be stored in a subfolder of [Assets/Microgames](Assets/Microgames). Microgame content is stored in a prefab with the microgame main script attached.

The main script for a microgame inherits Microgame.cs. See [MicroPlaceholder.cs](Assets/Microgames/Placeholder/MicroPlaceholder.cs) and [MicroClimb.cs](Assets/Microgames/Climb/MicroClimb.cs) for basic examples of microgame main scripts.

To test a microgame, replace the Microgame and/or Hard Microgame fields in **Assets/Microgames/MicrogamePoolTest.asset**, then run the **Testing** scene (Assets/Scenes/Testing.unity).

Some things to be aware of:
- All scripts exclusive to a microgame should be organized under the namespace **Micro.<microgame name>**.
- Instantiated game objects must be children of the microgame transform, such that they are removed once the microgame is destroyed.

## License

Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International: https://creativecommons.org/licenses/by-nc-sa/4.0/

Adhere to hololive Derivative Works Guidelines: https://en.hololive.tv/terms

© 2020 tian nya
