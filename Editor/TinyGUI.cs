using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public static class TinyGUI
    {
        #region Node

        public static void DrawNode(TinyNode node, bool selected, bool hasIn = false, bool hasOut = false)
        {
            GUI.Box(GetNodeRect(node.Position), node.Title, selected ? Styles.SelectedNode : Styles.UnSelectedNode);
            GUI.Toggle(GetInRect(node.Position), hasIn, string.Empty, Styles.NodeIn);
            GUI.Toggle(GetOutRect(node.Position), hasOut, string.Empty, Styles.NodeOut);
        }

        public static Rect GetNodeRect(Vector2 position) =>
            new Rect(position, Styles.NodeSize);

        public static Rect GetInRect(TinyNode node) =>
            GetInRect(node.Position);

        public static Rect GetInRect(Vector2 position) =>
            new Rect(new Vector2(position.x + 10, position.y + 12), Styles.ControlSize);

        public static Rect GetOutRect(TinyNode node) =>
            GetOutRect(node.Position);

        public static Rect GetOutRect(Vector2 position) =>
            new Rect(new Vector2(position.x + Styles.NodeSize.x - 30, position.y + 12), Styles.ControlSize);

        #endregion

        public static bool DrawEdge(Rect @out, Rect @in, bool selected = false) =>
            DrawEdge(@out.center - Vector2.one * 2, @in.center - Vector2.one * 2, selected);

        public static bool DrawEdge(Vector2 @out, Vector2 @in, bool selected)
        {
            Handles.DrawBezier(
                @in,
                @out,
                @in + Vector2.left * 50f,
                @out - Vector2.left * 50f,
                selected ? Styles.SelectedEdge : Styles.UnSelectedEdge,
                null,
                Styles.EdgeSize
            );

            return Handles.Button((@in + @out) * 0.5f, Quaternion.identity, Styles.EdgeSize, Styles.EdgePickSize,
                                  Handles.RectangleHandleCap);
        }
        
        public static void DrawGrid(Vector2 offset, Rect position, float scale)
        {
            DrawGrid(offset, position, 20 * scale, 0.4f, Color.gray);
            DrawGrid(offset, position, 100 * scale, 0.8f, Color.gray);
        }

        private static void DrawGrid(Vector2 offset, Rect position, float gridSpacing, float gridOpacity,
            Color gridColor)
        {
            var widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            var heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            var newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (var i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                                 new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (var j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                                 new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        #region Styles

        private static InternalStyles _styles;
        // ReSharper disable once ConvertToNullCoalescingCompoundAssignment
        private static InternalStyles Styles => _styles ?? (_styles = new InternalStyles());

        private class InternalStyles
        {
            public readonly float EdgeSize = 4;
            public readonly float EdgePickSize = 8;
            public readonly Vector2 NodeSize = new Vector2(200, 40);
            public readonly Vector2 ControlSize = new Vector2(20, 20);

            public readonly Color SelectedEdge = Color.blue;
            public readonly Color UnSelectedEdge = Color.gray;

            public readonly GUIStyle UnSelectedNode;
            public readonly GUIStyle SelectedNode;
            public readonly GUIStyle NodeIn;
            public readonly GUIStyle NodeOut;

            public InternalStyles()
            {
                UnSelectedNode = "flow node 0";
                SelectedNode = "flow node 1";
                NodeOut = NodeIn = "Radio";
//                NodeOut.active.
            }
        }

        #endregion

        public static void DrawMultiSelectionRect(Vector2 start, Vector2 current)
        {
            var temp = Handles.color;
            Handles.color = Color.blue;
            Handles.DrawLines(new Vector3[]
            {
                start, new Vector3(start.x, current.y),
                new Vector3(start.x, current.y), current,
                current, new Vector3(current.x, start.y),
                new Vector3(current.x, start.y), start
            });
            Handles.color = temp;
            GUI.changed = true;
        }
    }
}