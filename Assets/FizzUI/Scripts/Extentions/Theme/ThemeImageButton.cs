using Fizz.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Extentions
{
    public class ThemeImageButton : MonoBehaviour
    {
        [SerializeField] ThemeImage ThemeImage;
        [SerializeField] ThemeSprite ActivatedSprite;
        [SerializeField] ThemeSprite DeactivatedSprite;

        private void Awake ()
        {
            Button button = gameObject.GetComponent<Button> ();
            if (button != null && button.transition == Selectable.Transition.SpriteSwap)
            {
                SpriteState buttonSpriteState = button.spriteState;
                buttonSpriteState.disabledSprite = ThemeImage.GetThemeSprite (ActivatedSprite);
                buttonSpriteState.highlightedSprite = ThemeImage.GetThemeSprite (DeactivatedSprite);
                buttonSpriteState.pressedSprite = ThemeImage.GetThemeSprite (DeactivatedSprite);
                button.spriteState = buttonSpriteState;
            }
        }
    }
}