using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public abstract class TinyGraphEditorWindow : TinyHookupContext
    {
        private readonly TinyBuffer _buffer = new TinyBuffer();
        private readonly TinyEventProcessor _eventProcessor = new TinyEventProcessor();

        protected virtual void OnEnable()
        {
            _buffer.OnEnable(Selector);
            _eventProcessor.OnEnable(Selector, _buffer);

            Selector.OnNodeSelectionChanged += guid =>
                Selection.SetActiveObjectWithContext(Graph.GetNode(guid), this);

            Selector.OnEdgeSelectionChanged += (@in, @out) =>
                Selection.SetActiveObjectWithContext(Graph.GetEdge(@in, @out), this);
        }

        protected virtual void OnDisable() => Clear();

        private void OnGUI()
        {
            _buffer.OnUpdate(Graph);
            _eventProcessor.Process(Graph);

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
            if (!Selector.IsEmpty)
                Repaint();
        }

        private void DrawGraph()
        {
            var rect = TinyGUI.BeginZoom(_eventProcessor.Scale);
            TinyGUI.DrawGrid(Graph == null ? Vector2.zero : Graph.Offset, rect);
            DrawGraph(false);
            DrawGraph(true);
            TinyGUI.EndZoom();
            DrawMenu();
        }

        private void DrawMenu()
        {
            GUI.BeginGroup(new Rect(0, 0, position.width, 24), Label, "ProgressBarBack");
            var selected = GUI.Toolbar(new Rect(2, 2, position.width / 4, 20), -1, new[] {"New", "Load", "Save"});
            GUI.EndGroup();

            switch (selected)
            {
                case 0:
                    Clear();
                    CreateGraph();
                    OnNew();
                    break;
                case 1:
                    OnLoad();
                    break;
                case 2:
                    OnSave();
                    break;
                default:
                    return;
            }
        }

        private void DrawGraph(bool selected)
        {
            if (Graph == null)
                return;

            foreach (var node in Graph.Nodes.Where(x => selected == Selector.IsSelected(x)))
                TinyGUI.DrawNode(node, selected, Graph.HasInEdge(node), Graph.HasOutEdge(node));

            foreach (var edge in Graph.Edges.Where(x => selected == Selector.IsSelected(x)))
            {
                var @out = TinyGUI.GetOutRect(Graph.GetNode(edge.Out));
                var @in = TinyGUI.GetInRect(Graph.GetNode(edge.In));
                if (TinyGUI.DrawEdge(@out, @in, selected))
                    Selector.AddSingle(edge);
            }
        }

        protected override void Clear()
        {
            if (Selection.activeObject is TinyEdge)
                Selection.activeObject = null;
            if (Selection.activeObject is TinyNode)
                Selection.activeObject = null;

            _buffer.Clear();
            base.Clear();
        }

        protected abstract void OnNew();
        protected abstract void OnLoad();
        protected abstract void OnSave();
        
        
    }
}