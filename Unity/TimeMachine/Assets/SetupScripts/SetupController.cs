
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Threading;
using DataTypes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class SetupController : MonoBehaviour
{
    public static SetupController staticSetupController;
    
    public UsbCableBehavior usbCable;
    public CableConnectionInfoController cableConnectionController;
    public WifiPanelManager wifiPanelManager;
    public ArduinoComms espComm;

    private List<ProgressionInfo> _progressionInfo = new List<ProgressionInfo>();
    private string[] comPortsBefore;
    [SerializeField] private bool espConnected = false;
    private bool enactProgressionChangeExternal;
    private bool progressionButtonsEnabled = true;
    private bool resetWifiPanel;
    private bool loadNextScene;

    private int progression;    //0. Welcome Screen, 1. Disconnect USB, 2. Connect Usb, 3. choose wifi 4. end 5. nextScene
    
    /*
     * Elements
     */
    public GameObject pc;
    public GameObject title;
    public GameObject subtitle;
    public GameObject cableConnectionInfoPanel;
    public GameObject wifiPanel;
    public GameObject stepOneDescription;
    public GameObject stepTwoDescription;


    public GameObject buffer;
    
    void Awake()
    {
        if(staticSetupController != null)
            Destroy(staticSetupController);
        else
            staticSetupController = this;
         
        //DontDestroyOnLoad(this);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //PC-USB cable animation
        _progressionInfo.Add(new ProgressionInfo(pc,0,new Vector3(-1435,-132f,0), () =>
        {
            usbCable.EaseIn();
        }));
        _progressionInfo.Add(new ProgressionInfo(pc,1,new Vector3(-492.87f,-79.246f,0), () =>
        {
            usbCable.EaseAway();
        }));
        _progressionInfo.Add(new ProgressionInfo(pc,2,new Vector3(-492.87f,-79.246f,0), () =>
        {
            usbCable.EaseIn();
        }));
        _progressionInfo.Add(new ProgressionInfo(pc,4,new Vector3(-1435,-132f,0), () =>
        {
            usbCable.EaseIn();
        }));
        
        //Title
        _progressionInfo.Add(new ProgressionInfo(title,0,new Vector3(0,76.2f,0),null));
        _progressionInfo.Add(new ProgressionInfo(title,1,new Vector3(492,76.219f,0),null));
        _progressionInfo.Add(new ProgressionInfo(title,4,new Vector3(0,76.2f,0),null));

        //Subtitle
        _progressionInfo.Add(new ProgressionInfo(subtitle,0,new Vector3(0,-25,0),null));
        _progressionInfo.Add(new ProgressionInfo(subtitle,1,new Vector3(0,-500,0),null));
        _progressionInfo.Add(new ProgressionInfo(subtitle,2,new Vector3(0,-800,0), () =>
        {
            subtitle.GetComponent<TextMeshProUGUI>().text = "Setup complete! Press space to continue to time machine!";
        }));
        _progressionInfo.Add(new ProgressionInfo(subtitle,4,new Vector3(0,-25,0),null));
        
        
        //Cable connection panel
        _progressionInfo.Add(new ProgressionInfo(cableConnectionInfoPanel,1,new Vector3(1241,-301,0),null));
        _progressionInfo.Add(new ProgressionInfo(cableConnectionInfoPanel,2,new Vector3(492,-301,0),null));
        _progressionInfo.Add(new ProgressionInfo(cableConnectionInfoPanel,3,new Vector3(492,-800.22998f,0),null));
        
        //Wifi panel
        _progressionInfo.Add(new ProgressionInfo(wifiPanel,2,new Vector3(1236,-162.5f,0),null));
        _progressionInfo.Add(new ProgressionInfo(wifiPanel,3,new Vector3(557.890015f,-162.5f,0),null));
        _progressionInfo.Add(new ProgressionInfo(wifiPanel,4,new Vector3(557.890015f,-800.22998f,0),null));
        
        //Step One Description
        _progressionInfo.Add(new ProgressionInfo(stepOneDescription,0,new Vector3(1248,-88.2350006f,0),null));
        _progressionInfo.Add(new ProgressionInfo(stepOneDescription,1,new Vector3(492,-88.1999969f,0),null));
        _progressionInfo.Add(new ProgressionInfo(stepOneDescription,2,new Vector3(492,-800,0),null));
        
        //Step One Description
        _progressionInfo.Add(new ProgressionInfo(stepTwoDescription,1,new Vector3(1248,-88.2350006f,0),null));
        _progressionInfo.Add(new ProgressionInfo(stepTwoDescription,2,new Vector3(492,-88.1999969f,0),null));
        _progressionInfo.Add(new ProgressionInfo(stepTwoDescription,3,new Vector3(492,-800,0),null));
        
        //Other assignments
        _progressionInfo.Add(new ProgressionInfo(null,2,new Vector3(0,0,0), () =>
        {
            print("Getting current COMports");
            DetermineAndConnectToPort();
        }));
        
        _progressionInfo.Add(new ProgressionInfo(null,1,new Vector3(0,0,0), () =>
        {
            progressionButtonsEnabled = true;
        }));
        _progressionInfo.Add(new ProgressionInfo(null,2,new Vector3(0,0,0), () =>
        {
            progressionButtonsEnabled = false;
        }));
        
        _progressionInfo.Add(new ProgressionInfo(null,4,new Vector3(0,0,0), () =>
        {
            progressionButtonsEnabled = true;
        }));
        
        _progressionInfo.Add(new ProgressionInfo(null,5,new Vector3(0,0,0), () =>
        {
            espComm.DisconnectHandshake();
            Thread disconnectThread = new Thread(() =>
            {
                print("Waiting for time machine to disconnect");
                while (espComm.incomingData.Split(':')[0] != "Goodbye")
                {
                    
                }
                loadNextScene = true;
            });
            
            disconnectThread.Start();
            
        }));

        EnactProgressionChange();
    }

    // Update is called once per frame
    void Update()
    {
        cableConnectionController.SetTimeMachineDetected(espConnected);
        
        if (Input.GetKeyDown(KeyCode.Space) && progressionButtonsEnabled)
        {
            progression++;
            EnactProgressionChange();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && progressionButtonsEnabled)
        {
            progression--;
            EnactProgressionChange();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            cableConnectionController.SetTimeMachineDetected(true);
        }
        
        if (enactProgressionChangeExternal)
        {
            EnactProgressionChange();
            enactProgressionChangeExternal = false;
        }

        if (resetWifiPanel)
        {
            wifiPanelManager.ResetWifiField();
            resetWifiPanel = false;
        }

        if (loadNextScene)
        {
            LoadTimeMachineScene();
        }
    }

    public void ConnectToWifi(string ssid,string pass)
    {
        espComm.ConnectWifi(ssid,pass);
        wifiPanelManager.SetAllInactive();
        Thread wifiThread = new Thread(() =>
        {
            print("Waiting for ESP to attempt connection");
            
            while (espComm.incomingData.Split(':')[0] != "Connecting")
            {
                
            }
            
            print("Waiting for wifi");
            while (espComm.incomingData.Split(':')[0] == "Connecting")
            {
                
            }

            if (espComm.incomingData.Split(':')[0] == "Connected")
            {
                print("Connected to wifi!");
                DataTransferClass.staticDataTransferClass.ip = espComm.incomingData.Split(':')[1];
                DataTransferClass.staticDataTransferClass.port = espComm.incomingData.Split(':')[2];
                DataTransferClass.staticDataTransferClass.dataRead = true;
                progression++;
                enactProgressionChangeExternal = true;


            }else if (espComm.incomingData.Split(':')[0] == "Connection Failed")
            {
                print("Wifi connection failed");
                resetWifiPanel = true;
            }
            else
            {
                print("Weird error..?");
            }
        });
        
        wifiThread.Start();
    }

    private void DetermineAndConnectToPort()
    {
        comPortsBefore = SerialPort.GetPortNames();

        print("Searching for timemachine");
        Thread myThread = new Thread(() =>
        {
            while (!espConnected)
            {
                string[] newPorts = SerialPort.GetPortNames();
                //print("Searching");
                foreach (string s in newPorts)
                {
                    //print($"Checking if {s} exist: {_comPortsBefore.Contains(s)}");
                    if (!comPortsBefore.Contains(s))
                    {
                        print($"Port found! Connecting to {s}");
                        Thread.Sleep(3000);
                        espComm.Begin(s);
                        while (espComm.incomingData.Split(':')[0] != "Ready")
                        {
                            print("Waiting for esp");
                        }
                        print("Connected!");
                        espConnected = true;
                        enactProgressionChangeExternal = true;

                        progression++;

                        return;
                    }
                }
            }
        });
        
        myThread.Start();
    }
    
    private void EnactProgressionChange()
    {
        print($"Progression level={progression}");
        foreach (ProgressionInfo progressionPosition in _progressionInfo)
        {
            if (progressionPosition.ProgressionLevel == progression)
            {
                if (progressionPosition.Obj != null)
                {
                    LeanTween.moveLocal(progressionPosition.Obj, progressionPosition.Position, 1).setEase(LeanTweenType.easeInOutQuad).setOnComplete(
                        () =>
                        {
                            progressionPosition.Code?.Invoke();
                        });
                }
                else
                {
                    progressionPosition.Code.Invoke();
                }
                
            }
        }
    }

    private void LoadTimeMachineScene()
    {
        SceneManager.LoadScene(1);
    }

    
}
