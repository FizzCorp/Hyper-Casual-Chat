using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fizz.UI.Extentions
{

    [InitializeOnLoad]
    [CustomEditor (typeof (FizzTheme))]
    public class FizzThemeEditor : Editor
    {
        [MenuItem ("Window/Fizz/Edit FizzTheme")]
        public static void Edit ()
        {
            var instance = FizzTheme.NullableInstance;

            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<FizzTheme> ();
                string properPath = Path.Combine (Application.dataPath, FizzTheme.FizzThemePath);
                if (!Directory.Exists (properPath))
                {
                    Directory.CreateDirectory (properPath);
                }

                string fullPath = Path.Combine (
                                      Path.Combine ("Assets", FizzTheme.FizzThemePath),
                                      FizzTheme.FizzThemeAssetName + FizzTheme.FizzThemeAssetExtension);
                AssetDatabase.CreateAsset (instance, fullPath);
            }

            Selection.activeObject = FizzTheme.Instance;
        }

        public FizzThemeEditor ()
        {
           
        }

        public override void OnInspectorGUI ()
        {
            //base.OnInspectorGUI ();
            serializedObject.Update ();

            EditorGUILayout.PropertyField (serializedObject.FindProperty ("currentTheme"), true);

            serializedObject.ApplyModifiedProperties ();
        }
    }
}