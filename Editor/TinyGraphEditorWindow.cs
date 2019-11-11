﻿using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public abstract class TinyGraphEditorWindow : TinyHookupContext
    {
        private readonly TinyEventProcessor _eventProcessor = new TinyEventProcessor();

        protected virtual void OnEnable()
        {
            Selector.OnNodeSelectionChanged += guid =>
                Selection.SetActiveObjectWithContext(Graph.GetNode(guid), this);

            Selector.OnEdgeSelectionChanged += (@in, @out) =>
                Selection.SetActiveObjectWithContext(Graph.GetEdge(@in, @out), this);
        }

        protected virtual void OnDisable() => Clear();

        private void OnGUI()
        {
            _eventProcessor.Process(Graph, Selector);

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
            TinyGUI.DrawGrid(Graph == null ? Vector2.zero : Graph.Offset, position, 1);
            DrawGraph(false);
            DrawGraph(true);
            DrawMenu();
        }

        private void DrawMenu()
        {
            GUI.BeginGroup(new Rect(0, 0, position.width, 24));
            GUILayout.BeginHorizontal(GUILayout.Width(position.width / 4));
            var selected = GUILayout.Toolbar(-1, new[] {"New", "Load", "Save"});
            GUILayout.EndHorizontal();
            GUI.EndGroup();
            if (selected < 0)
                return;

            if (selected == 0)
            {
                Clear();
                CreateGraph();
            }
            if (selected == 1)
                OnLoad();
            if (selected == 2)
                OnSave();
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

            base.Clear();
        }

        protected abstract void OnLoad();
        protected abstract void OnSave();
    }
}