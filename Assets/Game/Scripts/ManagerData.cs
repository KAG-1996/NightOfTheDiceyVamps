using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ManagerData : MonoBehaviour
{
    public static ManagerData Instance;
    public TypeSaveData _typeSave;
    public SaveData _saveData;
    ManagerLoadingScreen LoadingScreen => ManagerLoadingScreen.Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            try
            {
                Load();
            }
            catch
            {
                ZDebug.Log("Creating Save File...");
                Save();
                ZDebug.Log("Save File Created!");
                Load();
            }
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_saveData._settingsData._idiom];
    }
    public void Save()
    {
        switch (_typeSave)
        {
            case TypeSaveData.BINARY: _saveData.SaveToBinary(); break;
            case TypeSaveData.JASON: _saveData.SaveToJson(); break;
        }
        StartCoroutine(ISaveIcon());
    }
    public void Load()
    {
        switch (_typeSave)
        {
            case TypeSaveData.BINARY: _saveData.LoadFromBinary(); break;
            case TypeSaveData.JASON: _saveData.LoadFromJson(); break;
        }
    }
    [ContextMenu("NewPlayerData")]
    public void NewPlayerData()
    {
        _saveData._playerData = new PlayerData(0, 2, 2, SetInitialDeck(7));
    }
    List<int> SetInitialDeck(int size)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < size; i++) list.Add(i < 4 ? 0 : UnityEngine.Random.Range(1, 6));
        return list;
    }
    IEnumerator ISaveIcon()
    {
        if (!LoadingScreen) yield return new WaitUntil(() => LoadingScreen);
        LoadingScreen._animSave.SetBool("save", true);
        yield return new WaitForSeconds(1f);
        LoadingScreen._animSave.SetBool("save", false);
    }
}