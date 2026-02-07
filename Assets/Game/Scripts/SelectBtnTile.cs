using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBtnTile : SelectButton
{
    public TileSelect _tileSelect;
    protected override void PlaySource(TypeSFX sfx)
    {
        if (!_tileSelect.CanSetDrawer()) return;
        base.PlaySource(sfx);
    }
}
