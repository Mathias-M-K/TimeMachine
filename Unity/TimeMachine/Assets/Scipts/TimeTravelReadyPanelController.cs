using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeTravelReadyPanelController : MonoBehaviour
{
    public static TimeTravelReadyPanelController timeTravelReadyPanelController;

    private void Awake()
    {
        if (timeTravelReadyPanelController != null)
        {
            Destroy(this);
        }
        else
        {
            timeTravelReadyPanelController = this;
        }
    }


    [Header("TimeTravelReadyField")]
    public Image border;
    public TextMeshProUGUI timeTravelReadyField;
    public TextMeshProUGUI timeTravelReadyFieldHelpText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void TimeTravelStart()
    {
        timeTravelReadyField.text = "travelling!";
        timeTravelReadyFieldHelpText.text = "";
        print("Tweening color");
        LeanTween.color(border.rectTransform, new Color32(42, 157, 143, 255), 0.2f).setLoopPingPong(30).setOnComplete(
            () =>
            {
                timeTravelReadyField.text = "Timetravel Ready";
                timeTravelReadyFieldHelpText.text = "Press space to timetravel";
            });
        //LeanTween.color(timeTravelReadyField.rectTransform, new Color32(42, 157, 143, 255), 0.2f).setLoopPingPong(30);

    }
}
