using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public Controller controller;

    //List
    private readonly List<int> years = new List<int>() {1791, 1995, 2020, 2029};    //Years the machine is able to travel to
    private readonly List<DateType> dates = new List<DateType>();

    //Status fields
    public TextMeshProUGUI timeTravelConnectionStatus;
    public TextMeshProUGUI gateStatus;
    public TextMeshProUGUI timeTravelPowerStatus;
    public TextMeshProUGUI fluxCapacitorChargeStatus;

    //Present day field
    [Header("Present Day")] public TextMeshProUGUI presentDay;
    public TextMeshProUGUI presentMonth;
    public TextMeshProUGUI presentYear;
    public TextMeshProUGUI presentHour;
    public TextMeshProUGUI presentMin;
    public TextMeshProUGUI presentSec;

    //Destination day field
    [Header("Destination")] public TextMeshProUGUI destinationDay;
    public TextMeshProUGUI destinationMonth;
    public TextMeshProUGUI destinationYear;
    public TextMeshProUGUI destinationHour;
    public TextMeshProUGUI destinationMin;
    public TextMeshProUGUI destinationSec;


    /**
     * Variables
     */
    //Timemachine
    private bool timeMachineActive;
    private float activeTime;
    
    private bool destinationReady;
    private bool gateReady;

    //Flux Capacitor
    private bool fluxCapacitorCharged = false;
    private bool fluxCapacitorCharging;
    private float fluxCapacitorInterpolationPeriod = 0.5f;
    private float fluxCapacitorChargeTime;
    private float dotCounter = 0;
    private float fluxCapacitorChargeProgression = 0;

    //DestinationPanel
    [Header("DestinationFields")] public Image destinationPanel;
    public TextMeshProUGUI messageField;
    public TMP_InputField destinationYearInput;

    private DateType destinationDate;
    private bool destinationPanelActive;
    private float messageActiveTime = 0;
    private bool yearAccepted;
    private int timeTravelYear = 0;
    private float destinationCalculation = 0;
    private bool destinationFieldShouldBeUpdated;
    
    //Time travel prompt
    [Header("Bottom Prompt")]
    public GameObject timeTravelPrompt;
    private bool timeTravelPromptActive = true;

    //Colors
    private readonly Color32 orangeNegative = new Color32(231, 111, 81, 255);
    private readonly Color32 orangeMild = new Color32(233, 196, 106, 255);
    private readonly Color32 blue = new Color32(81, 186, 231, 255);


    private void Start()
    {
        //April 27, 1791
        dates.Add(new DateType(1995, 30, 11, 1995, 2, 00, 00));
        dates.Add(new DateType(1791, 27, 4, 1791, 22, 30, 00));
        dates.Add(new DateType(2029, DateTime.Now.Day, DateTime.Now.Month, 2029, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePresentDayFields();
        
        if (destinationFieldShouldBeUpdated)
        {
            UpdateDestinationFields();
        }

        if (gateReady && fluxCapacitorCharged && timeMachineActive && destinationReady)
        {
            TweenInReadyPrompt();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.InitiateTimeTravel();
            }
        }
        else
        {
            TweenOutReadyPrompt();
        }

        messageActiveTime += Time.deltaTime;
        destinationCalculation += Time.deltaTime;
        
        
        float msgActiveTime = 3; //How long before message resets
        if (messageActiveTime >= msgActiveTime)
        {
            if (yearAccepted)
            {
                messageField.text = "Accepted";
                SetDestinationFields("N/A",orangeNegative);
                destinationCalculation = 0;
                destinationDate = FetchDateData(timeTravelYear);
                destinationFieldShouldBeUpdated = true;
                LeanTween.moveLocalY(destinationPanel.gameObject, -1137f, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                destinationPanelActive = false;
                yearAccepted = false;
            }
            else
            {
                messageField.text = "Enter destination year";
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gateStatus.text = "open";
            gateStatus.color = blue;
            gateReady = true;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gateStatus.text = "closed";
            gateStatus.color = orangeNegative;
            gateReady = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (destinationPanelActive)
            {
                if (CheckDestinationYear(int.Parse(destinationYearInput.text)))
                {
                    messageField.text = "Calculating Destination";
                    yearAccepted = true;
                    timeTravelYear = int.Parse(destinationYearInput.text);
                    messageActiveTime = 0;
                }
                else
                {
                    messageField.text = "Denied";
                    messageActiveTime = 0;
                }
            }
            else
            {
                LeanTween.moveLocalY(destinationPanel.gameObject, 0, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                destinationPanelActive = true;
                messageField.text = "Enter destination year";
                destinationYearInput.text = "";
                destinationYearInput.Select();
                destinationYearInput.ActivateInputField();
            }
        }

        

        if (controller.btn1 == 1)
        {
            timeTravelConnectionStatus.text = "Active";
            timeTravelConnectionStatus.color = blue;
            timeMachineActive = true;
            activeTime += Time.deltaTime;
        }
        else
        {
            timeTravelConnectionStatus.text = "Disconnected";
            timeTravelConnectionStatus.color = orangeNegative;
            timeMachineActive = false;
            activeTime = 0;
        }

        if (controller.btn2 == 1)
        {
            timeTravelPowerStatus.text = "On";
            timeTravelPowerStatus.color = blue;

            if (!fluxCapacitorCharged)
            {
                if (!fluxCapacitorCharging)
                {
                    fluxCapacitorCharging = true;
                    fluxCapacitorChargeStatus.text = "Charging";
                }


                fluxCapacitorChargeTime += Time.deltaTime;
                fluxCapacitorChargeProgression += Time.deltaTime;

                if (fluxCapacitorChargeTime >= fluxCapacitorInterpolationPeriod)
                {
                    fluxCapacitorChargeStatus.text += ".";
                    dotCounter++;

                    int dotCount = 6;

                    if (dotCounter > dotCount)
                    {
                        fluxCapacitorChargeStatus.text = "Charging";
                        dotCounter = 0;
                    }

                    fluxCapacitorChargeTime -= fluxCapacitorInterpolationPeriod;
                }

                float chargeTime = 4;

                if (fluxCapacitorChargeProgression >= chargeTime)
                {
                    fluxCapacitorChargeStatus.text = "Charged";
                    fluxCapacitorChargeStatus.color = blue;

                    fluxCapacitorCharged = true;
                    fluxCapacitorCharging = false;
                }
            }
        }
        else
        {
            timeTravelPowerStatus.text = "off";
            timeTravelPowerStatus.color = orangeNegative;

            fluxCapacitorChargeStatus.text = "No charge";
            fluxCapacitorChargeStatus.color = orangeNegative;
            fluxCapacitorCharged = false;
            fluxCapacitorChargeProgression = 0;
        }
    }

    private void UpdatePresentDayFields()
    {
        float activeTimeRequirement = 1;
        float activeTimeIncrement = 0.75f;

        if (timeMachineActive)
        {
            if (activeTime < activeTimeRequirement) return;
            activeTimeRequirement += activeTimeIncrement;
            SetTextField(presentDay, DateTime.Now.Day.ToString(), orangeMild);


            if (activeTime < activeTimeRequirement) return;
            activeTimeRequirement += activeTimeIncrement;
            SetTextField(presentMonth, DateTime.Now.Month.ToString(), orangeMild);


            if (activeTime < activeTimeRequirement) return;
            activeTimeRequirement += activeTimeIncrement;
            SetTextField(presentYear, DateTime.Now.Year.ToString(), orangeMild);


            if (activeTime < activeTimeRequirement) return;
            activeTimeRequirement += activeTimeIncrement;
            SetTextField(presentHour, DateTime.Now.Hour.ToString(), orangeMild);

            if (activeTime < activeTimeRequirement) return;
            activeTimeRequirement += activeTimeIncrement;
            SetTextField(presentMin, DateTime.Now.Minute.ToString(), orangeMild);

            if (activeTime < activeTimeRequirement) return;
            SetTextField(presentSec, DateTime.Now.Second.ToString(), orangeMild);
        }
        else
        {
            SetPresentDayFields("N/A", orangeNegative);
        }
    }
    private void SetPresentDayFields(string text, Color32 color)
    {
        SetTextField(presentDay, text, color);
        SetTextField(presentMonth, text, color);
        SetTextField(presentYear, text, color);
        SetTextField(presentHour, text, color);
        SetTextField(presentMin, text, color);
        SetTextField(presentSec, text, color);
    }

    private void UpdateDestinationFields()
    {
        float timeRequirement = 1;
        float timeIncrement = 0.75f;

        if (destinationCalculation < timeRequirement) return;
        timeRequirement += timeIncrement;
        SetTextField(destinationDay, destinationDate.Day.ToString("00"), orangeMild);

        if (destinationCalculation < timeRequirement) return;
        timeRequirement += timeIncrement;
        SetTextField(destinationMonth, destinationDate.Month.ToString("00"), orangeMild);

        if (destinationCalculation < timeRequirement) return;
        timeRequirement += timeIncrement;
        SetTextField(destinationYear, destinationDate.Year.ToString("00"), orangeMild);

        if (destinationCalculation < timeRequirement) return;
        timeRequirement += timeIncrement;
        SetTextField(destinationHour, destinationDate.Hour.ToString("00"), orangeMild);

        if (destinationCalculation < timeRequirement) return;
        timeRequirement += timeIncrement;
        SetTextField(destinationMin, destinationDate.Min.ToString("00"), orangeMild);

        if (destinationCalculation < timeRequirement) return;
        SetTextField(destinationSec, destinationDate.Sec.ToString("00"), orangeMild);
        destinationFieldShouldBeUpdated = false;
        destinationReady = true;
    }
    private void SetDestinationFields(string text, Color32 color)
    {
        SetTextField(destinationDay, text, color);
        SetTextField(destinationMonth, text, color);
        SetTextField(destinationYear, text, color);
        SetTextField(destinationHour, text, color);
        SetTextField(destinationMin, text, color);
        SetTextField(destinationSec, text, color);
        destinationReady = false;
    }

    

    private void SetTextField(TextMeshProUGUI field, string text, Color32 color)
    {
        field.text = text;
        field.color = color;
    }

    private bool CheckDestinationYear(int year)
    {
        foreach (int i in years)
        {
            if (year == i)
            {
                return true;
            }
        }

        return false;
    }

    private DateType FetchDateData(int year)
    {
        foreach (DateType dateType in dates)
        {
            if (dateType.TriggerYear == year)
            {
                Debug.Log($"Returning {dateType.TriggerYear}");
                return dateType;
            }
        }

        return null;
    }
    
    public void TweenOutReadyPrompt()
    {
        if (!timeTravelPromptActive) return;
        LeanTween.moveLocalY(timeTravelPrompt.gameObject, -774, 0.4f).setEase(LeanTweenType.easeOutQuad);
        timeTravelPromptActive = false;
    }

    public void TweenInReadyPrompt()
    {
        if (timeTravelPromptActive) return;
        LeanTween.moveLocalY(timeTravelPrompt.gameObject, -570, 0.4f).setEase(LeanTweenType.easeOutQuad);
        timeTravelPromptActive = true;
    }
}