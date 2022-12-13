# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Releases]
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