using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace HCGames.Extensions.Editor
{
    
    [InitializeOnLoad]
    public class PlayCustomization
    {
        private const string ToggleKey = "CustomPlayButtonToggle";
        private const string PreviousSceneKey = "PreviousScene";
        private const string MenuKey = "HCTools/Start in Loading Scene";
        private static bool _isEnabled;
        private static string _previousScene;

        static PlayCustomization()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.delayCall += Initialize;
        }
        
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            _isEnabled = EditorPrefs.GetBool(ToggleKey, false);
            _previousScene = EditorPrefs.GetString(PreviousSceneKey, "");
            Menu.SetChecked(MenuKey, _isEnabled);
        }

        [MenuItem(MenuKey, false, 1000)]
        private static void TogglePlayButtonCustomization()
        {
            _isEnabled = !_isEnabled;
            EditorPrefs.SetBool(ToggleKey, _isEnabled);
            Menu.SetChecked(MenuKey, _isEnabled);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!_isEnabled) return;
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                CheckAndChangeScene();
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                ReturnToPreviousScene();
            }
        }

        private static void ReturnToPreviousScene()
        {
            if (string.IsNullOrEmpty(_previousScene)) return;
            EditorSceneManager.OpenScene(_previousScene);
        }

        private static void CheckAndChangeScene()
        {
            _previousScene = SceneManager.GetActiveScene().path;
            EditorPrefs.SetString(PreviousSceneKey, _previousScene);
            if (SceneManager.GetActiveScene().name == "LoadingScene") return;
            EditorSceneManager.OpenScene("Assets/Scenes/LoadingScene.unity");
        }
    }

}