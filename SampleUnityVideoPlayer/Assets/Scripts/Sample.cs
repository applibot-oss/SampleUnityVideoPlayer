using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera = null;
    [SerializeField]
    private Text stateText = null;
    [SerializeField]
    private Button button = null;
    [SerializeField]
    private Text buttonText = null;

    private Art.Sample.VideoPlay logoVideo = null;
    private Art.Sample.VideoPlay opVideo = null;

    void Awake()
    {
        StartMovie();
    }

    public void OnClick()
    {
        if (buttonText.text == "Play")
        {
            StartMovie();
        }
        else if (buttonText.text == "Pause")
        {
            Pause();
        }
        else if (buttonText.text == "Resume")
        {
            Resume();
        }
    }

    private void StartMovie()
    {
        stateText.text = "ロード中です。";
        button.interactable = false;

        // インスタンスの生成
        if(logoVideo == null)
        {
            logoVideo = Art.Sample.VideoPlay.Create(mainCamera);    
        }
        if (opVideo == null)
        {
            opVideo = Art.Sample.VideoPlay.Create(mainCamera);
        }

        // イベント設定
        logoVideo.OnPrepareCompleted = PrepareCompleted;
        logoVideo.OnStarted = Started;
        logoVideo.OnEnd = PlayEnd;
        logoVideo.OnErrorReceived = ErrorReceived;
        opVideo.OnPrepareCompleted = PrepareCompleted;
        opVideo.OnStarted = Started;
        opVideo.OnEnd = PlayEnd;
        opVideo.OnErrorReceived = ErrorReceived;

        // 表示を有効にする
        logoVideo.SetEnabled(true);
        opVideo.SetEnabled(true);

        // 事前ロード
        logoVideo.Preload(Application.streamingAssetsPath + "/applibot_3sec.mp4");
        opVideo.Preload(Application.streamingAssetsPath + "/applibot_eagle_movie.mp4");
    }

    private void Pause()
    {
        stateText.text = "Paused";
        buttonText.text = "Resume";
        logoVideo.Pause();
        opVideo.Pause();
    }

    private void Resume()
    {
        buttonText.text = "Pause";
        logoVideo.Resume();
        opVideo.Resume();
    }

    private void Started()
    {
        if (logoVideo.State == Art.Sample.VideoPlay.PlayState.Playing
            && opVideo.State != Art.Sample.VideoPlay.PlayState.Playing)
        {
            button.interactable = true;
            buttonText.text = "Pause";
        }
        stateText.text = "Started";

        Debug.Log("Started");
    }

    private void PlayEnd()
    {
        stateText.text = "PlayEnd";
        Debug.Log("PlayEnd");

        if (logoVideo.State == Art.Sample.VideoPlay.PlayState.Stoped
            && opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded)
        {
            buttonText.text = "Stop";
            logoVideo.SetEnabled(false);
            button.interactable = false;
            opVideo.PlayPrepared(false, true);
        }
        else if(opVideo.State == Art.Sample.VideoPlay.PlayState.Stoped)
        {
            buttonText.text = "Play";
            opVideo.SetEnabled(false);
            button.interactable = true;
        }
    }

    private void ErrorReceived(string message)
    {
        stateText.text = "ErrorReceived";
        Debug.LogError("ErrorReceived : " + message);
    }

    private void PrepareCompleted()
    {
        stateText.text = "PrepareCompleted";
        Debug.Log("PrepareCompleted");

        if (logoVideo.State == Art.Sample.VideoPlay.PlayState.Loaded
           && opVideo.State == Art.Sample.VideoPlay.PlayState.Loaded)
        {
            logoVideo.PlayPrepared(false, false);
        }
    }
}
