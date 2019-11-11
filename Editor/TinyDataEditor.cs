using System;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public abstract class TinyDataEditor : UnityEditor.Editor
    {
        private ITinyDataDrawer _drawer;

        protected abstract TinyInspectorType InspectorType { get; }

        private void OnEnable()
        {
            try
            {
                if (serializedObject.context is TinyGraph context)
                    _drawer = (ITinyDataDrawer) Activator.CreateInstance(
                        InspectorType == TinyInspectorType.Edge ? context.EdgeDataDrawer : context.NodeDataDrawer);
                InternalOnEnable();
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                DestroyImmediate(this);
            }
        }

        protected abstract void InternalOnEnable();

        protected override void OnHeaderGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(28f);
            GUILayout.BeginVertical();
            GUILayout.Space(12f);
            GUILayout.Label(InspectorType.ToString(), EditorStyles.boldLabel);
            GUILayout.Space(12f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            if (_drawer == null)
                return;

            serializedObject.Update();
            if (!(serializedObject.targetObject is ITinyDataProvider dataProvider))
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            dataProvider.Data = _drawer.OnGUI(dataProvider.Data);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}