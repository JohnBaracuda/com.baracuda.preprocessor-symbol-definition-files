using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

// ReSharper disable NonReadonlyMemberInGetHashCode

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
    
    
    /// <summary>
    /// Class containing preprocessor symbols and additional related data.
    /// </summary>
    [Serializable]
    internal sealed class PreprocessorSymbolData : ISerializationCallbackReceiver
    {
        #region --- [SERIALIZED] ---
        
        [Tooltip("The Scripting Define Symbol that will be added.")]
        [SerializeField] private string symbol = string.Empty;
        [Tooltip("Enable / Disable the usage of the symbol.")]
        [SerializeField] private bool enabled = true;
        [Tooltip("Set the Build Target Group for the symbol. The symbol will only be used if the current build target and the set value match.")]
        [SerializeField] private FlagsBuildTargetGroup targetGroup = PreprocessorDefineUtilities.FlagsBuildTargetCache;
        
        [SerializeField] [HideInInspector] private bool isValid;
        
        #endregion
        
        //--------------------------

        #region --- [PROPERTIES] ---
        
        internal string Symbol => symbol;
        internal bool Enabled => enabled;
        internal FlagsBuildTargetGroup TargetGroup => targetGroup;
        internal bool IsValid
        {
            get => isValid;
            set => isValid = value;
        }

        #endregion

        //--------------------------

        #region --- [SETTER] ---

        internal void SetEnabled(bool value) => enabled = value;

        #endregion
        
        //--------------------------
        
        #region --- [PROPERTY DRAWER HELPER] ---
        
        public const string NAMEOF_SYMBOL = nameof(symbol);
        public const string NAMEOF_ENABLED = nameof(enabled);
        public const string NAMEOF_TARGET = nameof(targetGroup);
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region --- [INTERFACE] ---

        public void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                enabled = true;
                targetGroup = PreprocessorDefineUtilities.FlagsBuildTargetCache;
            }
        }

        public void OnAfterDeserialize()
        {
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region --- [EQUALS] ---

        public override bool Equals(object obj)
            => obj is PreprocessorSymbolData other && Equals(other);

        private bool Equals(PreprocessorSymbolData other)
        {
            return symbol == other.symbol && targetGroup == other.targetGroup;
        }

        
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (symbol != null ? symbol.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ enabled.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) targetGroup;
                hashCode = (hashCode * 397) ^ isValid.GetHashCode();
                return hashCode;
            }
        }

        #endregion
        
    }
}