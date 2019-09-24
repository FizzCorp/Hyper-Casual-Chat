using System;
using Fizz.UI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Components
{
    /// <summary>
    /// User interface other chat cell view.
    /// </summary>
    public class FizzMessageOtherCellView : FizzMessageCellView
    {
        /// <summary>
        /// Sender nick name label.
        /// </summary>
        [SerializeField] TextWithEmoji NickLabel;
        /// <summary>
        /// The translate toggle node.
        /// </summary>
        [SerializeField] RectTransform TranslateToggleNode;
        /// <summary>
        /// The translate toggle.
        /// </summary>
        [SerializeField] Button TranslateToggle;

        public Action<int> OnTranslateToggle { get; set; }

        private FizzTranslationToggleComponent translateToggleImage;

        void Awake()
        {
            translateToggleImage = TranslateToggle.GetComponent<FizzTranslationToggleComponent>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TranslateToggle.onClick.AddListener(ToggleTranslateClicked);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TranslateToggle.onClick.RemoveListener(ToggleTranslateClicked);
        }

        #region Public Methods


        public override void SetData(FizzMessageCellModel model, bool appTranslationEnabled)
        {
            base.SetData(model, appTranslationEnabled);

            NickLabel.text = _model.Nick;
            //NickLabel.color = Utils.GetUserNickColor(_model.From);

            LoadChatMessageAction();
        }

        #endregion

        #region Methods

        void LoadChatMessageAction()
        {
            MessageLabel.gameObject.SetActive(true);

            NickLabel.text = _model.Nick;

            MessageLabel.text = _model.GetActiveMessage();

            bool showTranslationToggle = _appTranslationEnabled;

            translateToggleImage.Configure(_model.TranslationState);

            TranslateToggleNode.gameObject.SetActive(showTranslationToggle);
        }

        void ToggleTranslateClicked()
        {
            _model.ToggleTranslationState();
            MessageLabel.text = _model.GetActiveMessage();
            OnTranslateToggle.Invoke(rowNumber);
        }

        #endregion
    }
}