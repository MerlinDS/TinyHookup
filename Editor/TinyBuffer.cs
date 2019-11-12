using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TinyHookup.Editor
{
    public class TinyBuffer
    {
        private TinySelector _selector;
        private TinyGraph _graph;

        private readonly List<Guid> _copyBuffer = new List<Guid>();

        public void OnEnable(TinySelector selector) =>
            _selector = selector;

        public void OnUpdate(TinyGraph graph) => 
            _graph = graph;

        public void Copy()
        {
            Clear();
            _copyBuffer.AddRange(_selector);
        }

        public void Paste(Vector2 position)
        {
            _selector.Clean();
            var nodes = _copyBuffer.Select(x => _graph.GetNode(x)).ToList();
            var offset = GetPositionOffset(position, nodes);

            var map = PasteNodes(nodes, offset);
            PasteEdges(map);

            _selector.Add(map.Values.Select(_graph.GetNode));
            GUI.changed = true;
        }

        private void PasteEdges(IReadOnlyDictionary<Guid, Guid> map)
        {
            foreach (var pair in map)
            {
                var sourceEdges = _graph.GetEdges(pair.Key).Where(x => map.ContainsKey(x.In)).ToList();
                foreach (var edge in sourceEdges)
                    _graph.CopyEdge(pair.Value, map[edge.In], edge.Data);
            }
        }

        private Dictionary<Guid, Guid> PasteNodes(List<TinyNode> nodes, Vector2 offset)
        {
            var map = new Dictionary<Guid, Guid>();
            foreach (var source in nodes)
            {
                var position = offset + source.Position;
                var copy = _graph.CopyNode(source, position);
                map.Add(source.Id, copy.Id);
            }

            return map;
        }

        private Vector2 GetPositionOffset(Vector2 position, IReadOnlyCollection<TinyNode> nodes) =>
            position - new Vector2(nodes.Min(x => x.Position.x), nodes.Min(x => x.Position.y));

        public void Clear()
        {
            _copyBuffer.Clear();
        }
    }
}