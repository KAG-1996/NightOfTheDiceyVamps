using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTile : HoverSelect
{
    public TileSelect _tileSelect;
    public GameObject _bnt;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;
    ManagerScenes ManagerScene => ManagerScenes.Instance;
    public void SetSelected(bool active)
    {
        if (!_tileSelect.CanSetDrawer()) return;
        DeActiveBtn(active);
        ManagerInputCall.UpdateSelected(active ? _bnt : null);
    }
    public void DeActiveBtn(bool active) => _bnt.SetActive(active);
    public override void OnSelect(BaseEventData eventData)
    {
        if (!_tileSelect.CanSetDrawer()) return;
        MapGenerator.Instance._currentGoTile = gameObject;
        _renderer.material.color = _colorEnter;
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        if (!_tileSelect.CanSetDrawer()) return;
        _renderer.material.color = _colorExit;
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        if (ManagerScene._phase != PhaseLoading.NONE) return;
        if (!_tileSelect.CanSetDrawer()) return;
        _tileSelect.SetValues();
        gameObject.SetActive(false);
    }
}
