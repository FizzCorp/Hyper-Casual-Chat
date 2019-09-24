using Fizz.UI.Extentions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzHypercasualStickerView : MonoBehaviour
    {
        [SerializeField] Image StickerImage;

        public Action<FizzHypercasualStickerView> OnStickerClick;

        public FizzHypercasualStickerDataItem StickerData { get { return data; } }

        private FizzHypercasualStickerDataItem data;
        private Button button;

        public void SetStickerData (FizzHypercasualDataItem dataItem)
        {
            data = (FizzHypercasualStickerDataItem)dataItem;

            StickerImage.sprite = data.Content;
        }

        private void Awake ()
        {
            button = gameObject.GetComponent<Button> ();
        }

        private void OnEnable ()
        {
            button.onClick.AddListener (OnClick);
        }

        private void OnDisable ()
        {
            button.onClick.RemoveListener (OnClick);
        }

        private void OnClick ()
        {
            if (OnStickerClick != null)
                OnStickerClick.Invoke (this);
        }
    }
}