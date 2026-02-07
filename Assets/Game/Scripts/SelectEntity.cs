using UnityEngine;
using UnityEngine.EventSystems;

public class SelectEntity : HoverSelect
{
    public float _yOffset = 2.5f;
    public BoxCollider _col3D;
    public Animator _anim;
    public ParticleSystem _psDamage, _psBlock;
    public Dice _dice;
    public GameObject _goNav;
    ManagerGame ManagerGame => ManagerGame.Instance;
    public void Click()
    {
        if (_dice == null) ZDebug.Log("NUll");
        _dice.Z_GiveDeckValue();
        ManagerGame.SetDices();
        ManagerGame._diceDrag.GiveValues();
    }
    public void Selector(bool active)
    {
        ManagerGame._goSelector.SetActive(active);
        ManagerGame._diceDrag.gameObject.SetActive(ManagerGame._diceDrag._currentValue > 0);
        ManagerGame._goSelector.transform.position =
            new Vector3(transform.position.x, _col3D.bounds.size.y + _yOffset, transform.position.z);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Click();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Selector(true);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Selector(false);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Selector(true);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Selector(false);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        Click();
    }
}
