using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "StarAwardsConfiguration", menuName = "Configuration/StarAwardsConfiguration")]
    public class StarAwardsSOData : ScriptableObject
    {
        public List<StarAwardSOData> soDataList;
    }
}