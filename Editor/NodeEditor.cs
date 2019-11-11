using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    [CustomEditor(typeof(TinyNode))]
    public sealed class NodeEditor : TinyDataEditor
    {
        private SerializedProperty _title;
        protected override TinyInspectorType InspectorType => TinyInspectorType.Node;

        protected override void InternalOnEnable() => 
            _title = serializedObject.FindProperty("Title");

        private void OnDisable() => 
            _title = null;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_title, new GUIContent("Title", "A title of the node"));
            
            
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            
            base.OnInspectorGUI();
        }
    }
}