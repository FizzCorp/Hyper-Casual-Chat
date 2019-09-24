using Fizz.Chat;
using UnityEngine;

namespace Fizz.UI
{
	/// <summary>
	/// Data source for chat user interface
	/// </summary>
	public interface IFizzCustomMessageCellViewDataSource
	{
		/// <summary>
		/// Get custom drawable RectTransform for custom message
		/// which will be added to chat cell custom node container
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		RectTransform GetCustomMessageCellViewNode (FizzChannelMessage message);
	}
}