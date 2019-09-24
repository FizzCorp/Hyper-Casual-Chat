using UnityEngine;

namespace Fizz.UI.Extentions
{
    public class FizzTheme : ScriptableObject
    {
        public const string FizzThemeAssetName = "FizzTheme";
        public const string FizzThemePath = "FizzUI/Resources";
        public const string FizzThemeAssetExtension = ".asset";

        private static FizzTheme instance;

        [SerializeField]
        private FizzThemeData currentTheme;

        public static FizzThemeData FizzThemeData
        {
            get
            {
                return Instance.currentTheme;
            }

            set
            {
                if (Instance.currentTheme != value)
                {
                    Instance.currentTheme = value;
                }
            }
        }

        public static FizzTheme Instance
        {
            get
            {
                instance = NullableInstance;

                if (instance == null)
                {
                    // If not found, autocreate the asset object.
                    instance = ScriptableObject.CreateInstance<FizzTheme> ();
                }

                return instance;
            }
        }

        public static FizzTheme NullableInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load (FizzThemeAssetName) as FizzTheme;
                }

                return instance;
            }
        }
    }
}
