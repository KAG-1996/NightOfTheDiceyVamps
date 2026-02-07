using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ManagerInfoScroll : MonoBehaviour
{
    public int currentDescriptor = 0;
    public ScrollRect _srInfo;
    public TextMeshProUGUI _txtInfo;
    public LocalizeStringEvent _locEvent;
    public DescriptionContent _dcPrefab;
    public InfoContent[] _infoConts;
    private void Start()
    {
        UpdatePage();
    }
    public void SetDescriptionPanels(object o)
    {
        foreach (var a in _infoConts)
        {
            switch (a._name)
            {
                case "Protecciones":
                    if (o is ProtectionValues pv)
                    {
                        for (int i = 0; i < pv._defValues.Length; i++)
                        {
                            DescriptionContent desc = Instantiate(_dcPrefab, a._pnlDescriptor.transform);
                            desc._img.sprite = pv._defValues[i]._sprites;
                            desc._locEvent.StringReference = pv._defValues[i]._locString;
                        }
                    }
                    break;
                case "Dados":
                    if (o is DiceClass dc)
                    {
                        for (int i = 0; i < dc._values.Length; i++)
                        {
                            DescriptionContent desc = Instantiate(_dcPrefab, a._pnlDescriptor.transform);
                            desc._img.sprite = dc._values[i]._sprite;
                            desc._locEvent.StringReference = dc._values[i]._locString;
                        }
                    }
                    break;
                case "Casillas":
                    if (o is TileValues tv)
                    {
                        for (int i = 1; i < tv._tileValues.Length - 1; i++)
                        {
                            DescriptionContent desc = Instantiate(_dcPrefab, a._pnlDescriptor.transform);
                            desc._img.sprite = tv._tileValues[i]._sprites;
                            desc._locEvent.StringReference = tv._tileValues[i]._locString;
                        }
                    }
                    break;
            }
        }
    }
    public void CGSets(CanvasGroup cg, bool active)
    {
        cg.alpha = active ? 1 : 0;
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }
    public void Z_ChangePage(bool forward)
    {
        currentDescriptor += forward ? 1 : -1;
        if (forward)
            currentDescriptor = currentDescriptor == _infoConts.Length ? 0 : currentDescriptor;
        else
            currentDescriptor = currentDescriptor < 0 ? _infoConts.Length - 1 : currentDescriptor;
        UpdatePage();
    }
    private void UpdatePage()
    {
        foreach (var a in _infoConts) CGSets(a._cgDesc, false);
        _srInfo.content = _infoConts[currentDescriptor]._pnlDescriptor.GetComponent<RectTransform>();
        CGSets(_infoConts[currentDescriptor]._cgDesc, true);
        _locEvent.StringReference = _infoConts[currentDescriptor]._locString;
    }
}
