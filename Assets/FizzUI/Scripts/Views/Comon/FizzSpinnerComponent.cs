using UnityEngine;

namespace Fizz.UI
{
    public class FizzSpinnerComponent : MonoBehaviour
    {
        [SerializeField] RectTransform target;
        [SerializeField] float rotationSpeed = 5;

        private bool rotationEnabled = false;

        #region Public Methods

        public void ShowSpinner()
        {
            gameObject.SetActive(true);
            rotationEnabled = true;
        }

        public void HideSpinner()
        {
            gameObject.SetActive(false);
            rotationEnabled = false;
        }

        void Update()
        {
            if (rotationEnabled)
            {
                target.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            }
        }

        #endregion
    }
}
