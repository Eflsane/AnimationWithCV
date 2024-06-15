using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using TcpClient = System.Net.Sockets.TcpClient;

public class TcpClientController : MonoBehaviour
{
    [SerializeField] private int port = 9001;
    [SerializeField] private string host = "127.0.0.1";
    private TcpClient _tcpClient;
    private Thread _tcpListenerThread;
    private NetworkStream _tcpStream;
    [SerializeField] private string sendData = "";
    private bool _isConnected;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void StartConnectionTcp()
    {
        _isConnected = true;
        _tcpListenerThread = new Thread (new ThreadStart(ConnectToServer));
        _tcpListenerThread.IsBackground = true;
        _tcpListenerThread.Start();
    }

    public void StopConnectionTcp()
    {
        _isConnected = false;
        _tcpListenerThread.Abort();
        
        print("Disconnected from server");
    }

    public void SetSendData(string dataToSend)
    {
        sendData = dataToSend;
    }

    /*public void DisconnectClient()
    {
        _tcpStream?.Close();
        _tcpClient?.Close();
        
    }*/
    
    private void ConnectToServer()
    {
        try
        {
            using (var client = new TcpClient())
            {
                client.Connect(IPAddress.Parse(host), port);
                Debug.Log($"Connected to server: {host}:{port}");

                // Отправляем сообщения в бесконечном цикле
                while (_isConnected)
                {
                    var jsonData = JsonUtility.ToJson(new FacialEmotionData(sendData));
                    var data = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    print($"data to send: {jsonData}");
                    client.GetStream().Write(data, 0, data.Length);
                    
                    Thread.Sleep(1000);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting to server: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        StopConnectionTcp();
    }
}
