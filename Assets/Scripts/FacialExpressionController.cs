using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
// using UnityEngine.Serialization;
// using UnityEditor.Scripting.Python;

public class FacialExpressionController : MonoBehaviour
{
    private TcpListener _listener;
    CubismModel _model;
    
    private int _port = 13967;
    private string _host = "127.0.0.1";
    private NetworkStream _stream;
    private StreamReader _reader;
    private TcpListener _tcpListener;
    private Thread _tcpListenerThread;
    public TcpClient connectedTcpClient;
    public string receivedData = "";

    private Transform _cubismParameters;
    private Transform _paramRotX;
    private Transform _paramRotY;
    private Transform _paramRotZ;
    private Transform _paramEyeROpen;
    private Transform _paramEyeLOpen;
    private Transform _paramMouthOpen;
    
    private bool _isCalibrating;
    private FacialExpressionData _minFaceData;
    private FacialExpressionData _maxFaceData;
    private bool _isCalibrationCompleted;

    // Start is called before the first frame update
    void Start()
    {
        _isCalibrating = false;
        _isCalibrationCompleted = false;
        //_model = _model.FindCubismModel(includeParents:true);

        _model.transform.localScale *= 6.0f; 
        
        _cubismParameters = _model.transform.GetChild(0);
        _paramRotX = _cubismParameters.Find("ParamAngleX");
        _paramRotY = _cubismParameters.Find("ParamAngleY");
        _paramRotZ = _cubismParameters.Find("ParamAngleZ");
        _paramEyeLOpen = _cubismParameters.Find("ParamEyeLOpen");
        _paramEyeROpen = _cubismParameters.Find("ParamEyeROpen");
        _paramMouthOpen = _cubismParameters.Find("ParamMouthOpenY");

        
    }

    public void StartListeningTcp()
    {
        _tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));
        _tcpListenerThread.IsBackground = true;
        _tcpListenerThread.Start();
    }

    public void StopListeningTcp()
    {
        connectedTcpClient.GetStream().Close();
        connectedTcpClient.Close();
        _tcpListener.Stop();
        _tcpListenerThread.Abort();
        
        Debug.Log("Server has stopped listening");
    }

    private void ListenForIncommingRequests()
    {
        // Create listener on localhost port.          
        _tcpListener = new TcpListener(IPAddress.Parse(_host), _port);
        _tcpListener.Start();              
        Debug.Log("Server is listening");
        
        while (true) 
        {
            using (connectedTcpClient = _tcpListener.AcceptTcpClient())
            {
                // Get a stream object for reading
                using (NetworkStream stream = connectedTcpClient.GetStream()) 
                {
                    int length;
                    byte[] buffer =  new byte[connectedTcpClient.ReceiveBufferSize];
                    // Read incoming stream into byte array.                        
                    while ((length = stream.Read(buffer, 0, buffer.Length)) != 0) 
                    {
                        var incomingData = new byte[length];
                        Array.Copy(buffer, 0, incomingData, 0, length);
                        // Convert byte array to string message.                            
                        string clientMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log("client message received as: " + clientMessage);

                        // Store the received data
                        receivedData = clientMessage;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (receivedData == string.Empty)
            return;
        
        var facialExpressionData = JsonUtility.FromJson<FacialExpressionData>(receivedData);
        

        if (_isCalibrationCompleted)
        {
            /*facialExpressionData.headRotX = (facialExpressionData.headRotX - _minFaceData.headRotX) /
                                            (_maxFaceData.headRotX - _minFaceData.headRotX);
            facialExpressionData.headRotY = (facialExpressionData.headRotY - _minFaceData.headRotY) /
                                            (_maxFaceData.headRotY - _minFaceData.headRotY);
            facialExpressionData.headRotZ = (facialExpressionData.headRotZ - _minFaceData.headRotZ) /
                                            (_maxFaceData.headRotZ - _minFaceData.headRotZ);
            */
            facialExpressionData.leftEye = (facialExpressionData.leftEye - _minFaceData.leftEye) /
                                           (_maxFaceData.leftEye - _minFaceData.leftEye);
            facialExpressionData.rightEye = (facialExpressionData.rightEye - _minFaceData.rightEye) /
                                            (_maxFaceData.rightEye - _minFaceData.rightEye);
            facialExpressionData.mouthOpenness = (facialExpressionData.mouthOpenness - _minFaceData.mouthOpenness) /
                                                 (_maxFaceData.mouthOpenness - _minFaceData.mouthOpenness);
            
            
            print($"{_minFaceData.leftEye}, {_maxFaceData.leftEye}");
        }
        
        
        // x - min(x)) / ( max(x) - min(x) 
        _model.Parameters[_paramRotX.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override,
                (facialExpressionData.headRotX * 30 * -1));   
        _model.Parameters[_paramRotY.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override,
                (facialExpressionData.headRotY * 30 * 2));  
        _model.Parameters[_paramRotZ.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override,
                (facialExpressionData.headRotZ * 30 * -1 * 2)); 
        _model.Parameters[_paramEyeLOpen.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override, 
                facialExpressionData.leftEye);
        _model.Parameters[_paramEyeROpen.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override, 
                facialExpressionData.rightEye);
        _model.Parameters[_paramMouthOpen.GetSiblingIndex()].
            BlendToValue(CubismParameterBlendMode.Override, 
                facialExpressionData.mouthOpenness);
        
    }
    
    void OnApplicationQuit()
    {
        // close the streams and the connection
        StopListeningTcp();
    }

    IEnumerator CalibrateFaceInputs()
    {
        _isCalibrationCompleted = false;
        
        List<FacialExpressionData> faceData = new List<FacialExpressionData>();

        while (_isCalibrating)
        {
            faceData.Add(JsonUtility.FromJson<FacialExpressionData>(receivedData));

            _minFaceData = new FacialExpressionData
            (
                faceData.Min(x => x.headRotX),
                faceData.Min(x => x.headRotY),
                faceData.Min(x => x.headRotZ),
                faceData.Min(x => x.leftEye),
                faceData.Min(x => x.rightEye),
                faceData.Min(x => x.mouthOpenness)
            );
            
            _maxFaceData = new FacialExpressionData
            (
                faceData.Max(x => x.headRotX),
                faceData.Max(x => x.headRotY),
                faceData.Max(x => x.headRotZ),
                faceData.Max(x => x.leftEye),
                faceData.Max(x => x.rightEye),
                faceData.Max(x => x.mouthOpenness)
            );
            
            yield return new WaitForSeconds(0.1f);
        }

        _isCalibrationCompleted = true;
    }

    public void SetCalibrationValue(bool value)
    {
        _isCalibrating = value;
        if (!_isCalibrating)
            return;
        
        StartCoroutine(CalibrateFaceInputs());
    }

    public void SetModel(CubismModel model)
    {
        _model = model;
    }
}

[System.Serializable]
public class FacialExpressionData
{
    public float headRotX;
    public float headRotY;
    public float headRotZ;
    public float leftEye;
    public float rightEye;
    public float mouthOpenness;

    public FacialExpressionData(float headRotX, float headRotY, float headRotZ,
        float leftEye, float rightEye, float mouthOpenness)
    {
        this.headRotX = headRotX;
        this.headRotY = headRotY;
        this.headRotZ = headRotZ;
        this.leftEye = leftEye;
        this.rightEye = rightEye;
        this.mouthOpenness = mouthOpenness;
    }
}
