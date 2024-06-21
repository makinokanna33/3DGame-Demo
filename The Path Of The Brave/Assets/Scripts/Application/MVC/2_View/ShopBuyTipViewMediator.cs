using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShopBuyTipViewMediator : Mediator
    {
        public static new string NAME = "ShopBuyTipViewMediator";

        public ShopBuyTipView MyViewComponent { get { return ViewComponent as ShopBuyTipView; } }

        private ShopBuyData currentShopBuyData;
        public ShopBuyTipViewMediator() : base(NAME)
        {

        }
        public override void SetView(object obj)
        {
            ShopBuyTipView shopBuyTipView = (obj as GameObject).GetComponent<ShopBuyTipView>();
            ViewComponent = shopBuyTipView;

            shopBuyTipView.cancelButton.onClick.AddListener(() =>
            {
                shopBuyTipView.gameObject.SetActive(false);
            });

            shopBuyTipView.sureButton.onClick.AddListener(() =>
            {
                shopBuyTipView.gameObject.SetActive(false);
                SendNotification(AppConst.C_ShopBuySure, new ShopArgs(currentShopBuyData));
            });
            
            // 实例化后先隐藏自己
            shopBuyTipView.gameObject.SetActive(false);
        }

        public void ShowBuyTip(ShopBuyData shopBuyData, string textCount)
        {
            currentShopBuyData = shopBuyData;
            MyViewComponent.UpdateInfo(currentShopBuyData.itemData.configData.itemName, textCount);
            MyViewComponent.gameObject.SetActive(true);
        }

        public void ClearPanel()
        {
            ViewComponent = null;
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_ShowShopBuyTip,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_ShowShopBuyTip:
                    ShopArgs shopArgs = notification.Body as ShopArgs;
                    ShowBuyTip(shopArgs.shopBuyData, shopArgs.textCount);
                    break;
            }
        }
    }
}