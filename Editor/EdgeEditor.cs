using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    [CustomEditor(typeof(TinyEdge))]
    public sealed class EdgeEditor : TinyDataEditor
    {
        private TinyEdge _edge;

        protected override TinyInspectorType InspectorType => TinyInspectorType.Edge;

        protected override void InternalOnEnable() => 
            _edge = serializedObject.targetObject as TinyEdge;

        protected override void OnDisable() => 
            _edge = null;

        public override void OnInspectorGUI()
        {
            var @in = Context.Graph.GetNode(_edge.In);
            var @out = Context.Graph.GetNode(_edge.Out);
            _edge.Weight = EditorGUILayout.FloatField("Weight", _edge.Weight);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(@out.Title))
                Context.Selector.AddSingle(@out);
            EditorGUILayout.LabelField("=>", GUILayout.Width(22));
            if (GUILayout.Button(@in.Title))
                Context.Selector.AddSingle(@in);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Remove this edge"))
            {
                Context.Graph.RemoveEdge(_edge);
                return;
            }
            
            base.OnInspectorGUI();
        }
    }
}