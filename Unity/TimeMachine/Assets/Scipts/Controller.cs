using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private string ipAddress;
    [SerializeField] private int port;

    
    [SerializeField] private string incomingRaw;
    [SerializeField] private float val1;
    [SerializeField] private float pingTime;
    [SerializeField] private string outgoingValue;
    
    
    private bool connected;    
    private readonly WifiConnection _comm = new WifiConnection();

    public AnimatedIconHandler connectionIndicator;
    public TextMeshProUGUI statusText;
    private bool _showingConnected;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _comm.PingDevice();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _comm.StartContinuousPinging();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _comm.StopContinuousPinging();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            connectionIndicator.ClickEvent();
        }
    
        if (Input.GetKeyDown(KeyCode.D))
        {
            _comm.CloseConnection("Manually Disconnected");
        }
        
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectToEsp();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _comm.WriteToArduino(outgoingValue);
        }

        incomingRaw = _comm.rawValue;
        val1 = _comm.val1;
        pingTime = _comm.pingTime;
        
        connected = _comm.connected;
        statusText.text = _comm.status;
        
        
        UpdateConnectedIcon(connected);
    }

    private void ConnectToEsp()
    {
        if (connected)
        {
            return;
        }
        _comm.Begin(ipAddress, port);
    }

    private void UpdateConnectedIcon(bool state)
    {
        if (state != _showingConnected)
        {
            _showingConnected = state;
            connectionIndicator.ClickEvent();
        }
    }
}