using System;
using UnityEditor;

namespace MothRequest.PreprocessorDefinitionFiles.Utils
{
    internal static class BuildTargetGroupExtensions
    {
        internal static FlagsBuildTargetGroup AsSingleFlags(this BuildTargetGroup current)
        {
            foreach (FlagsBuildTargetGroup value in Enum.GetValues(typeof(FlagsBuildTargetGroup)))
            {
                if (current.ToString().Equals(value.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return value;
                }
            }
            
            return FlagsBuildTargetGroup.Unknown;
        }
    }
}