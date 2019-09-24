using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI
{
    using Components;
    using Extentions;
    using Fizz.UI.Core;

    public class FizzHypercasualInputView : FizzInputView
    {
        [SerializeField] RectTransform PhrasesContainer;
        [SerializeField] RectTransform StickersContainer;
        [SerializeField] RectTransform TabsContainer;

        [SerializeField] Button RecentButton;
        [SerializeField] Slider TimerSlider;

        [SerializeField] RectTransform PhrasePool;
        [SerializeField] RectTransform StickerPool;

        [SerializeField] FizzHypercasualPhraseView PhraseViewPrefab;
        [SerializeField] FizzHypercasualStickerView StickerViewPrefab;
        [SerializeField] FizzHypercasualTagTabView TagTabViewPrefab;

        private IFizzHypercasualInputDataProvider dataProvider;
        private FizzHypercasualTagTabView selectedTab;

        private const float RESEND_TIMER = 1f;
        private float timeSinceLastSend = 0f;

        private void Awake ()
        {
            dataProvider = Registry.PredefinedInputDataProvider;
            AdjustGridSize ();
        }

        private void Update ()
        {
            if (Time.realtimeSinceStartup < timeSinceLastSend + RESEND_TIMER)
            {
                if (!TimerSlider.isActiveAndEnabled) TimerSlider.gameObject.SetActive (true);
                TimerSlider.value = 1 - ((timeSinceLastSend + RESEND_TIMER) - Time.realtimeSinceStartup) / RESEND_TIMER;
            }
            else if (TimerSlider.isActiveAndEnabled)
            {
                TimerSlider.gameObject.SetActive (false);
            }
        }

        protected override void OnEnable ()
        {
            base.OnEnable ();

            RecentButton.onClick.AddListener (OnRecentButtonClicked);
            LoadView ();
        }

        protected override void OnDisable ()
        {
            base.OnDisable ();

            RecentButton.onClick.RemoveListener (OnRecentButtonClicked);
        }

        public override void Reset ()
        {
            base.Reset ();
        }

        private void LoadView ()
        {
            LoadTagTabs ();
        }

        private void LoadTagTabs ()
        {
            TabsContainer.DestroyChildren ();
            List<string> tags = dataProvider.GetAllTags ();

            if (tags.Count == 0) return;

            bool selected = false;
            foreach (string tag in tags)
            {
                FizzHypercasualTagTabView tagView = Instantiate (TagTabViewPrefab);
                tagView.gameObject.SetActive (true);
                tagView.transform.SetParent (TabsContainer, false);
                tagView.transform.localScale = Vector3.one;
                tagView.SetTag (tag);
                tagView.OnTabClick = OnTagTabSelected;

                if (!selected)
                {
                    OnTagTabSelected (tagView);
                    selected = true;
                }
            }
        }

        private void OnTagTabSelected (FizzHypercasualTagTabView tab)
        {
            if (selectedTab != null && !selectedTab.Tag.Equals (tab.Tag))
            {
                selectedTab.SetSelected (false);
            }

            SetRecentSelected (false);

            selectedTab = tab;
            selectedTab.SetSelected (true);

            LoadPhrases ();
            LoadStickers ();
        }

        private void LoadPhrases (bool loadRecent = false)
        {
            ReturnPhraseViewToPool ();
            if (!loadRecent && selectedTab == null) return;
            List<string> phrases = loadRecent ? dataProvider.GetRecentPhrases () : dataProvider.GetAllPhrases (selectedTab.Tag);
            if (phrases.Count == 0) return;
            
            foreach (string id in phrases)
            {
                FizzHypercasualDataItem phraseItem = dataProvider.GetPhrase (id);
                if (phraseItem == null) continue;

                FizzHypercasualPhraseView phraseView = GetPhraseViewFromPool ();
                phraseView.gameObject.SetActive (true);
                phraseView.transform.SetParent (PhrasesContainer, false);
                phraseView.transform.localScale = Vector3.one;
                phraseView.SetPhraseData (phraseItem);
                phraseView.OnPhraseClick = OnPhraseClicked;
            }
        }

        private void OnPhraseClicked (FizzHypercasualPhraseView phraseView)
        {
            if (phraseView == null) return;
            if (Time.realtimeSinceStartup < timeSinceLastSend + RESEND_TIMER) return;

            dataProvider.AddPhraseToRecent (phraseView.PhraseData.Id);
            timeSinceLastSend = Time.realtimeSinceStartup;

            if (OnSendData != null)
            {
                Dictionary<string, string> phraseData = new Dictionary<string, string> ();
                phraseData.Add ("type", "fizz_predefine_phrase");
                phraseData.Add ("phrase_id", phraseView.PhraseData.Id);
                OnSendData.Invoke (phraseData);
            }
        }

        private void LoadStickers (bool loadRecent = false)
        {
            ReturnStickerViewToPool ();
            if (!loadRecent && selectedTab == null) return;
            List<string> stickers = loadRecent ? dataProvider.GetRecentStickers () : dataProvider.GetAllStickers (selectedTab.Tag);
            if (stickers.Count == 0) return;

            foreach (string id in stickers)
            {
                FizzHypercasualDataItem stickerItem = dataProvider.GetSticker (id);
                if (stickerItem == null) continue;

                FizzHypercasualStickerView stickerView = GetStickerViewFromPool ();
                stickerView.gameObject.SetActive (true);
                stickerView.transform.SetParent (StickersContainer, false);
                stickerView.transform.localScale = Vector3.one;
                stickerView.SetStickerData (stickerItem);
                stickerView.OnStickerClick = OnStickerClicked;
            }
        }

        private void OnStickerClicked (FizzHypercasualStickerView stickerView)
        {
            if (stickerView == null) return;
            if (Time.realtimeSinceStartup < timeSinceLastSend + RESEND_TIMER) return;

            dataProvider.AddStickerToRecent (stickerView.StickerData.Id);
            timeSinceLastSend = Time.realtimeSinceStartup;

            if (OnSendData != null)
            {
                Dictionary<string, string> stickerData = new Dictionary<string, string> ();
                stickerData.Add ("type", "fizz_predefine_sticker");
                stickerData.Add ("sticker_id", stickerView.StickerData.Id);
                OnSendData.Invoke (stickerData);
            }
        }

        private void OnRecentButtonClicked ()
        {
            if (selectedTab != null)
            {
                selectedTab.SetSelected (false);
                selectedTab = null;
            }

            SetRecentSelected (true);

            LoadPhrases (true);
            LoadStickers (true);
        }

        private void SetRecentSelected (bool selected)
        {
            RecentButton.targetGraphic.enabled = selected;
        }

        private FizzHypercasualStickerView GetStickerViewFromPool ()
        {
            if (StickerPool.childCount > 0)
            {
                return StickerPool.GetChild (0).GetComponent<FizzHypercasualStickerView> ();
            }

            FizzHypercasualStickerView stickerView = Instantiate (StickerViewPrefab);
            stickerView.transform.SetParent (StickerPool, false);

            return stickerView;
        }

        private void ReturnStickerViewToPool ()
        {
            int childCount = StickersContainer.childCount;
            for (int index = 0; index < childCount; index++)
            {
                StickersContainer.GetChild (0).SetParent (StickerPool, false);
            }
        }

        private FizzHypercasualPhraseView GetPhraseViewFromPool ()
        {
            if (PhrasePool.childCount > 0)
            {
                return PhrasePool.GetChild (0).GetComponent<FizzHypercasualPhraseView> ();
            }

            FizzHypercasualPhraseView phraseView = Instantiate (PhraseViewPrefab);
            phraseView.transform.SetParent (PhrasePool, false);

            return phraseView;
        }

        private void ReturnPhraseViewToPool ()
        {
            int childCount = PhrasesContainer.childCount;
            for (int index = 0; index < childCount; index++)
            {
                PhrasesContainer.GetChild (0).SetParent (PhrasePool, false);
            }
        }

        private void AdjustGridSize ()
        {
            //Tags
            GridLayoutGroup TabGrid = TabsContainer.GetComponent<GridLayoutGroup> ();
            TabGrid.cellSize = new Vector2 (
                (TabsContainer.rect.width - TabGrid.padding.left - TabGrid.padding.right - (TabGrid.spacing.x * 3)) / 4,
                TabGrid.cellSize.y);
            //Phrases
            GridLayoutGroup PhrasesGrid = PhrasesContainer.GetComponent<GridLayoutGroup> ();
            PhrasesGrid.cellSize = new Vector2 (
                (PhrasesContainer.rect.width - PhrasesGrid.padding.left - PhrasesGrid.padding.right - (PhrasesGrid.spacing.x * 2)) / 3,
                (PhrasesContainer.rect.height - PhrasesGrid.padding.top - PhrasesGrid.padding.bottom - (PhrasesGrid.spacing.y * 2)) / 3);
            //Stickers
            GridLayoutGroup stickerGrid = StickersContainer.GetComponent<GridLayoutGroup> ();
            stickerGrid.cellSize = new Vector2 (
                (StickersContainer.rect.width - stickerGrid.padding.left - stickerGrid.padding.right - (stickerGrid.spacing.x * 4)) / 5,
                (StickersContainer.rect.width - stickerGrid.padding.left - stickerGrid.padding.right - (stickerGrid.spacing.x * 4)) / 5);
        }
    }
}
