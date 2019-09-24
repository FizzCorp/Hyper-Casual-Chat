using Fizz.UI.Extentions;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.Demo
{
    public class ThemeSelectorScript : MonoBehaviour
    {
        [SerializeField] FizzThemeData DefaultTheme;
        [SerializeField] FizzThemeData BlueTheme;
        [SerializeField] FizzThemeData GreenTheme;
        [SerializeField] FizzThemeData PurpleTheme;

        [SerializeField] Toggle defaultToggle;
        [SerializeField] Toggle blueToggle;
        [SerializeField] Toggle greenToggle;
        [SerializeField] Toggle purpleToggle;

        private void Awake ()
        {
            SetActiveToggle ();
        }

        public void HandleDefaultToggleValue (bool val)
        {
            if (val)
            {
                FizzTheme.FizzThemeData = DefaultTheme;
            }
        }

        public void HandleBueToggleValue (bool val)
        {
            if (val)
            {
                FizzTheme.FizzThemeData = BlueTheme;
            }
        }

        public void HandleGreenToggleValue (bool val)
        {
            if (val)
            {
                FizzTheme.FizzThemeData = GreenTheme;
            }
        }

        public void HandlePurpleToggleValue (bool val)
        {
            if (val)
            {
                FizzTheme.FizzThemeData = PurpleTheme;
            }
        }

        private void SetActiveToggle ()
        {
            if (FizzTheme.FizzThemeData.Equals (DefaultTheme)) defaultToggle.isOn = true;
            if (FizzTheme.FizzThemeData.Equals (BlueTheme)) blueToggle.isOn = true;
            if (FizzTheme.FizzThemeData.Equals (GreenTheme)) greenToggle.isOn = true;
            if (FizzTheme.FizzThemeData.Equals (PurpleTheme)) purpleToggle.isOn = true;
        }

    }
}