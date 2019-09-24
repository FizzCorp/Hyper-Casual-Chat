using Fizz.UI.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fizz.UI.Components
{
	/// <summary>
	/// User interface own repeat chat cell view.
	/// </summary>
	public class FizzMessageUserRepeatCellView : FizzMessageCellView
	{
		/// <summary>
		/// Chat message delivery status image.
		/// </summary>
		[SerializeField] Image DeliveryStatusImage;
		/// <summary>
		/// Chat message sent status image.
		/// </summary>
		[SerializeField] Image SentStatusImage;

		#region Public Methods

		public override void SetData (FizzMessageCellModel model, bool appTranslationEnabled)
		{
			base.SetData (model, appTranslationEnabled);

			LoadChatMessageAction ();
		}

		#endregion

		#region Private Methods

		private void LoadChatMessageAction () {
			MessageLabel.gameObject.SetActive (true);
            MessageLabel.text = _model.Body;
			
			if (_model.DeliveryState == FizzChatCellDeliveryState.Pending) {
				SentStatusImage.gameObject.SetActive (false);
				DeliveryStatusImage.gameObject.SetActive (false);
			} else if (_model.DeliveryState == FizzChatCellDeliveryState.Sent) {
				SentStatusImage.gameObject.SetActive (true);
				DeliveryStatusImage.gameObject.SetActive (false);
			} else if (_model.DeliveryState == FizzChatCellDeliveryState.Published) {
				SentStatusImage.gameObject.SetActive (false);
				DeliveryStatusImage.gameObject.SetActive (true);
			}
		}

		#endregion
	}
		
}
