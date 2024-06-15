using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FlutterUnityIntegration;
using Live2D.Cubism.Core;
using UnityEngine;

public class EmotionDetectionController : MonoBehaviour
{
    public string receivedData = "";

    [SerializeField] private EmotionalGamesManager emoGamesMan;
    // [SerializeField] private GameAimAngerMode gameAnger;
    // [SerializeField] private GameObject gameSad;
    
    private string _curEmo;
    
    private bool _isEmotionsListening;

    private TCPServerController _tcpServerController;
    private TcpClientController _tcpClientController;

    private PythonModulesExecutor _pythonModulesExecutor;
    private Process _emotionDetectionProcess;
    private const string AbsoluteExeEditorPathWin = "\\Assets\\PythonProgram\\emotion_detection.exe";
    private const string AbsoluteBatEditorPathWin = "\\Assets\\PythonProgram\\emotion_detection.bat";
    private const string AbsoluteExeReleasePathWin = "\\AnimationWithCV_Data\\StreamingAssets\\emotion_detection.exe";
    private const string AbsoluteBatReleasePathWin = "\\AnimationWithCV_Data\\StreamingAssets\\emotion_detection.bat";

    private bool _isEmotionDetectionActive;
    public Action EmotionDetectionActive = () => { };
    public Action EmotionDetectionDeactivated = () => { };

    // Start is called before the first frame update
    void Start()
    {
        _tcpServerController = GetComponent<TCPServerController>();
        _tcpClientController = GetComponent<TcpClientController>();
        _pythonModulesExecutor = GetComponent<PythonModulesExecutor>();

        _isEmotionDetectionActive = false;
        
        StartListeningEmotions();
    }

    public void StartListeningEmotions()
    {
        _tcpServerController.StartListeningTcp();
        
        _tcpClientController.StartConnectionTcp();
        
        _isEmotionsListening = true;

        _emotionDetectionProcess = _pythonModulesExecutor.StartDetectionPythonModule(AbsoluteBatEditorPathWin, AbsoluteBatReleasePathWin,
            _emotionDetectionProcess);
        
        print("Has started emotion detection");
    }

    private void StartEmotionDetectionPythonModule()
    {
        string pathToExe = Directory.GetCurrentDirectory();
        
        if (Application.isEditor)
        {
            pathToExe += AbsoluteExeEditorPathWin;
        }
        else
        {
            pathToExe += AbsoluteExeReleasePathWin;
        }

        if (_emotionDetectionProcess != null)
            return;
        
        _emotionDetectionProcess = new Process();

        _emotionDetectionProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
        _emotionDetectionProcess.StartInfo.FileName = $"{pathToExe}";
        _emotionDetectionProcess.StartInfo.Arguments = "";

        _emotionDetectionProcess.Start();
    }

    public void StopListeningEmotions()
    {
        _isEmotionDetectionActive = false;
        _isEmotionsListening = false;
        _tcpClientController.StopConnectionTcp();
        _tcpServerController.StopListeningTcp();

        _emotionDetectionProcess = _pythonModulesExecutor.
            StopEmotionDetectionPythonModule(_emotionDetectionProcess);
        
        _tcpServerController.StopListeningTcp();
        print("Has stopped emotion detection");
    }

    private void StopEmotionDetectionPythonModule()
    {
        if (_emotionDetectionProcess == null)
            return;
        
        print(_emotionDetectionProcess.ProcessName + " " + _emotionDetectionProcess.Id);
        //Process.GetProcessesByName(_cameraProcess.ProcessName)[1].Kill();
        _emotionDetectionProcess?.Close();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_isEmotionsListening)
            return;
        
        receivedData = _tcpServerController?.GetReceivedData();
        
        if (receivedData == string.Empty)
            return;
        
        
        var facialEmotionData = JsonUtility.FromJson<FacialEmotionData>(receivedData);

        if (facialEmotionData == null || 
            facialEmotionData.emotion == string.Empty || 
            facialEmotionData.emotion == null)
            return;
        
        print(facialEmotionData?.emotion);
        
        if (!_isEmotionDetectionActive)
        {
            _isEmotionDetectionActive = true;
            EmotionDetectionActive.Invoke();
        }

        if (facialEmotionData.emotion.Equals(Emotions.Surprise.ToString().ToLowerInvariant()) ||
            facialEmotionData.emotion.Equals(Emotions.Disgust.ToString().ToLowerInvariant()))
            facialEmotionData.emotion = Emotions.Angry.ToString().ToLowerInvariant();
        //send data back to flutter
        // _unityMessageManager.SendMessageToFlutter(facialEmotionData.emotion);
        
        print(Emotions.Angry.ToString());
        _tcpClientController.SetSendData(_curEmo);
        
        if(!emoGamesMan.isCounterOn)
            return;
        
        print(facialEmotionData.emotion.Equals(_curEmo));
        //activate mini-games
        if (facialEmotionData.emotion.Equals(_curEmo))
            emoGamesMan.currentCounterOnOneEmo += Time.deltaTime;
        else
        {
            emoGamesMan.currentCounterOnOneEmo = 0.0f;
            _curEmo = facialEmotionData.emotion;
        }
            
        
        if(emoGamesMan.currentCounterOnOneEmo < emoGamesMan.estimatedCounterTime)
            return;

        print(Emotions.Angry.ToString());
        _tcpClientController.SetSendData(_curEmo);

        if (_curEmo.Equals(Emotions.Angry.ToString().ToLowerInvariant()))
        {
            print("game to Start");
            emoGamesMan.isCounterOn = false;
            emoGamesMan.StartGame(EmoGameType.Anger);
        }
        else if (_curEmo.Equals(Emotions.Happy.ToString().ToLowerInvariant()))
        {
            print("game to Start");
            emoGamesMan.isCounterOn = false;
            emoGamesMan.StartGame(EmoGameType.Sad);
        }

        emoGamesMan.currentCounterOnOneEmo = 0;
    }
    
    void OnApplicationQuit()
    {
        // close the streams and the connection
        StopListeningEmotions();
    }
}

[Serializable]
public class FacialEmotionData
{
    public string emotion;


    public FacialEmotionData(string emotion)
    {
        this.emotion = emotion;
    }
}


public enum Emotions
{
    None = 0,
    Neutral = 1,
    Happy = 2,
    Angry = 3,
    Sad = 4,
    Disgust = 5,
    Surprise = 6,
}