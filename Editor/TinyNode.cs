using System;
using UnityEngine;

namespace TinyHookup.Editor
{
    public sealed class TinyNode : ScriptableObject, ITinyDataProvider
    {
        public Guid Id;

        public TinyNodeType Type;

        public string Title;

        public Vector2 Position;
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
}