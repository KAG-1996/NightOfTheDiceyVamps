using UnityEngine;
using UnityEngine.EventSystems; // Required for IPointerEnterHandler and IPointerExitHandler

public class HoverSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Color _colorEnter, _colorExit;
    public Renderer _renderer;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse Click on: " + gameObject.name);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _renderer.material.color = _colorEnter;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _renderer.material.color = _colorExit;
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        _renderer.material.color = _colorEnter;
    }
    public virtual void OnDeselect(BaseEventData eventData)
    {
        _renderer.material.color = _colorExit;
    }
    public virtual void OnSubmit(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}