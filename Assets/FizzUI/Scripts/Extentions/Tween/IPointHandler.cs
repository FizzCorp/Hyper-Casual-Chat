using UnityEngine.EventSystems;

namespace Fizz.UI.Tween
{
    public interface IPointHandler :
        IPointerEnterHandler,
        IPointerDownHandler,
        IPointerClickHandler,
        IPointerUpHandler,
        IPointerExitHandler
    {

        new void OnPointerEnter (PointerEventData eventData);
        new void OnPointerDown (PointerEventData eventData);
        new void OnPointerClick (PointerEventData eventData);
        new void OnPointerUp (PointerEventData eventData);
        new void OnPointerExit (PointerEventData eventData);

    }

}