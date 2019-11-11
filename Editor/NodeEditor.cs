using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    [CustomEditor(typeof(TinyNode))]
    public sealed class NodeEditor : TinyDataEditor
    {
        private SerializedProperty _title;
        protected override TinyInspectorType InspectorType => TinyInspectorType.Node;

        private static bool Foldout;

        protected override void InternalOnEnable() => 
            _title = serializedObject.FindProperty("Title");

        protected override void OnDisable() => 
            _title = null;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_title, new GUIContent("Title", "A title of the node"));
            
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            OnDrawEdges();
            base.OnInspectorGUI();
        }

        private void OnDrawEdges()
        {
            Foldout = EditorGUILayout.Foldout(Foldout, "Out edges", true);
            if(!Foldout)
                return;
            
            var node = serializedObject.targetObject as TinyNode;
            foreach (var edge in Context.Graph.GetEdges(node.Id))
            {
                var inNode = Context.Graph.GetNode(edge.In);
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button("=>", GUILayout.Width(28)))
                    Context.Selector.AddSingle(edge);
                if (GUILayout.Button(inNode.Title))
                    Context.Selector.AddSingle(inNode);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}