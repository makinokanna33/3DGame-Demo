using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShowShopBuyArgs
    {
        public ShopBuyData shopBuyData;

        public ShowShopBuyArgs(ShopBuyData shopBuyData)
        {
            this.shopBuyData = shopBuyData;
        }
    }

}
