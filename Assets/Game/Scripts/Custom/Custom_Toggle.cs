using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Custom_Toggle : Toggle
{
    ManagerInputCalls InputCall => ManagerInputCalls.Instance;

    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        InputCall.UpdateSelected(gameObject);
    }
}
