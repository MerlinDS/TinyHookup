using UnityEngine;

namespace TinyHookup.Editor
{
    public static partial class TinyGUI
    {
        private const float KEditorWindowTabHeight = 21.0f;
        private static  Matrix4x4 _prevMatrix;

        public static Rect ZoomArea { get; private set; }

        public static Rect BeginZoom(float zoom)
        {
            var possibleZoomArea = GUILayoutUtility.GetRect(0, 10000, 0, 10000);
            
            if (Event.current.type == EventType.Repaint) //the size is correct during repaint, during layout it's 1,1
                ZoomArea = possibleZoomArea;

            /*
             End the group Unity begins automatically for an EditorWindow to clip out the window tab.
             This allows us to draw outside of the size of the EditorWindow.
             */
            GUI.EndGroup();
            
            var clippedArea = ZoomArea.ScaleSizeBy(1f / zoom, ZoomArea.TopLeft());
            clippedArea.y += KEditorWindowTabHeight;
            GUI.BeginGroup(clippedArea);
            
            _prevMatrix = GUI.matrix;
            var translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            var scale = Matrix4x4.Scale(new Vector3(zoom, zoom, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
            
            return clippedArea;
        }

        public static void EndZoom()
        {
            GUI.matrix = _prevMatrix; //restore the original matrix
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, KEditorWindowTabHeight, Screen.width, Screen.height));
        }
    }
    
    public static class RectExtensions
    {
        public static Vector2 TopLeft(this Rect rect) => 
            new Vector2(rect.xMin, rect.yMin);

        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            var result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
    }
}