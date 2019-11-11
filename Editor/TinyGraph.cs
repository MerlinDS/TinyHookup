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

        public Type NodeDataDrawer { get; set; }
        public Type EdgeDataDrawer { get; set; }

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

        public TinyNode CreateNode(string label, Vector2 position, object data = default)
        {
            var node = TinyNode.Create(label, position);
            node.Data = data;
            _nodes.Add(node);
            return node;
        }

        public TinyNode GetNode(Guid id) =>
            _nodes.FirstOrDefault(x => x.Id == id);

        public TinyEdge GetEdge(Guid @in, Guid @out) =>
            _edges.First(e => e.In == @in && e.Out == @out);

        public void CreateEdge(TinyNode @out, TinyNode @in, object data = default)
        {
            if(@in.Id != @out.Id && !_edges.Any(x=>x.In == @in.Id && x.Out == @out.Id))
                _edges.Add(TinyEdge.Create(@out.Id, @in.Id, data));
        }

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
            foreach (var edge in  _edges.Where(x => x.In == id || x.Out == id).ToList())
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