using UnityEngine;

namespace MothRequest.PreprocessorDefinitionFiles.Example
{
    public class ExampleBehaviour : MonoBehaviour
    {
#pragma warning disable
        [Tooltip("Text will be displayed if preprocessor define 'EXAMPLE_A' is enabled")]
        [SerializeField] private string exampleMessageA = "Example A";
        [Tooltip("Text will be displayed if preprocessor define 'EXAMPLE_B' is enabled")]
        [SerializeField] private string exampleMessageB = "Example B";
        [Tooltip("Text will be displayed if preprocessor define 'EXAMPLE_C' is enabled")]
        [SerializeField] private string exampleMessageC = "Example C";
#pragma warning restore
    
        public void InvokeExample()
        {
#if EXAMPLE_A
            Debug.Log(exampleMessageA);
#endif
    
#if EXAMPLE_B
            Debug.Log(exampleMessageB);
#endif
    
#if EXAMPLE_C
            Debug.Log(exampleMessageC);
#endif
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    #region --- [EDITOR] ---
    
#if UNITY_EDITOR
    
    [UnityEditor.CustomEditor(typeof(ExampleBehaviour))]
    internal class ExampleBehaviourInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Invoke Example", GUILayout.MinWidth(120), GUILayout.Height(40)))
            {
                ((ExampleBehaviour)target).InvokeExample();
            }
            UnityEditor.EditorGUILayout.HelpBox("When invoked, the following code is executed", UnityEditor.MessageType.Info);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            var style = GUI.skin.GetStyle("HelpBox");
            style.richText = true;
            UnityEditor.EditorGUILayout.TextArea(MESSAGE, style);
        }
        
        
        private const string MESSAGE = 
            "    <b>public void InvokeExample()</b>\n" +
            "    {\n" +
            "#if EXAMPLE_A\n" +
            "        <b>Debug.Log(exampleMessageA);</b>\n" +
            "#endif\n" +
            "\n" +
            "#if EXAMPLE_B\n" +
            "        <b>Debug.Log(exampleMessageB);</b>\n" +
            "#endif\n" +
            "\n" +
            "#if EXAMPLE_C\n" +
            "        <b>Debug.Log(exampleMessageC);</b>\n" +
            "#endif\n" +
            "    }" +
            "";
    }
#endif
    
    #endregion
}