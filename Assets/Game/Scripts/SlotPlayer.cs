using UnityEngine;

public class SlotPlayer : SlotEntity
{
    public Dice[] _energy;
    public Dice _slotProtection;
    [Range(1, 10)] public int _energyDices = 1;
    public int _maxEnergyDices = 1;
    public Dice _currentEnergyDice;

    SaveData Data => ManagerData.Instance._saveData;
    public void SetStats(int health = -1, int energy = -1, int defense = -1)
    {
        _healthDices = health == -1 ? _maxHealthDices = Data._playerData._dicesHealth : health; 
        _energyDices = energy == -1 ? _maxEnergyDices = Data._playerData._dicesEnergy : energy;
        SetHealthValues(); 
        SetEnergyValues();
        SetDefenseValues(defense == -1 ? (TypeDefense)Data._playerData._defense : (TypeDefense)defense);
    }
    protected virtual void SetEnergyValues() => ManagerGame.Instance.SetCurrentEnergyDices(_energy, this); 
}
