using System;
using UnityEngine;

namespace DataTypes
{
    public class ProgressionInfo
    {
        public GameObject Obj;    
        public int ProgressionLevel;    //At which point in the progression is this
        public Vector3 Position;    //To which position is the object to move
        public Action Code;
        
        public ProgressionInfo(GameObject obj, int progressionLevel, Vector3 position,Action code)
        {
            Obj = obj;
            ProgressionLevel = progressionLevel;
            Position = position;
            Code = code;
        }
    }
}