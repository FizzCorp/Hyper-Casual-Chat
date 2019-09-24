using UnityEngine;
using UnityEngine.EventSystems;

namespace Fizz.UI.Extentions
{
    public class DragThresholdUtil : MonoBehaviour
    {
        void Start ()
        {
            int defaultValue = EventSystem.current.pixelDragThreshold;
            EventSystem.current.pixelDragThreshold =
                    Mathf.Max (
                        defaultValue,
                        (int)(defaultValue * Screen.dpi / 160f));
        }
    }
}