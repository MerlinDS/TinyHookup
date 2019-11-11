using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    [CustomEditor(typeof(TinyEdge))]
    public sealed class EdgeEditor : TinyDataEditor
    {
        private TinyGraph _graph;
        private ITinyConnection _edge;

        protected override TinyInspectorType InspectorType => TinyInspectorType.Edge;

        protected override void InternalOnEnable()
        {
            _graph = serializedObject.context as TinyGraph;
            _edge = serializedObject.targetObject as ITinyConnection;
        }

        private void OnDisable()
        {
            _edge = null;
            _graph = null;
        }

        public override void OnInspectorGUI()
        {
            if (_graph == null)
            {
                Debug.LogWarning("Can't find graph");
                return;
            }
            
            var @out = _graph.GetNode(_edge.Out);
            EditorGUILayout.LabelField("Out", @out.Title);
            var @in = _graph.GetNode(_edge.In);
            EditorGUILayout.LabelField("In", @in.Title);
            EditorGUILayout.Space();
            if (GUILayout.Button("Remove this edge"))
            {
                _graph.RemoveEdge(_edge);
                return;
            }
            
            base.OnInspectorGUI();
        }
    }
}