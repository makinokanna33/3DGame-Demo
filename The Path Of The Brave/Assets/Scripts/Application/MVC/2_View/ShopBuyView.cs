using FrameWork.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyApplication
{
    public class ShopBuyView : MonoBehaviour
    {
        public GameObject itemGrid;
        public Text nameText;
        public Text costText;
        public Button button;

        [HideInInspector] public ShopBuyData buyData;
        [HideInInspector] public ItemFillerView itemFillerView;
        public void UpdateInfo(ShopBuyData data)
        {
            // 生成一个 ItemFillerView

            nameText.text = data.itemData.configData.itemName;

            buyData = data;

#if UNITY_EDITOR
            itemFillerView = UnityEngine.Object.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<ItemFillerView>("Assets/ABRes/Prefabs/ItemFillerView.prefab"));
#else
            itemFillerView = UnityEngine.Object.Instantiate(ABManager.Instance.LoadRes<ItemFillerView>(AppConst.AB_Prefabs, AppConst.AB_ItemFillerView));
#endif
            // 更新UI信息
            itemFillerView.UpdateInfo(buyData.itemData);

            // 设置父物体
            itemFillerView.gameObject.transform.SetParent(itemGrid.transform, false);

            UpdateState();
        }

        public void UpdateState()
        {
            SaveDataProxy saveDataProxy = GameFacade.Instance.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            if (buyData.configData.buyType == ShopBuyType.Gold)
            {
                if(saveDataProxy.MyData.goldNum >= buyData.configData.buyCount)
                {
                    costText.text = string.Format("<color=yellow>{0}金币</color>", buyData.configData.buyCount.ToString());
                    button.interactable = true;
                }
                else
                {
                    costText.text = string.Format("<color=red>{0}金币</color>", buyData.configData.buyCount.ToString());
                    button.interactable = false;
                }
            }
            else if (buyData.configData.buyType == ShopBuyType.Crystal)
            {
                if (saveDataProxy.MyData.crystalNum >= buyData.configData.buyCount)
                {
                    costText.text = string.Format("<color=yellow>{0}晶石</color>", buyData.configData.buyCount.ToString());
                    button.interactable = true;
                }
                else
                {
                    costText.text = string.Format("<color=red>{0}晶石</color>", buyData.configData.buyCount.ToString());
                    button.interactable = false;
                }
            }
        }

        public void OnButtonClick()
        {
            GameFacade.Instance.SendNotification(AppConst.C_ShowShopBuyTip, new ShopArgs(buyData, costText.text));
        }
    }
}

