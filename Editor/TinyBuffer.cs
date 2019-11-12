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
        private Vector2 _mousePosition;

        public void OnEnable(TinySelector selector) => 
            _selector = selector;

        public void OnUpdate(TinyGraph graph)
        {
            _graph = graph;
            _mousePosition = Event.current.mousePosition;
        }

        public void Copy()
        {
            Clear();
            _copyBuffer.AddRange(_selector);
        }

        public void Paste()
        {
            _selector.Clean();
            var nodes = _copyBuffer.Select(x => _graph.GetNode(x)).ToList();
            var offset = GetPositionOffset(nodes);

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
                var position = _mousePosition + (source.Position - offset);
                var copy = _graph.CopyNode(source, position);
                map.Add(source.Id, copy.Id);
            }

            return map;
        }

        private static Vector2 GetPositionOffset(IReadOnlyCollection<TinyNode> nodes)
        {
            var min = new Vector2(nodes.Min(x => x.Position.x), nodes.Min(x => x.Position.y));
            var max = new Vector2(nodes.Max(x => x.Position.x), nodes.Max(x => x.Position.y));
            return max - min;
        }

        public void Clear()
        {
            _copyBuffer.Clear();
        }
    }
}