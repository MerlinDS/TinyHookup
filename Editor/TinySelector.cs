using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyHookup.Editor
{
    public class TinySelector : IEnumerable<Guid>, ITinyConnection
    {
        private readonly HashSet<Guid> _selections = new HashSet<Guid>();
        
        public event Action<Guid> OnNodeSelectionChanged;
        public event Action<Guid, Guid> OnEdgeSelectionChanged;

        public Guid Out { get; private set; }
        public Guid In { get; private set; }

        public void Add(IEnumerable<TinyNode> nodes)
        {
            _selections.Clear();
            foreach (var node in nodes)
                _selections.Add(node.Id);
        }

        public void AddSingle(TinyEdge edge)
        {
            Clean();
            In = edge.In;
            Out = edge.Out;
            OnEdgeSelectionChanged?.Invoke(edge.In, edge.Out);
        }
        public void AddSingle(TinyNode node)
        {
            if (node == null || IsSelected(node))
                return;

            _selections.Clear();
            _selections.Add(node.Id);
            OnNodeSelectionChanged?.Invoke(node.Id);
        }

        public bool IsEmpty => _selections.Count == 0 && In == Guid.Empty && Out == Guid.Empty;
        public void Clean()
        {
            _selections.Clear();
            Out = In = Guid.Empty;
            OnNodeSelectionChanged?.Invoke(Guid.Empty);
        }

        public bool IsSelected(TinyNode node) =>
            node != null && _selections.Contains(node.Id);

        public bool IsSelected(TinyEdge edge) =>
            edge != null && (_selections.Contains(edge.In) || _selections.Contains(edge.Out)) 
            || In == edge.In && Out == edge.Out;

        public IEnumerator<Guid> GetEnumerator() =>
            _selections.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();


    }
}