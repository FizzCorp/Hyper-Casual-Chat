using UnityEngine;

namespace Fizz.Demo
{
    public class SafeAreaScript : MonoBehaviour
    {
        private RectTransform panel;

        private Rect lastSafeArea = new Rect(0, 0, Screen.width, Screen.height);

        void Awake()
        {
            panel = GetComponent<RectTransform>();

            Refresh();
        }

        void FixedUpdate()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeRect = UpdateSafeAreaRect();
            if (safeRect != lastSafeArea)
            {
                ApplySafeArea(safeRect);
            }
        }

        Rect UpdateSafeAreaRect()
        {
#if UNITY_2017_2_OR_NEWER
            return Screen.safeArea;
#endif
        }

        void ApplySafeArea(Rect sRect)
        {
            lastSafeArea = sRect;

            var anchorMin = sRect.position;
            var anchorMax = sRect.position + sRect.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
        }
    }
}