using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fizz.Demo
{
    public class SceneSelectorScript : MonoBehaviour
    {
        public void HandleHypercasualInputView ()
        {
            SceneManager.LoadScene ("HypercasualInputView");
        }

        public void HandleClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}