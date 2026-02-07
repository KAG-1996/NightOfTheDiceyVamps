using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ManagerGameTutorial : ManagerGame
{
    [Serializable]
    public struct TutorialParts
    {
        public Button _btnPlay, _btnRoll, _btnShuffle, _btnInfo;
        public GameObject _goHand;
    }
    [Header("-- Tutorial Stuff --")]
    public Speech _speech;
    public Image _imgVamp;
    public TextMeshProUGUI _txtSpeech;
    public LocalizeStringEvent _locEventTutorial;
    public GameObject _pnlTuto;
    public Animator _animTuto;
    public float _speedWrite = 0.01f;
    //public Animator _
    public TutorialParts _tutoParts;
    public StatsEnemy[] _stats;
    public BattleSets _battle;
    public List<int> _deckTutorial;

    readonly int RollShuffle = 3;
    private int Skills = 3;
    protected override IEnumerator Start()
    {
        _tutoParts._btnInfo.gameObject.SetActive(false);
        _tutoParts._btnPlay.gameObject.SetActive(false);
        _tutoParts._btnRoll.gameObject.SetActive(false);
        _tutoParts._btnShuffle.gameObject.SetActive(false);
        //_tutoParts._goHand.SetActive(false);
        SetInitials();
        yield return IMovePlayer(_tEnd);
        _animTuto.SetTrigger("Reveal");
        _animPlayer.SetBool("Iddle", true);
        _player._entity._goNav.transform.SetParent(_canvasWorld);
        _goBtnPlayer = _player._entity._goNav;
        _txtSpeech.SetText(string.Empty);
        WriteText();
    }

    [ContextMenu("WRITTE")]
    void WriteText() => StartCoroutine(ShowText());
    public int _textPass = 0;
    IEnumerator ShowText()
    {
        if (_textPass == _speech._speechLines.Length) yield break;
        _locEventTutorial.StringReference = _speech._speechLines[_textPass]._locString; // Set the full text initially
        _txtSpeech.maxVisibleCharacters = 0; // Start with no visible characters

        //for (int i = 0; i <= _speech._speechLines[_textPass]._desc.Length; i++)
        if (_txtSpeech.text.Length == 0) yield return new WaitUntil(() => _txtSpeech.text.Length != 0);
        for (int i = 0; i <= _txtSpeech.text.Length; i++)
        {
            yield return new WaitForSeconds(_speedWrite);
            if (ManagerLoadingScreen.Instance._pnlExit.activeInHierarchy || _pnlSkillUse.activeInHierarchy || _pnlInfo.activeInHierarchy)
                yield return new WaitUntil(() => 
                !ManagerLoadingScreen.Instance._pnlExit.activeInHierarchy && 
                !_pnlSkillUse.activeInHierarchy && 
                !_pnlInfo.activeInHierarchy);
            _txtSpeech.maxVisibleCharacters = i;
        }
        yield return IWaitNext(_speech._speechLines[_textPass]._tittle);
        _textPass++;
        if (_textPass == _speech._speechLines.Length)
        {
            _txtSpeech.maxVisibleCharacters = 0;
            _animTuto.SetTrigger("Hide");
            yield break;
        }
        if (_phase != Phases.PLANNING)
        {
            _txtSpeech.maxVisibleCharacters = 0;
            yield return IWaitPlanningPhase();
        }
        WriteText();
    }
    IEnumerator IWaitNext(string id)
    {
        switch (id)
        {
            default: yield return new WaitForSeconds(7f); break;
            case TutorialID._ENTITY: yield return IWaitEntities(); break;
            case TutorialID._INTRO: 
                yield return new WaitForSeconds(2.5f);
                SetGame();
                break;
            case TutorialID._DICE:
                //_tutoParts._goHand.SetActive(true);
                SetDices();
                yield return IWaitDiceDrag();
                break;
            case TutorialID._INFO:
                _tutoParts._btnInfo.gameObject.SetActive(true);
                yield return new WaitForSeconds(7f);
                break;
            case TutorialID._ROLL:
                _tutoParts._btnRoll.gameObject.SetActive(true); 
                yield return new WaitForSeconds(7f); 
                break;
            case TutorialID._SHUFFLE:
                _tutoParts._btnShuffle.gameObject.SetActive(true); 
                yield return new WaitForSeconds(7f); 
                break;
            case TutorialID._SKILLS:
                foreach (var a in _skills) a._btnDice.interactable = true;
                yield return new WaitForSeconds(7f); 
                break;
            case TutorialID._ATTACK:
                _tutoParts._btnPlay.gameObject.SetActive(true);
                yield return IWaitAttackPhase(); 
                break;
        }
    }
    IEnumerator IWaitDiceDrag() { yield return new WaitUntil(() => _diceDrag.gameObject.activeInHierarchy); }
    IEnumerator IWaitAttackPhase() { yield return new WaitUntil(() => _phase == Phases.ATTACKING); }
    IEnumerator IWaitPlanningPhase() { yield return new WaitUntil(() => _phase == Phases.PLANNING); }
    IEnumerator IWaitEntities()
    {
        while(!WillEnemiesDamage() && _player._slotProtection._currentValue == 0)
        {
            yield return null;
        }
    }
    public override IEnumerator ISetDices()
    {
        if (!ShuffleAndRoll())
        {
            foreach (var a in _skills) a._btnDice.interactable = false;
            yield break;
        }
        yield return base.ISetDices();
    }
    bool ShuffleAndRoll() => _tutoParts._btnShuffle.gameObject.activeInHierarchy && _tutoParts._btnRoll.gameObject.activeInHierarchy;
    bool WillEnemiesDamage()
    {
        for (int i = 0; i < _battle._totalEnemies; i++)
        {
            if (_enemies[i]._slotDamage._currentValue != 0) return true;
        }
        return false;
    }
    protected override void ClearData() { } //Not deleting from tutorial
    protected override void SetDeckForShuffle()
    {
        _shuffleList.Clear();
        _shuffleList = _deckTutorial;
        _dicesPlayed.Clear();
    }
    protected override IEnumerator IEndFight(bool defeat)
    {
        yield return IGetOut();
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
    }
    public override void NewPhase()
    {
        base.NewPhase();
        _Reroll = RollShuffle;
        _reShuffle = RollShuffle;
        _txtRoll.SetText($"{_Reroll}");
        _txtShuffle.SetText($"{_reShuffle}");
    }
    public override void SetCurrentSkillDices()
    {
        foreach (var a in _skills) a.gameObject.SetActive(false);
        if (Skills == 0) return;
        for (int i = 0; i < Skills; i++)
            _skills[i].gameObject.SetActive(true);
    }
    protected override void SetPlayerStats() => _player.SetStats(2, 2, 0);
    protected override void SetEnemies()
    {
        foreach (var a in _enemies) a.gameObject.SetActive(false);
        ReordeEnemies re = new();
        foreach (var a in _enemyPositions)
        {
            if (a._tPositions.Length == _battle._totalEnemies)
            {
                re = a;
                break;
            }
        }
        for (int i = 0; i < _battle._totalEnemies; i++)
        {
            _enemies[i].SetStats(_stats[UnityEngine.Random.Range(0, _stats.Length)]);
            _enemies[i].gameObject.SetActive(true);
            _enemies[i]._typeEnemy = _enemies[i]._stats._typeEnemy;
            _enemies[i]._goEntity = Instantiate(_enemies[i]._stats._goPrefab, re._tPositions[i].position, Quaternion.identity);
            _enemies[i]._goEntity.transform.SetParent(re._tPositions[i]);
            _enemies[i]._entity = _enemies[i]._goEntity.GetComponent<SelectEntity>();
            _enemies[i]._entity._dice = _enemies[i]._slotDamage;
            _enemies[i]._entity._goNav.transform.SetParent(_canvasWorld);
            _enemies[i]._canvasGroup.alpha = 1;
        }
    }
    public override void ShowInfo()
    {
        if (!_tutoParts._btnInfo.gameObject.activeInHierarchy) return;
        base.ShowInfo();
    }
    public override void Z_AttackPhase()
    {
        if (!_tutoParts._btnPlay.gameObject.activeInHierarchy) return;
        base.Z_AttackPhase();
    }
    public override void Z_Roll()
    {
        if (!_tutoParts._btnRoll.gameObject.activeInHierarchy) return;
        base.Z_Roll();
    }
    public override void Z_ReShuffle()
    {
        if (!_tutoParts._btnShuffle.gameObject.activeInHierarchy) return;
        base.Z_ReShuffle();
    }
}

struct TutorialID
{
    public const string
        _INTRO = "INTRO",
        _DICE = "DICE",
        _ENTITY = "ENTITY",
        _ATTACK = "ATTACK",
        _HEALTHENERGY = "HEALTH ENERGY",
        _REQUIREMENTS = "REQUIREMENTS",
        _ROLL = "ROLL",
        _SHUFFLE = "SHUFFLE",
        _SKILLS = "SKILLS",
        _PROTECTION = "PROTECTION",
        _WAVES = "WAVES",
        _INFO = "INFO",
        _OUTRO = "OUTRO";
}
