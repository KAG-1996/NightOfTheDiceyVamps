using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotEnemy : SlotEntity
{
    [Header("Stats")]
    public StatsEnemy _stats;
    public Image _imgFace;
    public Dice _slotAttack, _slotDefense, _slotDamage;
    public void SetStats(StatsEnemy stats)
    {
        _stats = stats;
        _imgFace.sprite = _stats._sprite;
        _healthDices = _maxHealthDices = _stats._healthDices;
        SetHealthValues();
        //SetDefenseValues((int)_stats._typeDefense);
        SetDefenseValues(_stats._typeDefenses[Random.Range(0, _stats._typeDefenses.Length)]);
    }
}
