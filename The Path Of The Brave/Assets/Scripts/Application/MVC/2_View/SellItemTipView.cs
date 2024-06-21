using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class SellItemTipView : MonoBehaviour
    {
        public GameObject m_Tip;
        public GameObject m_InputNumTip;
        public GameObject m_TextWarning;

        [Header("UI组件")]
        public Text textInfo;
        public Text textWarning;

        public Button btnUp;
        public Button btnDown;

        public Button btnSure;
        public Button btnCancel;

        public InputField inputField;

        public void Init()
        {
            m_Tip.SetActive(false);
            inputField.text = "";
        }

        public void UpdateInfo(SellState delState, ItemData itemData, int num = 1)
        {
            if (delState == SellState.InputNum)
            {
                textInfo.text = string.Format("出售物品【{0}】", itemData.configData.itemName);
                m_InputNumTip.SetActive(true);
                m_TextWarning.SetActive(false);
            }
            else
            {
                textInfo.text = string.Format("即将出售物品【{0}】{1}个", itemData.configData.itemName, num);
                textWarning.text = string.Format("出售价格：{0}金币", itemData.configData.sellPrice * num);
                m_InputNumTip.SetActive(false);
                m_TextWarning.SetActive(true);
            }

            m_Tip.SetActive(true);
            transform.SetAsLastSibling();
        }
    }
}

