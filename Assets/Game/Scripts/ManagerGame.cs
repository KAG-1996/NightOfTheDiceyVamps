using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using UnityEngine.XR;

public class ManagerGame : MonoBehaviour
{
    public static ManagerGame Instance;

    public Phases _phase;
    public TypeSkill _skillActive = TypeSkill.NONE;
    public TileValues _tileValues;
    public BattleSettings _battleSets;
    public ProtectionValues _protectValues;
    public DiceClass _diceClass;
    public SlotPlayer _player;
    public GameObject _goSelector, _goPlayer;
    public Dice _currentDiceSelected, _diceDrag;
    public Dice[] _deck, _skills;
    public SlotEnemy[] _enemies;
    public GameObject[] _goLayouts, _goGamepadIcons;
    public GameObject _goBossLayout;
    public Scrollbar _sbInfo;
    public ManagerInfoScroll _infoScroll;
    public PointerEventData _eventData;

    public Animator _anim, _animPlayer, _animWaves;
    public int _Reroll, _reShuffle, _waves = 0;
    public GameObject _pnlSkillUse, _pnlInfo, _pnlDefeat;
    public CanvasGroup _cgInfo;
    public TextMeshProUGUI _txtRoll, _txtShuffle, _txtSkillUse, _txtWaves;
    public LocalizeStringEvent _locEventSkill, _locEventWaves;
    public LocalizedString _locWaves;
    public List<int> _shuffleList = new List<int>();
    public List<int> _dicesPlayed = new List<int>();
    public List<int> _deckValues = new List<int>();

    public ReordeEnemies[] _enemyPositions;
    public Transform _tStart, _tEnd, _tGetOut;
    public Transform _canvasWorld, _canvasCamera;
    public GameObject  _goBtnUseSkill, _goBtnPlayer;

    private float Speed = 0.75f;
    private float _speedRun = 20f;

    protected TypeSkill _skillBe;
    protected Dice _goTempDice;

    SaveData Data => ManagerData.Instance._saveData;
    MapGenerator MapGenerator => MapGenerator.Instance;
    ManagerInputCalls InputCall => ManagerInputCalls.Instance;
    ManagerLoadingScreen ManagerLoadingScreen => ManagerLoadingScreen.Instance;
    WaitForSeconds WaitBeforeAttack => new WaitForSeconds(0.5f);
    private void Awake() => Instance = this; 
    private void OnDestroy() => Instance = null; 
    /*[ContextMenu("TDealPlayerDmg")]
    void TDealPlayerDmg() => StartCoroutine(IDealDmg(5, 0, 0, _player));
    [ContextMenu("THealPlayer")]
    void THealPlayer() => StartCoroutine(IHeal(6, _player));
    [ContextMenu("DrainEnergy")]
    void TDrainEnergy() => StartCoroutine(IUseEnergy(5, true));
    [ContextMenu("RestoreEnergy")]
    void TRestoreEnergy() => StartCoroutine(IUseEnergy(5, false));*/

