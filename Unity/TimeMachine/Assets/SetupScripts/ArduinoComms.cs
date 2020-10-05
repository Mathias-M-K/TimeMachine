using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

using UnityEngine;

public class ArduinoComms : MonoBehaviour
{
    private bool _active = false;

    wrmhl myDevice = new wrmhl(); // wrmhl is the bridge beetwen your computer and hardware.

    [Tooltip("SerialPort of your device.")]
    public string portName = "COM8";

    [Tooltip("Baudrate")]
    public int baudRate = 250000;


    [Tooltip("Timeout")]
    public int ReadTimeout = 20;

    [Tooltip("Something you want to send.")]
    public string dataToSend = "Hello World!";

    [Tooltip("You are recieving")] 
    public string incomingData;

    [Tooltip("QueueLenght")]
    public int QueueLenght = 1;
    
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            Begin(portName);
        }

        if (!_active) return;
        
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space))
        {
            myDevice.send(dataToSend);
            print("Sending");
        }
        
        
        string tempString = myDevice.readQueue();

        if (tempString != null)
        {
            incomingData = tempString;
            
        }
        
    }
    
    void OnApplicationQuit() { // close the Thread and Serial Port

        try
        {
            myDevice.close();
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(e);
        }
        
    }

    public void CloseConnection()
    {
        try
        {
            myDevice.close();
        }
        catch (NullReferenceException e)
        {
            Console.WriteLine(e);
        }
    }

    public void Begin(string comPort)
    {
        print("Initiating connection");
        _active = true;
        myDevice.set (comPort, baudRate, ReadTimeout, QueueLenght); // This method set the communication with the following vars;
        //                              Serial Port, Baud Rates, Read Timeout and QueueLenght.
        myDevice.connect (); // This method open the Serial communication with the vars previously given.
    }

    public void ConnectWifi(string ssid, string pass)
    {
        myDevice.send("WIFI:"+ssid+":"+pass);
    }

    public void DisconnectHandshake()
    {
        myDevice.send("Disconnect:0");
    }
}
