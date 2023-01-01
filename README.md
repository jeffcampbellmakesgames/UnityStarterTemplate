![GitHub issues](https://img.shields.io/github/issues/jeffcampbellmakesgames/UnityStarterTemplate)
[![Twitter Follow](https://img.shields.io/badge/twitter-%40stampyturtle-blue.svg?style=flat&label=Follow)](https://twitter.com/stampyturtle)

# Unity Starter Template
## About
Unity Starter Template is a template project for making a game in Unity. It comes with a set of helpful packages pre-installed along with a lightweight architecture and UI configuration to allow you to immediately jump in and start on your game.

### Minimum Requirements
* **Unity Version:** 2021.3.14f1 or Higher
* **Scripting Runtime**: .Net 4.X
* **GIT LFS**

### Third-party packages and plugins
* [`JCMG Utility`](https://github.com/jeffcampbellmakesgames/jcmg-utility) => A general-purpose utility library for editor/runtime.
* [`JCMG Slate`](https://github.com/jeffcampbellmakesgames/jcmg-slate) => A lightweight UI framework library for Unity
* [`Markdown Viewer`](https://github.com/jeffcampbellmakesgames/UnityMarkdownViewer) => An editor enhancement library that adds inspector support to markdown (.md) files.
* [`ScriptableObject-Architecture`](https://github.com/DanielEverland/ScriptableObject-Architecture) => A scriptable architecture support library.
* [`NaughtyAttributes`](https://github.com/dbrizov/NaughtyAttributes) => An editor enhancement library aimed for inspector enhancements.
* [`SuperUnityBuild`](https://github.com/superunitybuild/buildtool) => A powerful automation tool for quickly and easily generating builds with Unity.
* [`Yasirkula DebugConsole`](https://github.com/yasirkula/UnityIngameDebugConsole) => An in-game debug console for viewing console logs and executing commands at runtime.
* [`DOTween`](http://dotween.demigiant.com/) => DOTween is a fast, efficient, fully type-safe object-oriented animation engine for Unity, optimized for C# users, free and open-source, with tons of advanced features
* [`Graphy`](https://github.com/Tayx94/graphy) => Graphy is the ultimate, easy to use, feature packed FPS counter, stats monitor and debugger for your Unity project.

## How to Use
1. On the top right of this github page, select the *Use this template* button. This will enable you to create your own version of this repository (can be made private or public).

2. One area Github's Template feature doesn't handle well is LFS which this repository has handled. To fix any broken LFS links, clone the repository locally.

3. Add a remote repository to the original template (https://github.com/jeffcampbellmakesgames/UnityStarterTemplate.git). In this example I'm giving it the name `template_origin`.

<img width="473" alt="image" src="https://user-images.githubusercontent.com/1663648/210172973-f60fd1b7-f84c-489a-9ef1-58e1c4558914.png">

4. Fetch and pull all lfs objects from the template repository and then push them up to your own origin. This can be done with the following commands.

```
git lfs fetch template_origin
git lfs pull template_origin
git lfs push origin --all
```

## Support
If this is useful to you and/or you’d like to see future development and more tools in the future, please consider supporting it by contributing to the Github project (i.e, submitting bug reports or features and/or creating pull requests) or by buying me coffee using any of the links below. Every little bit helps!

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I3I2W7GX)

## Contributing

For information on how to contribute and code style guidelines, please visit [here](CONTRIBUTING.md).
