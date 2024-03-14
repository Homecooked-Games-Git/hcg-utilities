namespace HCGames.Extensions.Editor
{
    using UnityEngine;
    using UnityEditor;

    public class SOToJsonConverter : EditorWindow
    {
        private ScriptableObject scriptableObject;
        private string jsonRepresentation = "";
        private Vector2 scrollPos;

        [MenuItem("HCTools/SO to JSON Converter")]
        public static void ShowWindow()
        {
            GetWindow<SOToJsonConverter>("SO to JSON Converter");
        }

        private void OnGUI()
        {
            GUILayout.Label("Drag and Drop Scriptable Object Here", EditorStyles.boldLabel);

            // Track changes to the scriptableObject field
            EditorGUI.BeginChangeCheck(); 
            scriptableObject = EditorGUILayout.ObjectField(scriptableObject, typeof(ScriptableObject), false) as ScriptableObject;
            if (EditorGUI.EndChangeCheck())
            {
                // Automatically convert to JSON when a new Scriptable Object is dragged in
                if (scriptableObject != null)
                {
                    jsonRepresentation = JsonConvert.SerializeObject(scriptableObject, Formatting.Indented);
                }
            }

            GUILayout.Label("JSON Representation", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            // Display the JSON in a non-editable text area
            EditorGUILayout.TextArea(jsonRepresentation, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }

}