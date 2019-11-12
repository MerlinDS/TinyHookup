using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public class TinyEventProcessor
    {
        private TinySelector _selector;
        private TinyBuffer _buffer;
        private TinyGraph _graph;

        public bool MultiSelectionOn { get; private set; }

        public Vector2 StartPosition { get; private set; }

        public Vector2 CurrentPosition { get; private set; }

        public bool NewEdge { get; private set; }

        public void OnEnable(TinySelector selector, TinyBuffer buffer)
        {
            _selector = selector;
            _buffer = buffer;
        }

        public void Process(TinyGraph graph)
        {
            _graph = graph;
            Process(Event.current);
        }

        private void Process(Event @event)
        {
            if (_graph == null)
                return;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (@event.type)
            {
                case EventType.KeyUp:
                    CurrentPosition = @event.mousePosition;
                    if (@event.keyCode == KeyCode.Delete)
                    {
                        _graph.RemoveNodes(_selector);
                        _graph.RemoveEdge(_selector);
                        _selector.Clean();
                        GUI.changed = true;
                    }
                    else if(@event.keyCode == KeyCode.C && @event.control)
                        _buffer.Copy();
                    else if(@event.keyCode == KeyCode.V && @event.control)
                        _buffer.Paste();

                    break;
                case EventType.ScrollWheel:
                    /*_scale = Mathf.Clamp(_scale + @event.delta.y * 0.01F, 0.5F, 1);
                    UnityEngine.GUI.changed = true;*/
                    break;
                case EventType.MouseDown:
                    StartPosition = @event.mousePosition;
                    CurrentPosition = @event.mousePosition;
                    GUI.changed = ProcessMouseDown(@event);
                    break;
                case EventType.MouseDrag:
                    CurrentPosition = @event.mousePosition;
                    GUI.changed = ProcessDrag(@event);
                    break;
                case EventType.MouseUp:
                    GUI.changed = ProcessMouseUp(@event);
                    MultiSelectionOn = false;
                    NewEdge = false;
                    break;
            }
        }

        private bool ProcessDrag(Event @event)
        {
            if (NewEdge)
                //Avoid dragging during a new edge drawing
                return false;

            if (@event.button == 1)
            {
                foreach (var node in _graph.Nodes)
                    node.Position += @event.delta;
                _graph.Offset += @event.delta;
                @event.Use();
                return true;
            }

            if (_selector.IsEmpty)
                return false;

            //Dragging of selected nodes
            foreach (var node in _selector)
                _graph.GetNode(node).Position += @event.delta;

            @event.Use();
            return true;
        }

        private bool ProcessMouseDown(Event @event)
        {
            if (@event.button == 1)
            {
                //Clean up selection on right mouse click
                _selector.Clean();
//                _newEdge = EditingEdge.Empty;
                return true;
            }

            if (@event.button != 0)
                return false;

            var node = _graph.GetNodeUnder(CurrentPosition, _selector);
            if (node == default)
            {
                _selector.Clean();
                MultiSelectionOn = true;
                return false;
            }

            //Try Select node or edge on left mouse click
            var outRect = TinyGUI.GetOutRect(node);
            NewEdge = outRect.Contains(CurrentPosition);
            if (NewEdge)
                StartPosition = outRect.center;

            _selector.AddSingle(node);
            return true;
        }

        private bool ProcessMouseUp(Event @event)
        {
            if (@event.button == 1)
            {
                //End of mouse right click
                if (Vector2.Distance(@event.mousePosition, StartPosition) < 1)
                    ProcessContextMenu(@event.mousePosition);
                return false;
            }

            if (@event.button != 0)
                return false;

            //End of mouse left click
            if (NewEdge)
            {
                var @in = _graph.GetNodeUnder(@event.mousePosition);
                if (@in != null)
                    _graph.CreateEdge(_graph.GetNodeUnder(StartPosition), @in);
                else
                    ProcessContextMenu(@event.mousePosition);
                return true;
            }

            if (!MultiSelectionOn)
                return false;

            var selection = GetSelectionRect(StartPosition, CurrentPosition);
            _selector.Add(_graph.Nodes.Where(x => selection.Overlaps(TinyGUI.GetNodeRect(x.Position))));
            return true;
        }

        private void ProcessContextMenu(Vector2 mousePosition)
        {
            var connect = NewEdge;
            var @out = _graph.GetNodeUnder(StartPosition);
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () =>
            {
                var @in = _graph.CreateNode("Node", mousePosition);
                if (connect)
                    _graph.CreateEdge(@out, @in);
            });
            /*if (@out != null)
            {
                genericMenu.AddItem(new GUIContent("Copy"), false, () => Copy(@out));
                genericMenu.AddItem(new GUIContent("Delete"), false, () => _graph.RemoveNodes(new[] {@out.Id}));
            }
            else if(!string.IsNullOrEmpty(EditorGUIUtility.systemCopyBuffer))
                genericMenu.AddItem(new GUIContent("Paste"), false, Paste);*/

            genericMenu.ShowAsContext();
        }

        private static Rect GetSelectionRect(Vector2 startPosition, Vector2 endPosition)
        {
            var minX = Mathf.Min(startPosition.x, endPosition.x);
            var minY = Mathf.Min(startPosition.y, endPosition.y);
            var maxX = Mathf.Max(startPosition.x, endPosition.x);
            var maxY = Mathf.Max(startPosition.y, endPosition.y);
            var selection = new Rect(minX, minY, maxX - minX, maxY - minY);
            return selection;
        }
    }
}