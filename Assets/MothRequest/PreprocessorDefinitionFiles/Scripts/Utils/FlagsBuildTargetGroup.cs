using System;

namespace MothRequest.PreprocessorDefinitionFiles.Utils
{
    [Flags]
    internal enum FlagsBuildTargetGroup
    {
        Unknown = 0,
        Standalone = 1,
        iOS = 2,
        Android = 4,
        WebGL = 8,
        PS4 = 16, 
        XboxOne = 32, 
        tvOS = 64,
#if ! UNITY_2019_3_OR_NEWER
        Facebook = 128,
#endif
        Switch = 256, 
        Lumin = 512, 
        Stadia = 1024,
        CloudRendering = 2048, 
        GameCoreXboxSeries = 4096, 
        GameCoreXboxOne = 8192, 
        PS5 = 16384,
    }
}