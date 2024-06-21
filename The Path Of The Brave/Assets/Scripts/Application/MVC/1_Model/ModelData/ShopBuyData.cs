using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public enum ShopBuyType
    {
        Gold,
        Crystal
    }

    [System.Serializable]
    public class ShopBuyConfigData
    {
        [Tooltip("售卖物品数据")]
        public ItemBaseSOData itemSoData;

        [Tooltip("购买所需物品类型")]
        public ShopBuyType buyType;

        [Tooltip("购买所需物品数量")]
        public int buyCount;
    }

    [System.Serializable]
    public class ShopBuyData
    {
        public ShopBuyConfigData configData;
        public ItemData itemData;

        public ShopBuyData(ShopBuyConfigData configData)
        {
            this.configData = configData;
            itemData = new ItemData(this.configData.itemSoData.itemConfigData, 1, 1);
        }
    }
}

