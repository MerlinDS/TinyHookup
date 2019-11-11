using System;
using UnityEngine;

namespace TinyHookup.Editor
{
    public sealed class TinyNode : ScriptableObject, ITinyNode
    {
        public Guid Id;
        public string Title { get; set; }
        public Vector2 Position { get; set; }
        public object Data { get; set; }

        public static TinyNode Create(string title, Vector2 position, Guid id = default)
        {
            var node = CreateInstance<TinyNode>();
            node.Id = id == Guid.Empty ? Guid.NewGuid() : id;
            node.Position = position;
            node.Title = title;
            return node;
        }
    }

    public interface ITinyNode : ITinyDataProvider
    {
        string Title { get; set; }
        Vector2 Position { get; set; }
    }
}