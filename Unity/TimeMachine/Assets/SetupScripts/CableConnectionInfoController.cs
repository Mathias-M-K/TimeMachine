using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CableConnectionInfoController : MonoBehaviour
{
    public GameObject loadingCircle;
    public GameObject checkmark;
    public TextMeshProUGUI text;


    // Update is called once per frame
    void Update()
    {
        loadingCircle.transform.Rotate(new Vector3(0, 0, 100 * Time.deltaTime));
    }

    public void SetTimeMachineDetected(bool detected)
    {
        checkmark.SetActive(detected);
        loadingCircle.SetActive(!detected);

        if (detected)
        {
            text.text = "Time-machine detected!";
        }
        else
        {
            text.text = "Looking for time-machine";
        }
    }
}