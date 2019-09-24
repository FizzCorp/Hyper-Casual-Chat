using Fizz.UI.Extentions;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    public class FizzCustomPhraseView : IFizzCustomMessageCellView
    {
        [SerializeField] Text PhraseLabel;

        private string phrase;

        public void SetPhrase (string phrase)
        {
            this.phrase = phrase;
        }

        public override void LoadView ()
        {
            base.LoadView ();

            PhraseLabel.text = phrase;

            if (transform.parent != null)
            {
                CustomLayoutGroupElement layoutGroupElement = transform.parent.GetComponent<CustomLayoutGroupElement> ();
                if (layoutGroupElement != null)
                {
                    layoutGroupElement.anchor = TextAnchor.MiddleLeft;
                }
            }
        }
    }
}