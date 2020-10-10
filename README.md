# Holoware
***Holoware / メイドインホロ*** is a WarioWare-inspired *hololive* fangame featuring its various virtual YouTuber talents. 

## Contributing

*Holoware* development is discussed in the **[Hololive Creators Club](https://discord.gg/xJd9Der)** Discord server.

This project is being developed in **Unity 2019.4.11f1** using C#.

[Task list](https://docs.google.com/document/d/1RAYOofqorfN2YzauiiTMXFqHyFn2YotmVLP6PCBGmdE/edit?usp=sharing)

### Adding microgames

Create a branch to work in for your new microgame.

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
- Please do not modify existing assets, scripts and scenes other than Testing. Changes to Testing should not be merged into main.
- You may use existing assets such as image files, sound files, materials, etc. For characters, use the character sprites provided in [Assets/Sprites/Characters](Assets/Sprites/Characters). You will need to create other assets yourself.
- Do not worry about the microgame name displaying in the game as it involves the localization files.
- Keep your code readable and easily modifiable or I'll eat you alive.

Submit a pull request after completing your microgame and I'll handle the integration on my end.

## License

No license. Please do not redistribute content to non-contributors without permission from the repository owner. Exceptions apply to public domain/licensed assets such as plugins and some audio files.

© 2020 Tianyao Liu
