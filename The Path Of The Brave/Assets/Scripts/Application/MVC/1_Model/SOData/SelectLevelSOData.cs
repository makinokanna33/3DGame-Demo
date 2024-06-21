using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "SelectLevelConfiguration", menuName = "Configuration/SelectLevelConfiguration")]
    public class SelectLevelSOData : ScriptableObject
    {
        public SelectLevelConfigData configData;
    }
}

