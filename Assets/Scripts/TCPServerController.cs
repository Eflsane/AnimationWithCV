using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Live2D.Cubism.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class TCPServerController : MonoBehaviour
{
    [SerializeField] private int port = 13967;
    [SerializeField] private string host = "127.0.0.1";
    private TcpListener _tcpListener;
    private Thread _tcpListenerThread;
    private TcpClient _connectedTcpClient;
    private NetworkStream _tcpStream;
    [SerializeField]private string receivedData = "";
    private bool _isServerListening;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartListeningTcp()
    {
        _isServerListening = true;
        _tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));
        _tcpListenerThread.IsBackground = true;
        _tcpListenerThread.Start();
    }

    public void StopListeningTcp()
    {
        _isServerListening = false;
        DisconnectClient();
        _tcpListener.Stop();
        _tcpListenerThread.Abort();
        
        print("Server has stopped listening");
    }

    public string GetReceivedData()
    {
        return receivedData;
    }

    public void DisconnectClient()
    {
        _tcpStream?.Close();
        _connectedTcpClient?.Close();
        _connectedTcpClient = null;
        receivedData = String.Empty;
    }
    
    private void ListenForIncommingRequests()
    {
        // Create listener on localhost port.          
        _tcpListener = new TcpListener(IPAddress.Parse(host), port);
        _tcpListener.Start();              
        Debug.Log("Server is listening");
        
        while (_isServerListening) 
        {
            using (_connectedTcpClient = _tcpListener.AcceptTcpClient())
            {
                // Get a stream object for reading
                using (_tcpStream = _connectedTcpClient.GetStream()) 
                {
                    int length;
                    byte[] buffer =  new byte[_connectedTcpClient.ReceiveBufferSize];
                    // Read incoming stream into byte array.                        
                    while ((length = _tcpStream.Read(buffer, 0, buffer.Length)) != 0) 
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

    private void OnApplicationQuit()
    {
        StopListeningTcp();
    }
}

public enum RecievedDataType
{
    None = 0,
    Emotion = 1,
    FaceParams = 2
}
