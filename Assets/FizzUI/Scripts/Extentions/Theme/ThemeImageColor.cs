using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Extentions
{
    public class ThemeImageColor : MonoBehaviour
    {
        [SerializeField] ThemeColor Color;

        private void Awake ()
        {
            if (FizzTheme.FizzThemeData == null) return;

            Image image = gameObject.GetComponent<Image> ();
            if (image != null)
            {
                Color toApply = GetThemeColor (Color);
                image.color = new Color (toApply.r, toApply.g, toApply.b, image.color.a);
            }
        }

        private Color GetThemeColor (ThemeColor color)
        {
            switch (color)
            {
                case ThemeColor.Primary:
                    return FizzTheme.FizzThemeData.Primary;
                case ThemeColor.Secondary:
                    return FizzTheme.FizzThemeData.Secondary;
                case ThemeColor.Base_1:
                    return FizzTheme.FizzThemeData.Base_1;
                case ThemeColor.Base_2:
                    return FizzTheme.FizzThemeData.Base_2;
                default:
                    return FizzTheme.FizzThemeData.Base_2;
            }
        }
    }
}