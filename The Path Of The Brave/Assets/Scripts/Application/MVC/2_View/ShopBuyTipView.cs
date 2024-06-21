using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class ShopBuyTipView : MonoBehaviour
    {
        public Text textInfo;
        public Text textCount;

        public Button sureButton;
        public Button cancelButton;

        public void UpdateInfo(string shopName, string textPrice)
        {
            // 该UI将显示在最前面
            transform.SetAsLastSibling();

            textInfo.text = string.Format("购买商品【{0}】", shopName);
            textCount.text = textPrice;
        }
    }
}