    public static int LastLayout = 0;
    protected virtual IEnumerator Start()
    {
        SetInitials();
        yield return ISetLayout();
        yield return IMovePlayer(_tEnd);
        _animPlayer.SetBool("Iddle", true);
        SetGame();
        _player._entity._goNav.transform.SetParent(_canvasWorld);
        _goBtnPlayer = _player._entity._goNav;
        if (MapGenerator._currentTypeTile == TypeTile.BLOODSHED)
        {
            yield return new WaitUntil(() => _waves != 0);
            yield return IWaves();
        }
    }
    private void LateUpdate()
    {
        foreach (var a in _goGamepadIcons) a.SetActive(InputCall?._input.currentControlScheme == "Gamepad");
    }
    protected void SetInitials()
    {
        //_infoScroll.CGSets(_cgInfo, false);
        _pnlInfo.SetActive(false);
        _pnlSkillUse.SetActive(false);
        InputCall.UpdateSelected(null);
        SetDescriptionPanels();
        foreach (var a in _enemies) a.gameObject.SetActive(false);
        _goPlayer = Instantiate(_goPlayer, _tStart.position, Quaternion.identity);
        _animPlayer = _goPlayer.GetComponent<Animator>();
        _anim.SetTrigger("Hide");
        _animPlayer.SetBool("Iddle", false);
        _player._goEntity = _goPlayer;
        _player._entity = _goPlayer.GetComponent<SelectEntity>();
        _player._entity._dice = _player._slotProtection;
        SetPlayerStats();
    }
    protected void SetDescriptionPanels()
    {
        //_infoScroll._txtInfo.SetText(_infoScroll._infoConts[0]._name); 
        _infoScroll.SetDescriptionPanels(_diceClass);
        _infoScroll.SetDescriptionPanels(_protectValues);
    }
    protected virtual void SetPlayerStats() => _player.SetStats();
    protected virtual void SetGame()
    {
        SetPhase(Phases.PLANNING);
        _txtWaves.SetText(string.Empty);
        _txtWaves.gameObject.SetActive(false);
        CombatBegin();
        RollSkills(); //Skill Dices only roll at the begining of combat
        _anim.SetTrigger("Reveal");
    }
    void CombatBegin()
    {
        NewPhase();
        SetEnemies();
        SetCurrentSkillDices();
    }

