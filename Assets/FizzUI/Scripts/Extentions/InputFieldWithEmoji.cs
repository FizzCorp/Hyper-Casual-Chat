//
//  InputFieldWithEmoji.cs
//
//  Copyright (c) 2016 Fizz Inc
//
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

namespace Fizz.UI.Components
{
	public class InputFieldWithEmoji : InputField
	{
		private TextWithEmoji _emojiText;
		private bool _checkEmoji = true;

		public UnityEvent onSelect;
		public UnityEvent onDeselect;
		public UnityEvent onDone;

		public bool CheckEmoji {
			get {
				return this._checkEmoji;
			}
			set {
				_checkEmoji = value;
			}
		}

		protected override void Awake ()
		{
			base.Awake ();
			_emojiText = gameObject.GetComponentInChildren <TextWithEmoji> ();
		}

		public override void OnSelect (UnityEngine.EventSystems.BaseEventData eventData)
		{
			base.OnSelect (eventData);
			if (_checkEmoji) {
				this.textComponent.gameObject.SetActive (true);
				GetEmojiText ().gameObject.SetActive (false);
			}
			onSelect.Invoke ();
		}

		public override void OnDeselect (UnityEngine.EventSystems.BaseEventData eventData)
		{
			base.OnDeselect (eventData);
			if (_checkEmoji) {
				ForceUpdate ();
			}
			onDeselect.Invoke ();
		}

		public void ForceUpdate ()
		{
			if (_checkEmoji) {
				GetEmojiText ().gameObject.SetActive (true);
				GetEmojiText ().text = text;
				this.textComponent.text = text;
				this.textComponent.gameObject.SetActive (false);
				UpdateLabel ();
			}
		}

		public void ResetText ()
		{
			text = string.Empty;
			if (_checkEmoji) {
				this.textComponent.gameObject.SetActive (true);
				this.textComponent.text = string.Empty;
				GetEmojiText ().text = string.Empty;
				// GetEmojiText ().ResetText ();
				GetEmojiText ().gameObject.SetActive (false);
			}
			UpdateLabel ();
		}

		private TextWithEmoji GetEmojiText ()
		{
			if (_emojiText == null) {

				foreach (Transform child in transform) {
					if (child.GetComponent<TextWithEmoji> () != null) {
						_emojiText = child.GetComponent<TextWithEmoji> ();
						break;
					}
				}
			}
			return _emojiText;
		}

		void Update ()
		{
			if (m_Keyboard != null && m_Keyboard.status == TouchScreenKeyboard.Status.Done) {
				if (onDone != null) {
					onDone.Invoke ();
					m_Keyboard = null;
				}
			}
		}
	}
}