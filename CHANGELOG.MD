# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Releases]
### [0.8.1] - 2023-06-28
#### Added
* Added extension methods for `ISymbolObject`

#### Modified
* Modified .gitattributes to include LFS tracking for many different types of executable and assembly files, force all C# files to use LF line endings.

### [0.8.0] - 2023-06-28
#### Added
* Added new console commands:
	* `level-load` => Attempts to load a level by string symbol. Can only be used in-game.
	* `level-complete` => Attempts to complete the current level and proceed to the next one, or if there isn't one returns back to the lobby scene.
	* `save-clear-all` => Clears all save data from memory and deletes all save files on disk.
* Game systems can now be created in a similar way to App systems (as a scriptable object derived from `ScriptableGameSystem` or a monobehavior implementing `IGameSystem`), except these are setup and torn down when the game is started or exited from (can be multiple times per app session). Game systems must be added to the list of game systems on `GameControl`.
* Added several new UIScreen templates. These are present in the PreloadUI scene.
	* `GameOverUIScreen` => Can be used for game overs.
	* `CreditsUIScreen` => Shows credits for development on game. Can be shown when a player clicks the credits button on the `MainMenuUIScreen`.
	* `LoadingUIScreen` => A loading screen that currently has a fade in/out animation. Useful for transitions between scenes.

#### Changed
* Modified `SavesAppSystem` to include new methods for flushing save data to disk at runtime and to invoke an event `SaveDataUpdated` whenever this is done.
* Modified `SavesAppSystem` to include a hook for adding runtime-only data to save files on load.

#### Fixed
* Modified ComponentPool to enable and disable pooled items when spawned or recycled.

#### Added
* Added basic SaveAppSystem, LoadSavesUIScreen. This enables creating and loading previous save files.
* Added LevelData and ProgressionStore to make it easier to create and reference scenes.

### [0.7.0] - 2023-04-01
#### Added
* Added basic SaveAppSystem, LoadSavesUIScreen. This enables creating and loading previous save files.
* Added LevelData and ProgressionStore to make it easier to create and reference scenes.

#### Changed
* Updated MainMenuUIScreen for save logic so that:
    * When a player first starts the game they can either create a new game or go to the Settings.
    * Once a player has created a new save file, when they return to the Main Menu a new option will appear for either "Continue" to continue playing the latest updated save file and "Load Save" for selecting a specific save file to use.

#### Fixed
* Modified `game-exit` command to only attempt exiting the current game if in-game.

#### Removed
* Removed `game-enter` command as entering a game is dependent on which save file to use.

## [Releases]
### [0.6.0] - 2022-12-30
#### Added
* Implemented Graphics Settings UI to enable a player to modify their FPS target, resolution, and window style. When changed, a player can then hit a Apply Changes button to set thosen changes and either accept or reject them in a modal. If no input is provided the changes are automatically reverted after a short duration.
* Added AssemblyDefinition for ModalWindows and added as ref to Game

#### Changed
* Modified the MainMenu to hide when the settings button is clicked and when the settings menu is closed outside of the game the MainMenu will be relaunched.

#### Fixed
* Modified the scaling of the DebugUIConsole to work better in builds.
* Fixed missing references for ModalWindow prefabs.
* Fixed NRE occuring due to an invalid cast for Text in ModalWindows.

### [0.5.0] - 2022-12-14
#### Added
* Added Graphy UI package for showing performance metrics at runtime.
* Created several console commands for showing, hiding, and configuring the Graphy UI.
	* `perf-ui-toggle` => Toggles the performance metrics UI on or off.
	* `perf-ui-fps` => Configures the performance metrics UI for FPS on or off. This does not hide or show the overlay.
	* `perf-ui-audio` => Configures the performance metrics UI for Audio on or off. This does not hide or show the overlay.
	* `perf-ui-ram` => Configures the performance metrics UI for RAM on or off. This does not hide or show the overlay.
	* `perf-ui-advanced` => Configures the performance metrics UI for Advanced on or off. This does not hide or show the overlay.

### [0.4.1] - 2022-12-14
#### Fixed
* Assigned missing references for PauseAppSystem

### [0.4.0] - 2022-12-13
#### Added
* Added PauseAppSystem for handling pause state and operations. Pausing can only be done while in-game. Optionally Time.timeScale can be affected by pausing.
* Created console command `pause-toggle` to flip the current pause state of the game.

### [0.3.0] - 2022-12-13
#### Added
* Added DOTween library
* Added AudioControl audio system for handling playing and management of sound effects and music. Can handle fading and cross-fading of music.

### [0.2.1] - 2022-12-09
#### Fixed
* Fixed a bug where the commands for `game-exit` and `game-enter` were not available in the console.

### [0.2.0] - 2022-12-09
#### Added
* Added scriptable variables and game events for game state.
* Added DebugConsole to root scene and configured options.
* Implemented GameControl to handle loading and unloading the game. Uses several scriptable events and variables to communicate and maintain state.
* Added console commands to GameControl for entering and exiting game.

#### Changed
* Modified AppControl to load Lobby scene after app setup has completed.
* Modified MainMenuUIScreen to enter game when play game button is clicked.

### [0.1.0] - 2022-12-07
#### Added
* Initialized repo with .gitignore, .editorconfig, and github issue/pull request templates.
* Created empty Unity project based on 3D core template and modified project settings.
* Added AppControl, IAppSystem as lightweight app architecture.
* Added KenneyNL UIPack
* Added ModalWindows third-party content, lightly modified code and style.
* Created SettingsAppSystem, UserSettings for managing local player settings.
* Implemented basic game setup, main menu and settings UI.

## CL Template
This template should be used for any new releases to document any changes.

### [0.0.0] - 1900-01-01
#### Added
For new features.

#### Changed
For changes in existing functionality.

#### Deprecated
For soon-to-be removed features.

#### Removed
for now removed features.

#### Fixed
For any bug fixes.

#### Security
In case of vulnerabilities.