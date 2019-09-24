using UnityEngine;

namespace Fizz.UI.Extentions
{
    [CreateAssetMenu (menuName = "Fizz/Theme Data")]
    public class FizzThemeData : ScriptableObject
    {
        //Sprites
        public Sprite ChannelActivatedButton;
        public Sprite ChannelDeactivatedButton;
        public Sprite ChannelBackground;
        public Sprite ChannelBackgroundGlow;
        public Sprite ChannelHeader;
        public Sprite ChannelFooter;
        public Sprite CloseButton;
        public Sprite SendButton;
        public Sprite MoreButton;
        public Sprite DeliveryStatusIcon;
        public Sprite TranslationIcon;
        public Sprite LatestPageIcon;
        public Sprite HeaderActivatedButton;
        public Sprite HeaderDeactivatedButton;
        public Sprite HeaderTabsBackground;
        public Sprite HeaderTabsBackgroundGlow;
        public Sprite LoadHistoryIcon;
        public Sprite MessagesBackground;
        public Sprite OtherChatBackground;
        public Sprite OwnChatCellBackground;
        public Sprite MessageInputBackground;
        public Sprite RecentIcon;

        //Fonts
        public Font BoldFont;
        public Font NormalFont;

        //Colors
        public Color Primary;
        public Color Secondary;
        public Color Base_1;
        public Color Base_2;
    }

    public enum ThemeSprite
    {
        ChannelActivatedButton,
        ChannelDeactivatedButton,
        ChannelBackground,
        ChannelBackgroundGlow,
        ChannelHeader,
        ChannelFooter,
        CloseButton,
        SendButton,
        MoreButton,
        DeliveryStatusIcon,
        TranslationIcon,
        LatestPageIcon,
        HeaderActivatedButton,
        HeaderDeactivatedButton,
        HeaderTabsBackground,
        HeaderTabsBackgroundGlow,
        LoadHistoryIcon,
        MessagesBackground,
        OtherChatBackground,
        OwnChatCellBackground,
        MessageInputBackground,
        RecentIcon
    }

    public enum ThemeFont
    {
        Bold,
        Normal
    }

    public enum ThemeColor
    {
        Primary,
        Secondary,
        Base_1,
        Base_2
    }
}