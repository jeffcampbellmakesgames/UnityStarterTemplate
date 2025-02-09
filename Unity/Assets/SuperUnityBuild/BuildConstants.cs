using System;

// This file is auto-generated. Do not modify or move this file.

namespace SuperUnityBuild.Generated
{
    public enum ReleaseType
    {
        None,
        Development,
    }

    public enum Platform
    {
        None,
        macOS,
        PC,
    }

    public enum ScriptingBackend
    {
        None,
        IL2CPP,
        Mono,
    }

    public enum Architecture
    {
        None,
        macOS,
        Windows_x64,
    }

    public enum Distribution
    {
        None,
    }

    public static class BuildConstants
    {
        public static readonly DateTime buildDate = new DateTime(638395227265281760);
        public const string version = "1.0.0.2";
        public const ReleaseType releaseType = ReleaseType.Development;
        public const Platform platform = Platform.macOS;
        public const ScriptingBackend scriptingBackend = ScriptingBackend.IL2CPP;
        public const Architecture architecture = Architecture.macOS;
        public const Distribution distribution = Distribution.None;
    }
}

