using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{
    public AudioSource _source;
    public bool _overPlay = false;
    ManagerAudio ManagerAudio => ManagerAudio.Instance;
    public virtual void OnPointerEnter(PointerEventData eventData) => PlaySource(TypeSFX.NAV);
    public virtual void OnSelect(BaseEventData eventData) => PlaySource(TypeSFX.NAV);
    public virtual void OnPointerClick(PointerEventData eventData) => PlaySource(TypeSFX.SUBMIT);
    public virtual void OnSubmit(BaseEventData eventData) => PlaySource(TypeSFX.SUBMIT);
    protected virtual void PlaySource(TypeSFX sfx)
    {
        if (_source) ManagerAudio.PlaySFX(_source, sfx, _overPlay);
        else ManagerAudio.PlaySFX(sfx, _overPlay);
    }
}
