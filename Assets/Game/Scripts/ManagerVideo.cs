using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ManagerVideo : MonoBehaviour
{
    public VideoPlayer _videoPlayer;
    public Button _btnSelf;
    ManagerInputCalls ManagerInputCall => ManagerInputCalls.Instance;

    private void Update()
    {
        _btnSelf.interactable = ManagerScenes.Instance._phase == PhaseLoading.NONE;
    }
    private void Start()
    {
        ManagerInputCall.UpdateSelected(_btnSelf.gameObject);
    }
    void OnEnable()
    {
        // Ensure the VideoPlayer is assigned
        if (_videoPlayer == null) _videoPlayer = GetComponent<VideoPlayer>();
        if (_videoPlayer != null) _videoPlayer.loopPointReached += OnVideoFinished; 
    }

    void OnDisable()
    {
        if (_videoPlayer != null)
        {
            _videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Z_Skip();
    }
    public void Z_Skip()
    {
        //StopCoroutine("ShowText");
        ManagerScenes.Instance.UnLoadScene(SceneToLoad.MENU);
    }
}
