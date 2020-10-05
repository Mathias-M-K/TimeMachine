using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransferClass : MonoBehaviour
{
    public static DataTransferClass staticDataTransferClass;

    public string ip;
    public string port;
    public bool dataRead;
    
    
    void Awake()
    {
        if(staticDataTransferClass != null)
            Destroy(staticDataTransferClass);
        else
            staticDataTransferClass = this;
         
        DontDestroyOnLoad(this);
    }

}
