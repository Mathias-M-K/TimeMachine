
using System;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

public class WifiPanelManager : MonoBehaviour
{
    public TMP_InputField ssid;
    public TMP_InputField password;
    public Button connectBtn;
    
    private TMP_InputField activeField;

    private bool forceInactive;

    private void Start()
    {
        activeField = ssid;
    }


    // Update is called once per frame
    void Update()
    {
        if (!forceInactive)
        {
            if (ssid.text != "" && password.text != "")
            {
                connectBtn.interactable = true;
            }
            else
            {
                connectBtn.interactable = false;
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (activeField == ssid)
            {
                SetFieldActive(password);
            }
            else
            {
                SetFieldActive(ssid);
            }
        }
    }

    public void SetAllInactive()
    {
        forceInactive = true;
        
        connectBtn.interactable = false;
        ssid.interactable = false;
        password.interactable = false;
    }

    public void ResetWifiField()
    {
        forceInactive = false;
        
        ssid.text = "";
        ssid.interactable = true;

        password.text = "";
        password.interactable = true;
    }

    public void ConnectBtnPressed()
    {
        print("Connecting to wifi");
        SetupController.staticSetupController.ConnectToWifi(ssid.text,password.text);
    }

    private void SetFieldActive(TMP_InputField inputField)
    {
        inputField.Select();
        inputField.ActivateInputField();

        activeField = inputField;
    }
}
