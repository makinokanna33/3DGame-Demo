using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "SelectLevelsConfiguration", menuName = "Configuration/SelectLevelsConfiguration")]
    public class SelectLevelsSOData : ScriptableObject
    {
        public List<SelectLevelSOData> selectLevelSo;
    }
}


