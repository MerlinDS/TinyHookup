using System;
using UnityEditor;

namespace TinyHookup.Editor
{
    public abstract class TinyHookupContext : EditorWindow
    {
        protected string Label { get; set; }
        public TinyGraph Graph { get; private set; }
        public TinySelector Selector { get; } = new TinySelector();

        public Type NodeDataDrawer { get; private set; }
        public Type EdgeDataDrawer { get; private set; }

        protected void SetNodeDataDrawer<TDrawer>() where TDrawer : ITinyDataDrawer =>
            NodeDataDrawer = typeof(TDrawer);

        protected void SetEdgeDataDrawer<TDrawer>() where TDrawer : ITinyDataDrawer =>
            EdgeDataDrawer = typeof(TDrawer);

        protected void CreateGraph()
        {
            if (Graph != null)
                Graph.Dispose();

            Graph = TinyGraph.Create();
            Graph.OnCreateNode += OnCreateNode;
            Graph.OnCreateEdge += OnCreateEdge;
            Graph.OnCopyNode += OnCopyNode;
            Graph.OnCopyEdge += OnCopyEdge;
        }

        protected virtual void Clear()
        {
            Selector.Clean(true);
            if (Graph != null)
            {
                Graph.OnCreateNode -= OnCreateNode;
                Graph.OnCreateEdge -= OnCreateEdge;
                Graph.OnCopyNode -= OnCopyNode;
                Graph.OnCopyEdge -= OnCopyEdge;
                Graph.Dispose();
            }
            Graph = null;
        }

        protected virtual object OnCopyNode(object data)
        {
            return data;
        }
        
        protected virtual object OnCopyEdge(object data)
        {
            return data;
        }

        protected virtual void OnCreateNode(TinyNode node)
        {
            
        }
        
        protected virtual void OnCreateEdge(TinyEdge edge)
        {
            
        }
    }
}