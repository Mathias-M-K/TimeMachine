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
    public string rawValue;
    public float val1;
    public float val2;
    public bool connected;

    public string status = "Disconnected";
    private string queuedStatus = "Lost Connection";    //message queued 
    
    private int currentPing = 0;
    
    private TcpClient client;
    private NetworkStream theStream;
    private StreamWriter theWriter;
    private StreamReader theReader;

    private Thread myThread;
    private bool connectedMessageDelivered;
    private bool externalDisconnectPrompt;
    
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
            catch (Exception e)
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
                    Debug.Log("Here");
                    // ignored
                }


                // We split readings with a carriage return, so check for it 
                if (read == 13)
                {
                    // Once we have a reading, convert our buffer to a string, since the values are coming as strings
                    var str = Encoding.ASCII.GetString(buffer.ToArray());

                    rawValue = str;
                    // We assume that they're floats
                    var temp1 = float.Parse(str.Split(':')[1]);
                    var temp2 = float.Parse(str.Split(':')[2]);
                    var temp3 = float.Parse(str.Split(':')[3]);

                    val1 = temp1;
                    val2 = temp2;
                    pingResponse = temp3;


                    //Checking for disconnect value
                    if (temp2 == 1 || externalDisconnectPrompt)
                    {
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
            }

            status = queuedStatus;
            queuedStatus = "Lost Connection";
            connectedMessageDelivered = false;
            externalDisconnectPrompt = false;
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
        catch (Exception e)
        {
            Debug.Log("Found it");
            throw;
        }
        
    }

    public void PingDevice()
    {
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

        Thread waitForPing = new Thread(() =>
        {
            WriteToArduino($"Ping:{currentPing}");
            
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (pingResponse != currentPing)
            {
                if (sw.Elapsed.Seconds > 2)
                {
                    CloseConnection("Lost Connection");
                    return;
                }
            }

            pingTime = sw.Elapsed.Milliseconds;
        });

        waitForPing.Start();
    }

    public void ContinuousPinging(object state)
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
    }
}