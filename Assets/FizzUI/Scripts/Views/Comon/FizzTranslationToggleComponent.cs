using Fizz.UI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzTranslationToggleComponent : MonoBehaviour
    {
        [SerializeField] Image orignalImage;
        [SerializeField] Image translateImage;
        
        bool _isOriginal;
        
        public void Configure(FizzChatCellTranslationState state)
        {
            orignalImage.gameObject.SetActive(state == FizzChatCellTranslationState.Original);
            translateImage.gameObject.SetActive(state != FizzChatCellTranslationState.Original);
        }

        public void ShowOriginal()
        {
            _isOriginal = true;
            orignalImage.gameObject.SetActive(true);
            translateImage.gameObject.SetActive(false);
        }

        public void ShowTranslate()
        {
            _isOriginal = false;
            orignalImage.gameObject.SetActive(false);
            translateImage.gameObject.SetActive(true);
        }

        public void Toggle()
        {
            _isOriginal = !_isOriginal;
            orignalImage.gameObject.SetActive(_isOriginal);
            translateImage.gameObject.SetActive(!_isOriginal);
        }
    }
}