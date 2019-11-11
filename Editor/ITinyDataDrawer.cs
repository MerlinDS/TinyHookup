using System;

namespace TinyHookup.Editor
{
    [Flags]
    public enum TinyInspectorType
    {
        Node,
        Edge
    }
    public interface ITinyDataDrawer
    {
        
        object OnGUI(object data);
    }
}