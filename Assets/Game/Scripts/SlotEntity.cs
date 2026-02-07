using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotEntity : MonoBehaviour
{
    public Dice[] _health;
    [Range(1, 10)] public int _healthDices = 1;
    public int _maxHealthDices;
    public Dice _currentHealthDice;
    public TypeEnemy _typeEnemy = TypeEnemy.CHILD;
    public TypeDefense _typeDefense = TypeDefense.ENDURANCE;
    public ProtectionValues _defValues;
    public CanvasGroup _canvasGroup;
    public Image _imgDefense;
    public GameObject _goEntity;
    public SelectEntity _entity;
    protected virtual void SetHealthValues()
    {
        ManagerGame.Instance.SetCurrentHealthDices(_health, this);
    }
    //public virtual void SetDefenseValues(int value)
    public virtual void SetDefenseValues(TypeDefense value)
    {
        //_typeDefense = (TypeDefense)value;
        _typeDefense = value;
        _imgDefense.sprite = _defValues._defValues[(int)value]._sprites;
    }
}
