using Fizz.UI.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Fizz.UI
{
    public class FizzInputView : FizzBaseComponent
    {
        
        public SendMessageEvent OnSendMessage;
        public SendDataEvent OnSendData;

        #region Public Methods

        public virtual void Reset()
        {
            
        }

        public void SetInteractable(bool interactable)
        {
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = interactable;
            }
        }

        public void SetVisibility(bool visible)
        {
            CanvasGroup cg = gameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = visible ? 1 : 0;
            }
        }

        #endregion
        
        [System.Serializable]
        public class SendMessageEvent : UnityEvent<string>
        {

        }

        [System.Serializable]
        public class SendDataEvent : UnityEvent<Dictionary<string, string>>
        {

        }
    }
}
