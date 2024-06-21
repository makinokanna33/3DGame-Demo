using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [CreateAssetMenu(fileName = "ShopBuyConfiguration", menuName = "Configuration/ShopBuyConfiguration")]
    public class ShopBuySOData : ScriptableObject
    {
        public List<ShopBuyConfigData> datas;
    }
}

