using System;
using UnityEngine;

namespace TinyHookup.Editor
{
    public class TinyEdge : ScriptableObject, ITinyConnection, ITinyDataProvider
    {
        public Guid Out { get; private set; }
        public Guid In { get; private set; }

        public float Weight { get; set; }

        public object Data { get; set; }

        public static TinyEdge Create(Guid @out, Guid @in, object data)
        {
            var tinyEdge = CreateInstance<TinyEdge>();
            tinyEdge.Out = @out;
            tinyEdge.In = @in;
            tinyEdge.Data = data;
            tinyEdge.Weight = 0;
            return tinyEdge;
        }
    }
    public interface ITinyConnection
    {
        Guid Out { get; }
        Guid In { get; }
    }
}