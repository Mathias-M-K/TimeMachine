using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DestinationSelectController : MonoBehaviour
{
    public static DestinationSelectController destinationController;

    private void Awake()
    {
        if (destinationController != null)
        {
            Destroy(this);
        }
        else
        {
            destinationController = this;
        }
    }

    public TMP_InputField day;
    public TMP_InputField month;
    public TMP_InputField year;


    private TMP_InputField activeField;

    private bool automaticProgress = true;

    private void Start()
    {
        activeField = day;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && activeField.text.Length <= 0)
        {
            if (activeField == year)
            {
                SelectField(month);
            }else if (activeField == month)
            {
                SelectField(day);
            }
        }
    }

    public void DayNewInput(string s)
    {
        if (s.Length >= 2)
        {
            SelectField(month);
        }
    }
    public void MonthNewInput(string s)
    {
        if (s.Length >= 2)
        {
            SelectField(year);
        }
    }
    public void YearNewInput(string s)
    {
        if (s.Length >= 4)
        {
            print("Done");
            UserInterface.interfaceCtrl.TimeTravel();
        }
    }

    public void ResetFields()
    {
        print("Resetting fields");

        automaticProgress = true;
        day.text = "";
        month.text = "";
        year.text = "";

        SelectField(day);
    }

    void SelectField(TMP_InputField field)
    {
        field.Select();
        field.ActivateInputField();

        activeField = field;
    }
}