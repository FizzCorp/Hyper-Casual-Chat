using Fizz.UI.Extentions;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzCustomStickerView : IFizzCustomMessageCellView
    {
        [SerializeField] Image StickerImage;

        private Sprite sticker;

        public void SetSticker (Sprite sticker)
        {
            this.sticker = sticker;
        }

        public override void LoadView ()
        {
            base.LoadView ();

            StickerImage.sprite = sticker;

            if (transform.parent != null)
            {
                CustomLayoutGroupElement layoutGroupElement = transform.parent.GetComponent<CustomLayoutGroupElement> ();
                if (layoutGroupElement != null)
                {
                    layoutGroupElement.anchor = TextAnchor.MiddleRight;
                }
            }
        }
    }
}