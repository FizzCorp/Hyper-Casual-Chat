using UnityEngine;

namespace Fizz.Demo
{
    public class OrientationScript : MonoBehaviour
    {
        [SerializeField] ScreenOrientation Orientation;

        void Start()
        {
            Screen.orientation = Orientation;
        }
    }
}