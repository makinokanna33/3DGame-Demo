using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "ItemsBaseConfiguration", menuName = "Configuration/ItemsBaseConfiguration")]
    public class ItemsBaseSOData : ScriptableObject
    {
        public List<ItemBaseSOData> configDataList;
    }
}
