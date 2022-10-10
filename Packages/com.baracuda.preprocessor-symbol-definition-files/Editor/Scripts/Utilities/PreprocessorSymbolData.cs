using System;
using UnityEngine;

namespace Baracuda.PreprocessorDefinitionFiles.Scripts.Utilities
{
    /// <summary>
    /// Class containing preprocessor symbols and additional related data.
    /// </summary>
    [Serializable]
    public sealed class PreprocessorSymbolData : ISerializationCallbackReceiver
    {
        /*
         *  Inspector Fields
         */

        [Tooltip("The Scripting Define Symbol that will be added.")]
        [SerializeField] private string symbol = string.Empty;

        [Tooltip("Enable / Disable the usage of the symbol.")]
        [SerializeField] private bool enabled = true;

        [Tooltip("Set the Build Target Group for the symbol. The symbol will only be used if the current build target and the set value match.")]
        [SerializeField] private FlagsBuildTargetGroup targetGroup = PreprocessorDefineUtilities.FlagsBuildTargetCache;

        /*
         *  Private Fields
         */

        [SerializeField] [HideInInspector] private bool isValid;

        /*
         *  Properties
         */

        internal string Symbol => symbol;
        internal bool Enabled => enabled;
        internal FlagsBuildTargetGroup TargetGroup => targetGroup;
        internal bool IsValid
        {
            get => isValid;
            set => isValid = value;
        }

        /*
         *  ISerializationCallbackReceiver Interface
         */

        public void OnBeforeSerialize()
        {
            if (!string.IsNullOrWhiteSpace(symbol))
            {
                return;
            }

            enabled = true;
            targetGroup = targetGroup == FlagsBuildTargetGroup.Unknown
                ? PreprocessorDefineUtilities.FlagsBuildTargetCache
                : targetGroup;
        }

        public void OnAfterDeserialize()
        {
        }

        /*
         *  Misc
         */

        internal void SetEnabled(bool value) => enabled = value;

#pragma warning disable CS0659
        public override bool Equals(object obj)
#pragma warning restore CS0659
            => obj is PreprocessorSymbolData other && Equals(other);



        private bool Equals(PreprocessorSymbolData other)
        {
            return symbol == other.symbol && targetGroup == other.targetGroup;
        }
    }
}