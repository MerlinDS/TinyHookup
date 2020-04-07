using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TinyHookup.Editor
{
    public class TinyGraph : ScriptableObject
    {
        private readonly List<TinyNode> _nodes = new List<TinyNode>();
        private readonly List<TinyEdge> _edges = new List<TinyEdge>();

        public Vector2 Offset { get; set; }
        public IEnumerable<TinyNode> Nodes => _nodes;

        public IEnumerable<TinyEdge> Edges => _edges;

        public event Action<TinyNode> OnCreateNode;
        
        public event Func<object, object> OnCopyNode;
        public event Func<object, object> OnCopyEdge;
        public event Action<TinyEdge> OnCreateEdge;

        public static TinyGraph Create() => CreateInstance<TinyGraph>();

        private void OnDestroy() => Dispose();

        public void Dispose()
        {
            foreach (var node in _nodes)
                DestroyImmediate(node);
            _nodes.Clear();
            foreach (var edge in _edges)
                DestroyImmediate(edge);
            _edges.Clear();
        }

        public TinyNode CreateNode(string label, Vector2 position, object data = default, TinyNodeType type = TinyNodeType.Regular)
        {
            var node = TinyNode.Create(label, position);
            node.Type = type;
            node.Data = data;
            OnCreateNode?.Invoke(node);
            _nodes.Add(node);
            return node;
        }
        
        public TinyNode CopyNode(TinyNode source, Vector2 position)
        {
            var node = TinyNode.Create($"{source.Title} Copy", position);
            node.Data = OnCopyNode?.Invoke(source.Data);
            _nodes.Add(node);
            return node;
        }

        public TinyNode GetNode(Guid id) =>
            _nodes.FirstOrDefault(x => x.Id == id);

        public TinyEdge GetEdge(Guid @in, Guid @out) =>
            _edges.First(e => e.In == @in && e.Out == @out);

        public IEnumerable<TinyEdge> GetEdges(Guid @out) =>
            _edges.Where(e => e.Out == @out);

        public void CreateEdge(TinyNode @out, TinyNode @in, object data = default) => 
            CreateEdge(@out.Id, @in.Id, data);

        public void CreateEdge(Guid @out, Guid @in, object data = default)
        {
            if (@in == @out || _edges.Any(x => x.In == @in && x.Out == @out))
                return;
            
            var edge = TinyEdge.Create(@out, @in, data);
            OnCreateEdge?.Invoke(edge);
            _edges.Add(edge);
        }

        public void CopyEdge(Guid @out, Guid @in, object data) => 
            _edges.Add(TinyEdge.Create(@out, @in, OnCopyEdge?.Invoke(data)));

        public bool HasInEdge(TinyNode node) =>
            _edges.Any(x => x.In == node.Id);

        public bool HasOutEdge(TinyNode node) =>
            _edges.Any(x => x.Out == node.Id);

        public IEnumerable<TinyNode> GetNodes(IEnumerable<Guid> ids)
            => _nodes.Where(x => ids.Contains(x.Id));

        public void RemoveEdge(ITinyConnection edge) =>
            RemoveEdge(edge.In, edge.Out);

        public void RemoveEdges(Guid id)
        {
            foreach (var edge in _edges.Where(x => x.In == id || x.Out == id).ToList())
                RemoveEdge(edge);
        }

        private void RemoveEdge(Guid @in, Guid @out)
        {
            foreach (var edge in _edges.Where(e => e.In == @in && e.Out == @out).ToList())
            {
                _edges.Remove(edge);
                DestroyImmediate(edge);
            }
        }

        public void RemoveNodes(IEnumerable<Guid> ids)
        {
            foreach (var node in _nodes.Where(x => ids.Contains(x.Id)).ToList())
            {
                _nodes.Remove(node);
                RemoveEdges(node.Id);
                DestroyImmediate(node);
            }
        }
    }
}