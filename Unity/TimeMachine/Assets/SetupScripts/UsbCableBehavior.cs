
using System;
using UnityEngine;


public class UsbCableBehavior : MonoBehaviour
{
    private Vector3 startPos;
    
    private void Start()
    {
        startPos = transform.localPosition;
    }

    public void EaseAway()
    {
        LeanTween.moveLocal(gameObject, new Vector3(-532.3f, -377.6f, 0), 2f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void EaseIn()
    {
        LeanTween.moveLocal(gameObject, startPos, 2f).setEase(LeanTweenType.easeInOutQuad);
    }
}
