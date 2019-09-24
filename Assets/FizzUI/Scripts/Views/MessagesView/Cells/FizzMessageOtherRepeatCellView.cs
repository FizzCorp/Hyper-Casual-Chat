using System;
using UnityEngine;
using UnityEngine.UI;
using Fizz.UI.Model;

namespace Fizz.UI.Components
{
    /// <summary>
    /// User interface other repeat chat cell view.
    /// </summary>
    public class FizzMessageOtherRepeatCellView : FizzMessageCellView
    {
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

            LoadChatMessageAction();
        }

        #endregion

        #region Methods

        void LoadChatMessageAction()
        {
            MessageLabel.gameObject.SetActive(true);
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