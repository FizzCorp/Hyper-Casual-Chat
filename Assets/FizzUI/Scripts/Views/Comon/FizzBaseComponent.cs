using Fizz.Common;
using UnityEngine;

namespace Fizz.UI.Core
{
    /// <summary>
    /// Base UI Component
    /// </summary>
    public class FizzBaseComponent : MonoBehaviour
    {
        [SerializeField] RectTransform ConnectionNode;

        private RectTransform rectTransform;

        #region Properties
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = gameObject.GetComponent<RectTransform>();
                return rectTransform;
            }
        }
        #endregion

        #region Public Methods
        protected virtual void OnEnable()
        {
            try
            {
                FizzService.Instance.OnConnected += HandleOnConnected;
                FizzService.Instance.OnDisconnected += HandleOnDisconnected;
            }
            catch
            {
                FizzLogger.E("Something went wrong while binding event of FizzService.");
            }
        }

        protected virtual void OnDisable()
        {
            try
            {
                FizzService.Instance.OnConnected -= HandleOnConnected;
                FizzService.Instance.OnDisconnected -= HandleOnDisconnected;
            }
            catch
            {
                FizzLogger.E("Something went wrong while binding event of FizzService.");
            }
        }

        protected virtual void OnConnectionStateChange(bool isConnected)
        { }
        #endregion

        #region Private Methods
        private void SetupConnectionBanner()
        {
            if (ConnectionNode != null)
            {
                ConnectionNode.gameObject.SetActive(FizzService.Instance.IsConnected);
            }
        }
        #endregion

        #region EventHandle
        private void HandleOnConnected(bool sync)
        {
            if (ConnectionNode != null)
            {
                ConnectionNode.gameObject.SetActive(false);
            }

            OnConnectionStateChange(true);
        }

        private void HandleOnDisconnected(FizzException ex)
        {
            if (ConnectionNode != null)
            {
                ConnectionNode.gameObject.SetActive(true);
            }

            OnConnectionStateChange(false);
        }
        #endregion
    }
}