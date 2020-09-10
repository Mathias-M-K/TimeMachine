using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;


public class WifiConnection
{
    public string incomingDataStream = "VAL:0:0:Status_Message:0:0";
    public bool connected;

    public string status = "Disconnected";
    private string queuedStatus = "Default Message";    //message queued 
    
    private int currentPing = 0;
    
    private TcpClient client;
    private NetworkStream theStream;
    private StreamWriter theWriter;
    private StreamReader theReader;

    private Thread myThread;
    private bool connectedMessageDelivered;
    private bool externalDisconnectPrompt;
    private bool initialPingCompleted;

    //Ping values
    public float pingTime = 0;    //Current ping
    private float pingResponse = 0;    //Ping-response received from external device
    private Timer timer;

    public WifiConnection()
    {
        timer = new Timer(ContinuousPinging,"whatever",-1,-1);
    }


    public void Begin(string ipAddress, int port)
    {
        connectedMessageDelivered = false;
        externalDisconnectPrompt = false;


        // Give the network stuff its own special thread
        myThread = new Thread(() =>
        {
            //network stuff
            client = new TcpClient();

            //Read IP and port from arduino terminal
            try
            {
                status = "Attempting connection...";
                client.Connect(ipAddress, port);
            }
            catch (Exception)
            {
                if (externalDisconnectPrompt)
                {
                    status = "Connection canceled";
                    externalDisconnectPrompt = false;
                    return;
                }

                Begin(ipAddress, port);

                return;
            }


            theStream = client.GetStream();


            theReader = new StreamReader(theStream);
            theWriter = new StreamWriter(theStream);


            // We'll read values and buffer them up in here
            var buffer = new List<byte>();

            status = "Connected";
            

            while (client.Connected)
            {
                if (!connectedMessageDelivered)
                {
                    Debug.Log($"Connected to {ipAddress}");
                    connectedMessageDelivered = true;
                }

                connected = true;

                // Read the next byte
                int read = 0;
                try
                {
                     read = theReader.Read();
                }
                catch (Exception e)
                {
                    Debug.Log($"Read exception error {e}");

                    if (!queuedStatus.Equals("Lost Connection"))
                    {
                        CloseConnection("Device reset");
                    }
                }


                // We split readings with a carriage return, so check for it 
                if (read == 13)
                {
                    // Once we have a reading, convert our buffer to a string, since the values are coming as strings
                    var str = Encoding.ASCII.GetString(buffer.ToArray());

                    incomingDataStream = str;
                    // We assume that they're floats

                    var disconnectPromptValue = float.Parse(str.Split(':')[4]);
                    pingResponse = float.Parse(str.Split(':')[5]);


                    //Checking for disconnect value
                    if (disconnectPromptValue == 1 || externalDisconnectPrompt)
                    {
                        Debug.Log($"Client connection force closed: temp2:{disconnectPromptValue}, externalDisconnectprompt:{externalDisconnectPrompt}, QueuedStatus:{queuedStatus}");
                        break;
                    }


                    // Clear the buffer ready for another reading
                    buffer.Clear();
                }
                else
                {
                    // If this wasn't the end of a reading, then just add this new byte to our buffer
                    buffer.Add((byte) read);
                }

                if (!initialPingCompleted)
                {
                    PingDevice();
                    initialPingCompleted = true;
                }
            }
            
            Debug.Log("Exiting while-loop");
            status = queuedStatus;
            queuedStatus = "Disconnected";
            connectedMessageDelivered = false;
            externalDisconnectPrompt = false;
            
            Debug.Log($"ExternalDisconnectPrompt-End:{externalDisconnectPrompt}");
            connected = false;
        });


        myThread.Start();
    }

    public void WriteToArduino(string dataOut)
    {
        try
        {
            theWriter.Write(dataOut);
            theWriter.Flush();
        }
        catch (Exception)
        {
            Debug.Log($"Found it");
            throw;
        }
        
    }

    public void PingDevice()
    {
        if (!connected) return;

        Random rnd = new Random();
        int newPing = rnd.Next(0,254);
        if (newPing == currentPing)
        {
            currentPing++;
        }
        else
        {
            currentPing = newPing;
        }
        
        Debug.Log($"Pinging Device!");
        
        Thread waitForPing = new Thread(() =>
        {
            WriteToArduino($"Ping:{currentPing}");
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (pingResponse != currentPing)
            {
                if (sw.Elapsed.Seconds > 2)
                {
                    Debug.Log($"Ping timed out!");

                    if (!connected) return;

                    status = "Ping timed out";
                    CloseConnection("Lost Connection");
                    
                    return;
                }
            }

            Debug.Log("Ping response received!");
            
            pingTime = sw.Elapsed.Milliseconds;
        });

        waitForPing.Start();
    }

    private void ContinuousPinging(object state)
    {
        if (!connected)
        {
            StopContinuousPinging();
            return;
        }
        
        PingDevice();
    }
    
    public void StartContinuousPinging()
    {
        timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }
    
    public void StopContinuousPinging()
    {
        timer.Change(-1, -1);
    }

    public void CloseConnection(string disconnectStatus)
    {
        StopContinuousPinging();
        queuedStatus = disconnectStatus;
        externalDisconnectPrompt = true;
        client.Close();
        
    }
}