using Core;
using UnityEditor;
using UnityEngine;

namespace EditorCustom
{
    [CustomEditor(typeof(SessionManager))]
    public class SessionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);

            var sessionManager = (SessionManager) target;
            if (GUILayout.Button("Generate Template JSON"))
            {
                sessionManager.OnValidate();
                sessionManager.GenerateTemplateJson();
            }
        }
    }
}
