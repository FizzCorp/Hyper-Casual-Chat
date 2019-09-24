using Fizz.UI.Model;

namespace Fizz.UI.Components
{
    public class FizzMessageDateHeaderCellView : FizzMessageCellView
    {
        #region Public Methods

        public override void SetData(FizzMessageCellModel model, bool appTranslationEnabled)
        {
            base.SetData(model, appTranslationEnabled);

            MessageLabel.text = Utils.GetFormattedTimeForUnixTimeStamp(_model.Created);
        }

        #endregion
    }
}