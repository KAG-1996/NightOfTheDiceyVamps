using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Custom_TMP_Dropdown : TMP_Dropdown
{
    private GameObject _goBlocker;
    private Button _btnBlocker;

    ManagerInputCalls InputCall => ManagerInputCalls.Instance;
    private void LateUpdate()
    {
        if (!_btnBlocker) return;
        _btnBlocker.interactable = InputCall._input.currentControlScheme != "Gamepad";
    }
    void BlockerNavDisable() //Avoid detection by gamepad
    {
        _btnBlocker = _goBlocker.GetComponent<Button>();
        Navigation customNav = _btnBlocker.navigation;
        customNav.mode = Navigation.Mode.None;
        _btnBlocker.navigation = customNav;
    }
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        /*foreach(var a in m_Items)
        {
            if (a.toggle.isOn)
            {
                InputCall.UpdateSelected(a.gameObject);
                break;
            }
        }*/
    }
    protected override GameObject CreateBlocker(Canvas rootCanvas)
    {
        _goBlocker = base.CreateBlocker(rootCanvas);
        BlockerNavDisable();
        return _goBlocker;
    }
    protected override void DestroyBlocker(GameObject blocker)
    {
        _goBlocker = null;
        _btnBlocker = null;
        base.DestroyBlocker(blocker);
    }
}
