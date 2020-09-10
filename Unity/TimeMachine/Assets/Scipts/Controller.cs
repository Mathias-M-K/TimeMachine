using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public string ipAddress;
    public int port;

    
    [SerializeField] private string incomingRaw;
    [SerializeField] private float btn1;
    [SerializeField] private float btn2;
    [SerializeField] private float pingTime;


    private bool connected;    
    private readonly WifiConnection _comm = new WifiConnection();
    
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI pingShow;
    private bool _showingConnected;

    public Image img;
    private bool userPanelActive = false;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (userPanelActive)
            {
                LeanTween.moveLocalX(img.gameObject, 2160, 1).setEase(LeanTweenType.easeInOutQuad);
                userPanelActive = false;
            }
            else
            {
                LeanTween.moveLocalX(img.gameObject, 0, 1).setEase(LeanTweenType.easeInOutQuad);
                userPanelActive = true;
            }
            
        }
        
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            _comm.WriteToArduino("Servo:95");
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _comm.WriteToArduino("Servo:1");
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _comm.WriteToArduino("Servo:180");
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _comm.StartContinuousPinging();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _comm.StopContinuousPinging();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ConnectToEsp();
        }

        statusText.text = _comm.status;
        connected = _comm.connected;

        incomingRaw = _comm.incomingDataStream;
        btn1 = float.Parse(_comm.incomingDataStream.Split(':')[1]);
        btn2 = float.Parse(_comm.incomingDataStream.Split(':')[2]);
        
        pingTime = _comm.pingTime;
        pingShow.text = pingTime.ToString();
    }

    public void ConnectToEsp()
    {
        if (connected)
        {
            return;
        }
        _comm.Begin(ipAddress, port);
    }

    public void DisconnectEsp()
    {
        _comm.CloseConnection("Manually Disconnected");
    }
    

    public void PingDeviceButton()
    {
        _comm.PingDevice();
    }

    public void ServoUp()
    {
        print("Servo Up!");
        _comm.WriteToArduino($"Servo:1");
    }
    public void ServoDown()
    {
        print("Servo Down!");
        _comm.WriteToArduino($"Servo:180");
    }

    public void ServoStop()
    {
        print("Servo Stop!");
        _comm.WriteToArduino("Servo:95");
    }

    public void PrintTest()
    {
        Debug.Log("Pointer lifted");
    }
}