    public void SetDices() => StartCoroutine(ISetDices());
    public virtual IEnumerator ISetDices()
    {
        yield return new WaitForEndOfFrame();
        if (ManagerLoadingScreen._pnlExit.activeInHierarchy || _pnlInfo.activeInHierarchy) yield break;
        InputCall.UpdateSelected(SetDiceInHand());
    }
    public virtual void SkillUse(TypeSkill skill, Dice dice)
    {
        _txtSkillUse.SetText(string.Empty);
        InputCall.UpdateSelected(_goBtnUseSkill);
        _skillBe = skill;
        _goTempDice = dice;
        _pnlSkillUse.SetActive(true);
        _locEventSkill.StringReference = dice._diceValues._values[(int)_skillBe]._locStringDesc;
        //_txtSkillUse.SetText($"¿Activar: {dice._diceValues._values[(int)_skillBe]._name}?\n({dice._diceValues._values[(int)_skillBe]._desc})");
    }
    public virtual void NewPhase()
    {
        _anim.SetTrigger("Reveal");
        _skillActive = TypeSkill.NONE;
        _player._slotProtection.DiceValue(0);
        foreach (var enemy in _enemies) enemy._slotDamage.DiceValue(0);
        foreach (var a in _deck) a.gameObject.SetActive(true);
        foreach (var a in _skills) a._btnDice.interactable = true;
        RollDeck();
        RollEnemies();
        SetDeckForShuffle();
        ShuffleDeck();
        _Reroll = Data._playerData._rerolls;
        _reShuffle = Data._playerData._reshuffle;
        _txtRoll.SetText($"{_Reroll}");
        _txtShuffle.SetText($"{_reShuffle}");
    }
    protected virtual void SetDeckForShuffle()
    {
        _shuffleList.Clear();
        _shuffleList = Data._playerData._dicesDeck;
        _dicesPlayed.Clear();
    }
    [ContextMenu("SetDiceDeck")]
    protected virtual void ShuffleDeck()
    {
        SetDiceSelected();
        System.Random random = new();
        List<int> temp = _shuffleList;
        while(temp == _shuffleList)
            _shuffleList = _shuffleList.OrderBy(n => random.Next()).ToList();
        for (int i = 0; i < _deck.Length; i++)
        {
            if (_deck[i].gameObject.activeInHierarchy)
            {
                _deck[i]._anim.SetTrigger("Shuffle");
                _deck[i].SetDiceType((DamageType)_shuffleList[0]);
                _dicesPlayed.Add(_shuffleList[0]);
                _shuffleList.Remove(_shuffleList[0]);
            }
        }
    }
    IEnumerator IShuffle(int i)
    {
        _deck[i]._anim.SetTrigger("Shuffle");
        yield return new WaitForSeconds(0.5f);
        _deck[i].SetDiceType((DamageType)_shuffleList[0]);
    }
    public void SetDiceSelected(Dice dice = null)
    {
        _currentDiceSelected = dice;
        if (!dice) _diceDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 1000f;
        _diceDrag.gameObject.SetActive(dice != null);
    }
    public void SetCurrentHealthDices(Dice[] dices, SlotEntity entity)
    {
        foreach (var a in entity._health) a.gameObject.SetActive(false);
        for (int i = 0; i < entity._healthDices; i++)
        {
            dices[i].gameObject.SetActive(true);
            dices[i].DiceValue(6);
            entity._currentHealthDice = dices[i];
        }
    }
    public void SetCurrentEnergyDices(Dice[] dices, SlotPlayer entity)
    {
        foreach (var a in entity._energy) a.gameObject.SetActive(false);
        for (int i = 0; i < entity._energyDices; i++)
        {
            dices[i].gameObject.SetActive(true);
            dices[i].DiceValue(6);
            entity._currentEnergyDice = dices[i];
        }
    }
    public virtual void SetCurrentSkillDices()
    {
        foreach (var a in _skills) a.gameObject.SetActive(false);
        if (Data._playerData._diceSkills == 0) return;
        for (int i = 0; i < Data._playerData._diceSkills; i++)
            _skills[i].gameObject.SetActive(true);
    }
    protected virtual void SetEnemies()
    {
        foreach (var a in _enemies) a.gameObject.SetActive(false);
        StatsEnemy[] stats = null;
        BattleSets battle = new();
        foreach (var a in MapGenerator._battleSets._battleSets)
        {
            if (MapGenerator._currentLevel >= a._fromLv && 
                MapGenerator._currentLevel <= a._toLv)
            {
                stats = a._enemies;
                battle = a;
                break;
            }
        }
        if (MapGenerator._currentTypeTile == TypeTile.BOSS)
        {
            ZDebug.Log("Boss Fight");
            stats = MapGenerator._battleSets._battleSets[MapGenerator._battleSets._battleSets.Length - 1]._enemies;
            battle = MapGenerator._battleSets._battleSets[MapGenerator._battleSets._battleSets.Length - 1];
        }
        else if (stats == null)
        {
            if (stats == null) ZDebug.Log($"Stats null", HUE.RED);
            return;
        }
        ReordeEnemies re = new();
        foreach (var a in _enemyPositions)
        {
            if (a._tPositions.Length == battle._totalEnemies) 
            {
                re = a;
                break;
            }
        }
        List<StatsEnemy> alreadyStats = new();
        for (int i = 0; i < battle._totalEnemies; i++)
        {
            do _enemies[i].SetStats(stats[UnityEngine.Random.Range(0, stats.Length)]); 
            while (alreadyStats.Contains(_enemies[i]._stats));
            alreadyStats.Add(_enemies[i]._stats);
            //_enemies[i].SetStats(stats[UnityEngine.Random.Range(0, stats.Length)]);
            _enemies[i].gameObject.SetActive(true);
            _enemies[i]._typeEnemy = _enemies[i]._stats._typeEnemy;
            _enemies[i]._goEntity = Instantiate(_enemies[i]._stats._goPrefab, re._tPositions[i].position, Quaternion.identity);
            _enemies[i]._goEntity.transform.SetParent(re._tPositions[i]);
            _enemies[i]._canvasGroup.alpha = 1;
            _enemies[i]._entity = _enemies[i]._goEntity.GetComponent<SelectEntity>();
            _enemies[i]._entity._dice = _enemies[i]._slotDamage;
            _enemies[i]._entity._goNav.transform.SetParent(_canvasWorld);
            StartCoroutine(IRevealFoe(_enemies[i]._entity._renderer.gameObject));
        }
        if (MapGenerator._currentTypeTile == TypeTile.BLOODSHED && _waves == 0)
        {
            _waves = battle._totalWaves;
            _locEventWaves.StringReference = _locWaves;
            //_txtWaves.SetText($"Oleadas Restantes: {_waves}");
            _txtWaves.gameObject.SetActive(true);
            _txtWaves.SetText($"{_txtWaves.text}: {_waves}");
        }
    }
    protected void SetText(TextMeshProUGUI txt, string msg) => txt.SetText(msg); 
    public void SetEnemyStats(bool v)
    {
        foreach (var a in _enemies)
        {
            a._slotAttack.Roll(v);
            a._slotDefense.Roll(v);
        }
    }
    void ChangeDefenses()
    {
        foreach (var a in _enemies)
        {
            if (!a.gameObject.activeInHierarchy) continue;
            int v;
            do v = UnityEngine.Random.Range(0, a._stats._typeDefenses.Length);
            while (a._typeDefense == a._stats._typeDefenses[v]);
            a.SetDefenseValues(a._stats._typeDefenses[v]);
        }
    }
    void SwapValues()
    {
        foreach(var a in _enemies)
        {
            if (!a.gameObject.activeInHierarchy) continue;
            int def = a._slotDefense._currentValue;
            int atk = a._slotAttack._currentValue;
            a._slotDefense.DiceValue(atk);
            a._slotAttack.DiceValue(def);
        }
    }
    public void ActivateSkill(TypeSkill skill)
    {
        _skillActive = skill;
        ZDebug.Log($"{_skillActive} active!");
        switch (skill)
        {
            case TypeSkill.ENEMIES_STATS: SwapValues(); break;
            case TypeSkill.ENEMIES_INDEFENSE: foreach(var a in _enemies) a._slotDefense.DiceValue(0); break;
            case TypeSkill.ENEMIES_DEFENSES: ChangeDefenses(); break;
            case TypeSkill.PLAYER_HEAL: StartCoroutine(IHeal(6, _player)); break;
            case TypeSkill.PLAYER_RESTORE: StartCoroutine(IUseEnergy(6, false)); break;
        }
        foreach (var a in _skills) a._btnDice.interactable = false;
    }
    protected virtual void ClearData()
    {
        Data._mapData.Clear();
        Data._playerData = new();
        ManagerData.Instance.Save();
    }
    public virtual void ShowInfo()
    {
        if (_phase != Phases.PLANNING) return;
        if (ManagerLoadingScreen._pnlExit.activeInHierarchy || _pnlSkillUse.activeInHierarchy) return;
        _pnlInfo.SetActive(!_pnlInfo.activeInHierarchy);
        InputCall.UpdateSelected(_pnlInfo.activeInHierarchy ? _sbInfo.gameObject : SetDiceInHand());
    }
    void BlockAttack(SlotEntity receiver)
    {
        InputCall.TriggerRumble(TypeRumble.BLOCK);
        receiver._entity._psBlock.Play();
        ManagerAudio.Instance.PlaySFX(TypeSFX.BLOCK);
    }
    void UnSelect()
    {
        _goSelector.SetActive(false);
        InputCall.UpdateSelected(null);
        _diceDrag.DiceValue(0);
    }
    void ReturnToMap()
    {
        MapGenerator._currentLevel++;
        Data._playerData._currentLevel = MapGenerator._currentLevel;
        Data._playerData._currentTile = MapGenerator._currentTile;
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.COMBAT, false);
        ManagerData.Instance.Save();
    }
    void CheckMatchDices()
    {
        _deckValues.Clear();
        foreach (var d in _deck) if (d.gameObject.activeInHierarchy) _deckValues.Add(d._currentValue);
        var counts = _deckValues.GroupBy(f => f).Select(group => new { Value = group.Key, Count = group.Count() });
        foreach (var item in counts)
        {
            if (item.Count > 2)
            {
                ZDebug.Log($"Value: {item.Value}, must restore energy by: {item.Count}");
                StartCoroutine(IUseEnergy(item.Count, false));
            }
        }
    }
    bool AreEnemiesDefeated()
    {
        bool defeat = true;
        foreach (var vamp in _enemies)
        {
            if (vamp.isActiveAndEnabled)
            {
                defeat = false;
                break;
            }
        }
        return defeat;
    }
    int TotalEnergy()
    {
        int total = 0;
        foreach (var a in _player._energy) total += a._currentValue;
        return total;
    }
    int ValueFlip(int v)
    {
        switch (v)
        {
            default: return 0;
            case 1: return 6;
            case 2: return 5;
            case 3: return 4;
            case 4: return 3;
            case 5: return 2;
            case 6: return 1;
        }
    }
    GameObject SetDiceInHand()
    {
        foreach (var a in _deck)
        {
            if (a.gameObject.activeInHierarchy)
            {
                if (InputCall._input.currentControlScheme == "Gamepad") a._anim.SetTrigger("Selected");
                return a.gameObject;
            }
        }
        return null;
    }
    bool IsHandSelected()
    {
        foreach (var a in _deck)
        {
            if (EventSystem.current.currentSelectedGameObject)
            if (EventSystem.current.currentSelectedGameObject.name == a.name) return true;
        }
        return false;
    }
    bool IsAnyPnlActive()
    {
        return ManagerLoadingScreen._pnlExit.activeInHierarchy ||
            _pnlInfo.activeInHierarchy ||
            _pnlSkillUse.activeInHierarchy;
    }
    public void SetPhase(Phases phase) => _phase = phase;
    public bool HaveEnergyReq(int req) => req <= TotalEnergy();
    public void DrainEnergy(int v) => StartCoroutine(IUseEnergy(v, true));
    public void RestoreEnergy(int v) => StartCoroutine(IUseEnergy(v, false));
    protected void RollDeck(bool reroll = false) => StartCoroutine(IRoll(TypeRoll.DECK, reroll));
    protected void RollSkills() => StartCoroutine(IRoll(TypeRoll.SKILLS));
    protected void RollEnemies() => StartCoroutine(IRoll(TypeRoll.ENEMY));
    int GetRndEnumLength(Array a, int min = 0) => RandomValue(min, a.Length);
    int RandomValue(int min, int max) => UnityEngine.Random.Range(min, max);
    bool AreEntitiesActive(GameObject go) => _player.gameObject.activeInHierarchy && go.gameObject.activeInHierarchy;
    public virtual void Z_AttackPhase()
    {
        if (IsAnyPnlActive()) return;
        if (_phase != Phases.PLANNING) return;
        _goSelector.SetActive(false);
        SetDiceSelected();
        StartCoroutine(IAttackPhase()); 
    }
    public virtual void Z_Roll()
    {
        if (IsAnyPnlActive()) return;
        if (_phase != Phases.PLANNING) return;
        RollDeck(true); 
    }
    public virtual void Z_ReShuffle()
    {
        if (IsAnyPnlActive()) return;
        if (_phase != Phases.PLANNING) return;
        if (_reShuffle > 0)
        {
            bool result = IsHandSelected();
            if (!result) UnSelect();
            _reShuffle--;
            _txtShuffle.SetText($"{_reShuffle}");
            int times = 0;
            for (int i = 0; i < _deck.Length; i++)
                if (_deck[i].gameObject.activeInHierarchy) times++;
            for (int i = 0; i < times; i++)
                _shuffleList.Add(_dicesPlayed[i]);
            _dicesPlayed.Clear();
            ShuffleDeck();
            /*if (!result) */SetDices();
        }
    }
    public void Z_SkillUse(bool use)
    {
        if (use)
        {
            ActivateSkill(_skillBe);
            _goTempDice.gameObject.SetActive(false);
            if (Data._playerData._diceSkills > 0) Data._playerData._diceSkills--;
        }
        _pnlSkillUse.SetActive(false);
        _txtSkillUse.SetText(string.Empty);
        _skillBe = TypeSkill.NONE;
        SetDices();
    }
    public int currentDescriptor = 0;
    public void Z_ChangePage(bool forward)
    {
    }
    public void Z_PanelOpen(GameObject pnl) => pnl.SetActive(true);
    public void Z_PanelClose(GameObject pnl) => pnl.SetActive(false);
    public void Z_PanelOpen(CanvasGroup pnl) => _infoScroll.CGSets(pnl, true);
    public void Z_PanelClose(CanvasGroup pnl) => _infoScroll.CGSets(pnl, false);
    #region Coroutines
    protected virtual IEnumerator IEndFight(bool defeat)
    {
        if (defeat)
        {
            ClearData();
            ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
            yield break;
        }
        switch (MapGenerator._currentTypeTile)
        {
            case TypeTile.BOSS:
                ManagerScenes.Instance.UnLoadScene(SceneToLoad.CREDITS);
                ClearData();
                break;
            case TypeTile.BLOODSHED:
                _waves--;
                _locEventWaves.StringReference = _locWaves;
                //_txtWaves.SetText($"Oleadas Restantes: {_waves}");
                _txtWaves.SetText($"{_txtWaves.text}: {_waves}");
                if (_waves <= 0)
                {
                    yield return IGetOut();
                    ReturnToMap();
                }
                else
                {
                    yield return IWaves();
                    CombatBegin();
                }
                break;
            case TypeTile.BATTLE:
                yield return IGetOut();
                ReturnToMap();
                break;
        }
    }
    protected IEnumerator IMovePlayer(Transform t)
    {
        while (_goPlayer.transform.position != t.position)
        {
            yield return null;
            _goPlayer.transform.position = Vector2.MoveTowards(_goPlayer.transform.position, t.position, _speedRun * Time.deltaTime);
        }
    }
    IEnumerator IRevealFoe(GameObject go)
    {
        go.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        go.SetActive(true);
    }
    IEnumerator IRoll(TypeRoll tRoll, bool reroll = false)
    {
        if (tRoll == TypeRoll.DECK && reroll)
        {
            if (_Reroll > 0)
            {
                _Reroll--;
                _txtRoll.SetText($"{_Reroll}");
            }
            else yield break;
        }
        SetPhase(Phases.ROLLING);
        InputCall.TriggerRumbleIndef(false, TypeRumble.ROLLING);
        UnSelect();
        switch (tRoll)
        {
            case TypeRoll.DECK:
                foreach (var d in _deck) d.Roll(true);
                SetDiceSelected();
                break;
            case TypeRoll.SKILLS: foreach (var d in _skills) d.Roll(true); break;
            case TypeRoll.ENEMY: SetEnemyStats(true); break;
        }
        yield return new WaitForSeconds(1.5f);
        switch (tRoll)
        {
            case TypeRoll.DECK:
                foreach (var d in _deck)
                {
                    yield return new WaitForSeconds(0.1f);
                    d.Roll(false);
                }
                CheckMatchDices();
                break;
            case TypeRoll.SKILLS: foreach (var d in _skills) d.Roll(false); break;
            case TypeRoll.ENEMY: SetEnemyStats(false); break;
        }
        InputCall.TriggerRumbleIndef(true);
        SetPhase(Phases.PLANNING); 
        SetDices();
    }
    protected IEnumerator IGetOut()
    {
        _animPlayer.SetBool("Iddle", false);
        yield return IMovePlayer(_tGetOut);
        _goPlayer.transform.SetParent(transform);
    }
    IEnumerator IHeal(int Heal, SlotEntity entity)
    {
        while (Heal > 0)
        {
            yield return null;
            if (entity._currentHealthDice._currentValue < 6)
                entity._currentHealthDice.DiceValue(entity._currentHealthDice._currentValue + 1);
            else 
            {
                if (entity._healthDices < entity._maxHealthDices)
                {
                    entity._healthDices++; 
                    entity._currentHealthDice = entity._health[entity._healthDices - 1];
                    entity._currentHealthDice.DiceValue(entity._currentHealthDice._currentValue + 1);
                    entity._currentHealthDice.gameObject.SetActive(true);
                }
            }
            Heal--;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator IUseEnergy(int value, bool drain)
    {
        switch (drain)
        {
            case true:
                while (value > 0)
                {
                    yield return null;
                    _player._currentEnergyDice.DiceValue(_player._currentEnergyDice._currentValue - 1);
                    _player._currentEnergyDice.gameObject.SetActive(_player._currentEnergyDice._currentValue > 0);
                    if (!_player._currentEnergyDice.gameObject.activeInHierarchy && _player._energyDices > 1)
                    {
                        _player._energyDices--;
                        if (_player._energyDices >= 1) _player._currentEnergyDice = _player._energy[_player._energyDices - 1];
                        else _player._currentEnergyDice = _player._energy[0];
                    }
                    value--;
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            case false:
                while (value > 0)
                {
                    yield return null;
                    if (_player._currentEnergyDice._currentValue < 6)
                    {
                        if (!_player._currentEnergyDice.gameObject.activeInHierarchy) _player._currentEnergyDice.gameObject.SetActive(true);
                        _player._currentEnergyDice.DiceValue(_player._currentEnergyDice._currentValue + 1);
                    }
                    else
                    {
                        if (_player._energyDices < _player._maxEnergyDices)
                        {
                            _player._energyDices++;
                            //ZDebug.Log($"{_player._energyDices - 1}");
                            _player._currentEnergyDice = _player._energy[_player._energyDices - 1]; 
                            _player._currentEnergyDice.DiceValue(_player._currentEnergyDice._currentValue + 1);
                            _player._currentEnergyDice.gameObject.SetActive(true);
                        }
                    }
                    value--;
                    yield return new WaitForSeconds(0.1f);
                }
                break;
        }
    }
    IEnumerator IDealDmg(int dmg, int def, int plus, SlotEntity receiver, SlotEntity dealer = null)
    {
        if (!receiver || !receiver.gameObject.activeInHierarchy) yield break;
        if (dealer && dmg > 0)
        {
            dealer._entity._anim.SetTrigger("Attack");
        }
        int TotalDmg = 0;
        switch (receiver._typeDefense)
        {
            case TypeDefense.FRAGILE:
                if (def > 0 && dmg <= def)
                {
                    ZDebug.Log($"{receiver.name} BLOCK ATTACK!");
                    if (dmg > 0) BlockAttack(receiver);
                    yield break;
                }else TotalDmg = dmg + plus;
                break;
            case TypeDefense.ENDURANCE:
                int result = dmg - def;
                if (def > 0 && result <= 0)
                {
                    ZDebug.Log($"{receiver.name} RESIST ATTACK");
                    if (dmg > 0) BlockAttack(receiver);
                    yield break;
                }else TotalDmg = result + plus;
                break;
            case TypeDefense.MATCH:
                if (def > 0 && dmg != def)
                {
                    ZDebug.Log($"dmg {dmg} & def {def} NOt MATCH by! {receiver.name}");
                    if (dmg > 0) BlockAttack(receiver);
                    yield break;
                }else TotalDmg = dmg + plus;
                break;
            case TypeDefense.FLIP:
                if (def > 0 && dmg != ValueFlip(def))
                {
                    ZDebug.Log($"dmg {dmg} not equal as FLIP of def {def} by! {receiver.name}");
                    if (dmg > 0) BlockAttack(receiver);
                    yield break;
                }else TotalDmg = dmg + plus;
                break;
            case TypeDefense.EVEN_ODD:
                if (def > 0)
                {
                    if (def % 2 == 0)
                    {
                        if (dmg % 2 == 0)
                        {
                            ZDebug.Log($"BLOCKED by EVEN! {receiver.name}");
                            if (dmg > 0) BlockAttack(receiver);
                            yield break;
                        }else TotalDmg = dmg + plus;
                    }
                    else
                    {
                        if (dmg % 2 != 0)
                        {
                            ZDebug.Log($"BLOCKED by ODD! {receiver.name}");
                            if (dmg > 0) BlockAttack(receiver);
                            yield break;
                        }else TotalDmg = dmg + plus;
                    }
                }else TotalDmg = dmg + plus;
                break;
        }
        InputCall.TriggerRumble(TypeRumble.ATTACK);
        receiver._entity._psDamage.Play();
        ManagerAudio.Instance.PlaySFX(TypeSFX.HIT);
        while (TotalDmg > 0)
        {
            yield return null;
            receiver._currentHealthDice.DiceValue(receiver._currentHealthDice._currentValue - 1);
            receiver._currentHealthDice.gameObject.SetActive(receiver._currentHealthDice._currentValue > 0);
            if (!receiver._currentHealthDice.gameObject.activeInHierarchy)
            {
                receiver._healthDices--;
                if (receiver._healthDices >= 1) receiver._currentHealthDice = receiver._health[receiver._healthDices - 1];
                else yield return IDisableEnemy(receiver);
            }
            TotalDmg--;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator ISetLayout()
    {
        int layout = 0;
        while (layout == LastLayout)
        {
            yield return null;
            layout = UnityEngine.Random.Range(0, _goLayouts.Length);
        }
        var scenario = Instantiate(MapGenerator._currentTypeTile == TypeTile.BOSS ? 
            _goBossLayout : _goLayouts[LastLayout = layout], Vector3.zero, Quaternion.identity);
        scenario.transform.SetParent(_tStart);
    }
    IEnumerator IWaves()
    {
        _animWaves.SetTrigger("Reveal");
        yield return new WaitForSeconds(2.5f);
        _animWaves.SetTrigger("Hide");
    }
    IEnumerator IDisableEnemy(SlotEntity entity, float t = 1f)
    {
        entity._entity._anim.SetTrigger("Death");
        yield return new WaitForEndOfFrame();
        while(entity._canvasGroup.alpha > 0)
        {
            yield return null;
            t -= Speed * Time.deltaTime;
            entity._canvasGroup.alpha = t;
        }
        entity._entity._goNav.transform.SetParent(entity._goEntity.transform);
        entity._goEntity.SetActive(false);
        entity.gameObject.SetActive(false);
    }
    IEnumerator IDamageEnemies()
    {
        foreach (var vamp in _enemies)
        {
            if (AreEntitiesActive(vamp.gameObject))
            {
                yield return IDealDmg
                    (vamp._slotDamage._currentValue, vamp._slotDefense._currentValue, (int)vamp._slotDamage._damageType, vamp, _player); //Deal Damage to Vampire
                yield return WaitBeforeAttack;
            }
        }
    }
    IEnumerator IDamagePlayer()
    {
        foreach (var vamp in _enemies)
        {
            if (AreEntitiesActive(vamp.gameObject) && _skillActive != TypeSkill.PLAYER_INVINCIBLE)
            {
                yield return IDealDmg
                    (vamp._slotAttack._currentValue, _player._slotProtection._currentValue, 0, _player, vamp); //Deal Damage to Player
                yield return WaitBeforeAttack;
            }
            if (!_player.gameObject.activeInHierarchy)
            {
                ZDebug.Log($"Player defeated");
                _pnlDefeat.SetActive(true);
                SetPhase(Phases.DEFEAT);
                yield return new WaitForSeconds(2.5f);
                yield return IEndFight(true);
            }
        }
    }
    IEnumerator IAttackPhase()
    {
        SetPhase(Phases.ATTACKING);
        _anim.SetTrigger("Hide");
        yield return IDamageEnemies();
        if (AreEnemiesDefeated())
        {
            ZDebug.Log($"ALL ENEMIES DEFEATED");
            yield return IEndFight(false);
            SetPhase(Phases.VICTORY);
        }
        if (_phase == Phases.VICTORY) yield break;
        yield return IDamagePlayer();
        if (_phase == Phases.DEFEAT) yield break;
        NewPhase();
        if (MapGenerator)
        {
            switch (MapGenerator._currentTypeTile)
            {
                case TypeTile.BOSS: ChangeDefenses(); break;
            }
        }
    }
    #endregion
}