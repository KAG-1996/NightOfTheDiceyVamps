using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionEntity : HoverSelect
{
    public SelectEntity _entity;
    ManagerGame ManagerGame => ManagerGame.Instance;
    public override void OnSelect(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        _entity.Selector(true);
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        _entity.Selector(false);
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        if (ManagerGame._phase != Phases.PLANNING) return;
        _entity.Click();
    }
}
