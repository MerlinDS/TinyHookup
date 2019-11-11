using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public abstract class TinyGraphEditorWindow : EditorWindow
    {
        private TinyGraph _graph;
        private readonly TinySelector _selector = new TinySelector();
        private readonly TinyEventProcessor _eventProcessor = new TinyEventProcessor();

        public event Action OnLoad;
        public event Action<IEnumerable<TinyNode>, IEnumerable<TinyEdge>> OnSave;

        protected virtual void OnEnable()
        {
            if (_graph != null)
                _graph.Dispose();

            _graph = TinyGraph.Create();

            _selector.OnNodeSelectionChanged += guid =>
                Selection.SetActiveObjectWithContext(_graph.GetNode(guid), _graph);

            _selector.OnEdgeSelectionChanged += (@in, @out) =>
                Selection.SetActiveObjectWithContext(_graph.GetEdge(@in, @out), _graph);
        }

        protected virtual void OnDisable() => Clear();

        private void OnGUI()
        {
            _eventProcessor.Process(_graph, _selector);

            DrawGraph();
            if (_eventProcessor.MultiSelectionOn)
                TinyGUI.DrawMultiSelectionRect(_eventProcessor.StartPosition, _eventProcessor.CurrentPosition);

            if (_eventProcessor.NewEdge)
                TinyGUI.DrawEdge(_eventProcessor.StartPosition, _eventProcessor.CurrentPosition, true);

            if (GUI.changed)
                Repaint();
        }

        private void OnInspectorUpdate()
        {
            if (!_selector.IsEmpty)
                Repaint();
        }

        private void DrawGraph()
        {
            if (_graph == null)
                return;

            TinyGUI.DrawGrid(_graph.Offset, position, 1);
            DrawGraph(false);
            DrawGraph(true);
            DrawMenu();
        }

        private void DrawMenu()
        {
            GUI.BeginGroup(new Rect(0, 0, position.width, 24));
            GUILayout.BeginHorizontal(GUILayout.Width(position.width / 4));
            var selected = GUILayout.Toolbar(-1, new[] {"Load", "Save", "Close"});
            GUILayout.EndHorizontal();
            GUI.EndGroup();
            if(selected < 0)
                return;
            
            if(selected == 0)
                OnLoad?.Invoke();
            if(selected == 1)
                OnSave?.Invoke(_graph.Nodes.ToArray(), _graph.Edges.ToArray());
            if(selected == 2)
                Clear();
        }

        private void DrawGraph(bool selected)
        {
            foreach (var node in _graph.Nodes.Where(x => selected == _selector.IsSelected(x)))
                TinyGUI.DrawNode(node, selected, _graph.HasInEdge(node), _graph.HasOutEdge(node));

            foreach (var edge in _graph.Edges.Where(x => selected == _selector.IsSelected(x)))
            {
                var @out = TinyGUI.GetOutRect(_graph.GetNode(edge.Out));
                var @in = TinyGUI.GetInRect(_graph.GetNode(edge.In));
                if (TinyGUI.DrawEdge(@out, @in, selected))
                    _selector.AddSingle(edge);
            }
        }

        protected void SetNodeDataDrawer<TDrawer>() where TDrawer : ITinyDataDrawer =>
            _graph.NodeDataDrawer = typeof(TDrawer);

        protected void SetEdgeDataDrawer<TDrawer>() where TDrawer : ITinyDataDrawer =>
            _graph.EdgeDataDrawer = typeof(TDrawer);

        // ReSharper disable once ParameterHidesMember
        protected Guid CreateNode(string label, Vector2 position, object data = default) =>
            _graph.CreateNode(label, position, data).Id;

        protected void CreateEdge(Guid @out, Guid @in, object data = default)
        {
            var outNode = _graph.GetNode(@out);
            var inNode = _graph.GetNode(@in);
            if (outNode == null || inNode == null)
            {
                Debug.LogError("One of nodes was not created yet. Use CreateNode method!");
                return;
            }

            _graph.CreateEdge(outNode, inNode, data);
        }

        protected void Clear()
        {
            if (Selection.activeObject is TinyEdge)
                Selection.activeObject = null;
            if (Selection.activeObject is TinyNode)
                Selection.activeObject = null;

            _graph.Dispose();
        }
    }
}