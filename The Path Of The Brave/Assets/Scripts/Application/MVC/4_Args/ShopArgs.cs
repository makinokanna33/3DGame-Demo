using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShopArgs
    {
        public ShopBuyData shopBuyData;
        public string textCount;
        public ShopArgs(ShopBuyData shopBuyData, string textCount = "")
        {
            this.shopBuyData = shopBuyData;
            this.textCount = textCount;
        }
    }

    public class ShopSellArgs
    {
        public ItemData itemData;
        public int gridIndex;
        public int count;
        public ShopSellArgs(ItemData itemData, int gridIndex, int count = 1)
        {
            this.itemData = itemData;
            this.gridIndex = gridIndex;
            this.count = count;
        }
    }
}

