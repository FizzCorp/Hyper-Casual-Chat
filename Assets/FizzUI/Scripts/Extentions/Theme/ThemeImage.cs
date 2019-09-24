using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Extentions
{
    public class ThemeImage : MonoBehaviour
    {
        [SerializeField] ThemeSprite Sprite;

        private void Awake ()
        {
            if (FizzTheme.FizzThemeData == null) return;

            Image image = gameObject.GetComponent<Image> ();
            if (image != null)
            {
                image.sprite = GetThemeSprite (Sprite);
            }
        }

        public Sprite GetThemeSprite (ThemeSprite tSprite)
        {
            switch (tSprite)
            {
                case ThemeSprite.ChannelActivatedButton:
                    return FizzTheme.FizzThemeData.ChannelActivatedButton;
                case ThemeSprite.ChannelDeactivatedButton:
                    return FizzTheme.FizzThemeData.ChannelDeactivatedButton;
                case ThemeSprite.ChannelBackground:
                    return FizzTheme.FizzThemeData.ChannelBackground;
                case ThemeSprite.ChannelBackgroundGlow:
                    return FizzTheme.FizzThemeData.ChannelBackgroundGlow;
                case ThemeSprite.ChannelHeader:
                    return FizzTheme.FizzThemeData.ChannelHeader;
                case ThemeSprite.ChannelFooter:
                    return FizzTheme.FizzThemeData.ChannelFooter;
                case ThemeSprite.CloseButton:
                    return FizzTheme.FizzThemeData.CloseButton;
                case ThemeSprite.SendButton:
                    return FizzTheme.FizzThemeData.SendButton;
                case ThemeSprite.MoreButton:
                    return FizzTheme.FizzThemeData.MoreButton;
                case ThemeSprite.DeliveryStatusIcon:
                    return FizzTheme.FizzThemeData.DeliveryStatusIcon;
                case ThemeSprite.TranslationIcon:
                    return FizzTheme.FizzThemeData.TranslationIcon;
                case ThemeSprite.LatestPageIcon:
                    return FizzTheme.FizzThemeData.LatestPageIcon;
                case ThemeSprite.HeaderActivatedButton:
                    return FizzTheme.FizzThemeData.HeaderActivatedButton;
                case ThemeSprite.HeaderDeactivatedButton:
                    return FizzTheme.FizzThemeData.HeaderDeactivatedButton;
                case ThemeSprite.HeaderTabsBackground:
                    return FizzTheme.FizzThemeData.HeaderTabsBackground;
                case ThemeSprite.HeaderTabsBackgroundGlow:
                    return FizzTheme.FizzThemeData.HeaderTabsBackgroundGlow;
                case ThemeSprite.LoadHistoryIcon:
                    return FizzTheme.FizzThemeData.LoadHistoryIcon;
                case ThemeSprite.MessagesBackground:
                    return FizzTheme.FizzThemeData.MessagesBackground;
                case ThemeSprite.OtherChatBackground:
                    return FizzTheme.FizzThemeData.OtherChatBackground;
                case ThemeSprite.OwnChatCellBackground:
                    return FizzTheme.FizzThemeData.OwnChatCellBackground;
                case ThemeSprite.MessageInputBackground:
                    return FizzTheme.FizzThemeData.MessageInputBackground;
                case ThemeSprite.RecentIcon:
                    return FizzTheme.FizzThemeData.RecentIcon;
                default:
                    return FizzTheme.FizzThemeData.ChannelBackground;
            }
        }
    }
}