using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "StarAwardConfiguration", menuName = "Configuration/StarAwardConfiguration")]
    public class StarAwardSOData : ScriptableObject
    {
        public StarAwardConfigData configData;
    }
}