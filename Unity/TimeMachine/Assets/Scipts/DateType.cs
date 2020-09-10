using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateType
{
    public int TriggerYear;
    public int Day;
    public int Month;
    public int Year;
    public int Hour;
    public int Min;
    public int Sec;

    public DateType(int triggerYear,int day, int month, int year, int hour, int min, int sec)
    {
        TriggerYear = triggerYear;
        Day = day;
        Month = month;
        Year = year;
        Hour = hour;
        Min = min;
        Sec = sec;
    }
}
