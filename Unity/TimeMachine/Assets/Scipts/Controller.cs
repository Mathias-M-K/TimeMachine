using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Michsky.UI.ModernUIPack;
using UnityEditor.U2D;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private string ipAddress;
    [SerializeField] private int port;

    [SerializeField] private float incomingValue;
    [SerializeField] private string outgoingValue;
    [SerializeField] private bool connected;
    
    private readonly WifiConnection _comm = new WifiConnection();

    public AnimatedIconHandler connectionIndicator;
    private bool _showingConnected;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            connectionIndicator.ClickEvent();
        }
    
        if (Input.GetKeyDown(KeyCode.D))
        {
            _comm.CloseConnection();
        }
        
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectToEsp();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _comm.WriteToArduino(outgoingValue);
        }

        
        incomingValue = _comm.CurrentValue;
        connected = _comm.Connected;
        
        UpdateConnectedIcon(connected);
    }

    private void ConnectToEsp()
    {
        Debug.Log("Attempting connection...");
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