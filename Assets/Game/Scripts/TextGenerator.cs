using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TextGenerator : MonoBehaviour
{
    public Speech _speech;
    public TextMeshProUGUI _txtSpeech;
    public CanvasGroup _cg;
    public float _speedWrite = 0.01f;
    public Color _colorNight, _colorCurse;
    public SpriteRenderer _sRend;
    public int _textPass = 0;
    public Button _btnSelf;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;
    [ContextMenu("WRITTE")]
    void WriteText() => StartCoroutine(ShowText());
    public float _canvasTransition = 2f, _nightTransition = 2f;
    void Awake()
    {
        StartCoroutine(ShowText());
        ManagerInputCall.UpdateSelected(_btnSelf.gameObject);
    }
    private void Update()
    {
        _btnSelf.interactable = ManagerScenes.Instance._phase == PhaseLoading.NONE;
    }
    public void Z_Skip()
    {
        //StopCoroutine("ShowText");
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
    }
    IEnumerator ShowText()
    {
        _cg.alpha = 0f;
        _txtSpeech.maxVisibleCharacters = 0; // Start with no visible characters
        _sRend.color = _colorNight;
        float startTime = Time.time;
        float night = 0f;

        while (night < _nightTransition)
        {
            yield return null;
            float t = night / _nightTransition;
            _sRend.color = Color.Lerp(_colorNight, _colorCurse, t);

            night = Time.time - startTime;
        }
        float cg = 1;
        while (_cg.alpha < 1)
        {
            yield return null;
            cg += _canvasTransition * Time.deltaTime;
            _cg.alpha = cg;
        }

        if (_textPass == _speech._speechLines.Length) yield break;
        _txtSpeech.text = _speech._speechLines[_textPass]._desc; // Set the full text initially

        for (int i = 0; i <= _speech._speechLines[_textPass]._desc.Length; i++)
        {
            _txtSpeech.maxVisibleCharacters = i;
            yield return new WaitForSeconds(_speedWrite);
        }

        yield return new WaitForSeconds(7f);
        Z_Skip();
    }
}
