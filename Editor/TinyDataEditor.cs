using System;
using UnityEditor;
using UnityEngine;

namespace TinyHookup.Editor
{
    public abstract class TinyDataEditor : UnityEditor.Editor
    {
        private ITinyDataDrawer _drawer;
        protected TinyHookupContext Context { get; private set; }


        protected abstract TinyInspectorType InspectorType { get; }

        private void OnEnable()
        {
            try
            {
                Context = serializedObject.context as TinyHookupContext;
                if(Context == null)
                    throw new ArgumentException("Can't find context");
                    
                var drawerType = InspectorType == TinyInspectorType.Edge ? Context.EdgeDataDrawer : Context.NodeDataDrawer;
                if(drawerType != null)
                    _drawer = (ITinyDataDrawer) Activator.CreateInstance(drawerType);
                InternalOnEnable();
            }
#pragma warning disable 168
            catch (ArgumentException e)
#pragma warning restore 168
            {
                DestroyImmediate(this);
            }
        }

        protected virtual void OnDisable()
        {
            _drawer = null;
            Context = null;
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

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Data", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            dataProvider.Data = _drawer.OnGUI(dataProvider.Data);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}