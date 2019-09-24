using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Extentions
{
    public class ThemeLabel : MonoBehaviour
    {
        [SerializeField] ThemeFont Font;
        
        private void Awake ()
        {
            if (FizzTheme.FizzThemeData == null) return;

            Text label = gameObject.GetComponent<Text> ();
            if (label != null)
            {
                label.font = GetThemeFont (Font);
            }
        }

        private Font GetThemeFont (ThemeFont font)
        {
            switch (font)
            {
                case ThemeFont.Bold:
                    return FizzTheme.FizzThemeData.BoldFont;
                case ThemeFont.Normal:
                    return FizzTheme.FizzThemeData.NormalFont;
                default:
                    return FizzTheme.FizzThemeData.NormalFont;
            }
        }
    }
}