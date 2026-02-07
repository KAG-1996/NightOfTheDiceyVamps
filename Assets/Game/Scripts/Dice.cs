using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dice : MonoBehaviour, IPointerClickHandler
{
    public DiceType _typeDice;
    public DamageType _damageType;
    public Image _imgDice, _imgType;
    public Button _btnDice;
    public TextMeshProUGUI _txtEnergy;
    public Animator _anim;
    public DiceValues _diceValues;
    public DiceClass _diceClass;

    public int _currentValue = 6, _energyReq = 0;
    public bool _diceRolling;
    Dice CurrentDice => ManagerGame.Instance._currentDiceSelected;
    ManagerGame ManagerGame => ManagerGame.Instance;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;

    public Dice _diceTaken;

    [ContextMenu("ROLL")]
    public void Roll(bool roll)
    {
        _diceRolling = roll;
        if (_btnDice) _btnDice.enabled = !_diceRolling;
    }
    void Update()
    {
        if (!_diceRolling) return;
        DiceRoll();
    }
    void DiceRoll()
    {
        int rnd = UnityEngine.Random.Range(1, _diceValues._values.Length);
        DiceValue(rnd);
    }
    public void DiceValue(int value)
    {
        _currentValue = value > 0 ? value : 0;
        _imgDice.sprite = _diceValues._values[_currentValue]._sprite;
    }
    public void SetDiceType(DamageType dmg)
    {
        _damageType = _diceClass._values[(int)dmg]._type;
        _imgType.sprite = _diceClass._values[(int)dmg]._sprite;
        _energyReq = _diceClass._values[(int)dmg]._energyReq;
        _txtEnergy?.SetText($"{_energyReq}");
    }
    public void SetDiceReq(TypeSkill dmg)
    {
        _energyReq = _diceValues._values[(int)dmg]._energyReq;
        _txtEnergy?.SetText($"{_energyReq}");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_typeDice != DiceType.DECK) return;
        ManagerGame._eventData = eventData;
    }
    public void GiveValues(Dice dice = null)
    {
        DiceValue(dice ? dice._currentValue : 0);
        _damageType = dice ? dice._damageType : DamageType.NORMAL;
        _imgType.sprite = dice ? dice._imgType.sprite : _diceClass._values[0]._sprite;
    }
    public void Z_ApplySkill()
    {
        if (ManagerGame._skillActive != TypeSkill.NONE) return;
        ManagerGame.SkillUse((TypeSkill)_currentValue, this);
    }
    public void Z_TakeDeckValue()
    {
        if (!ManagerGame.HaveEnergyReq(_energyReq)) return;
        ManagerGame._diceDrag.GiveValues(this);
        ManagerInputCall.UpdateSelected(ManagerGame._goBtnPlayer);
        ManagerGame.SetDiceSelected(this);
    }
    public void Z_GiveDeckValue()
    {
        if (_currentValue == 0 && CurrentDice) //Set Dice to Entity
        {
            if (CurrentDice._energyReq > 0)
            {
                if (ManagerGame.HaveEnergyReq(CurrentDice._energyReq))
                    ManagerGame.DrainEnergy(CurrentDice._energyReq);
                else return;
            }
            DiceValue(CurrentDice._currentValue);
            _diceTaken = CurrentDice;
            _diceTaken.gameObject.SetActive(false);
            _damageType = _diceTaken._damageType;
            _imgType.sprite = _diceTaken._imgType.sprite;
            ManagerGame._dicesPlayed.Remove((int)CurrentDice._damageType);
            ManagerGame.SetDiceSelected();
        }
        else if (_currentValue != 0 && CurrentDice) //Exchange set dice
        {
            if (CurrentDice._energyReq > 0) 
                ManagerGame.DrainEnergy(CurrentDice._energyReq);
            if (_diceTaken._energyReq > 0)
                ManagerGame.RestoreEnergy(_diceTaken._energyReq);
            ManagerGame._dicesPlayed.Add((int)_damageType);
            DiceValue(CurrentDice._currentValue);
            ManagerGame._dicesPlayed.Remove((int)CurrentDice._damageType);
            _damageType = CurrentDice._damageType;
            _imgType.sprite = CurrentDice._imgType.sprite;
            _diceTaken.gameObject.SetActive(true);
            _diceTaken = CurrentDice;
            _diceTaken.gameObject.SetActive(false);
            ManagerGame.SetDiceSelected();
        }
        else if (_diceTaken) //Retrive dice from entity
        {
            if (_diceTaken._energyReq > 0)
            {
                ManagerGame.RestoreEnergy(_diceTaken._energyReq);
            }
            ManagerGame._dicesPlayed.Add((int)_damageType);
            DiceValue(0);
            _damageType = DamageType.NORMAL;
            _imgType.sprite = _diceClass._values[0]._sprite;
            _diceTaken.gameObject.SetActive(true);
            _diceTaken = null;
            ManagerGame._diceDrag.gameObject.SetActive(false);
            ManagerGame._diceDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 1000f;
        }
    }
}
