using System.Linq;
using UnityEngine;

namespace TinyHookup.Editor
{
    public static class TinyGraphGUIExtension
    {
        public static TinyNode GetNodeUnder(this TinyGraph graph, Vector2 point, TinySelector selector = null)
        {
            var nodes = graph.Nodes.Where(x => TinyGUI.GetNodeRect(x.Position).Contains(point));
            return selector == null ? nodes.FirstOrDefault() : nodes.OrderByDescending(selector.IsSelected).FirstOrDefault();
        }
    }
}