using System;
using UnityEngine;

namespace RPG.Saving
{    
    [Serializable]
    public class SerializableVector3 
    {
        readonly float x, y, z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}

