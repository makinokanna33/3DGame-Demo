using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public enum ShopType
    {
        buy,
        sell
    }
    public class ShopViewMediator : Mediator
    {
        public static new string NAME = "ShopViewMediator";

        public ShopView MyViewComponent { get { return ViewComponent as ShopView; } }

        public ShopType shopType;
        public ShopViewMediator() : base(NAME)
        {
            // ShopBuyViewMediator 的注册在每个需要 shopBuyView 的代理中都要进行判断注册
            if (!Facade.HasMediator(ShopBuyViewMediator.NAME))
                Facade.RegisterMediator(new ShopBuyViewMediator());
            // ItemFillerViewMediator 的注册在每个需要 ItemView 的代理中都要进行判断注册
            if (!Facade.HasMediator(ItemFillerViewMediator.NAME))
                Facade.RegisterMediator(new ItemFillerViewMediator());

            shopType = ShopType.buy;
        }
        public override void SetView(object obj)
        {
            ShopView ShopView = (obj as GameObject).GetComponent<ShopView>();
            ViewComponent = ShopView;

            // 激活面板
            OnEnable();

            ShopView.btnClose.onClick.AddListener(() =>
            {
                ShopView.gameObject.SetActive(false);
            });

            ShopView.togBuy.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    shopType = ShopType.buy;
                    SendNotification(AppConst.C_UpdateShop);
                }

            });

            ShopView.togSell.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    shopType = ShopType.sell;
                    SendNotification(AppConst.C_UpdateShop);
                }
            });
        }
        public override void ShowPanel()
        {
            // 面板未激活时才进行激活显示
            if (!MyViewComponent.gameObject.activeSelf)
                OnEnable();
        }
        private void OnEnable()
        {
            // 激活面板
            MyViewComponent.gameObject.SetActive(true);

            ShowShopContainer();
            
            // 更新UI视图
            SendNotification(AppConst.C_UpdateShop);
        }
        
        public void ShowShopContainer()
        {
            MyViewComponent.buyContainer.gameObject.SetActive(false);
            MyViewComponent.sellContainer.gameObject.SetActive(false);

            switch (shopType)
            {
                case ShopType.buy:
                    MyViewComponent.buyContainer.gameObject.SetActive(true);
                    break;
                case ShopType.sell:
                    MyViewComponent.sellContainer.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        private void ShowItem(ShopBuyData data)
        {
            ShopBuyViewMediator shopBuyViewMediator = Facade.RetrieveMediator(ShopBuyViewMediator.NAME) as ShopBuyViewMediator;
            shopBuyViewMediator.CreateView(data, MyViewComponent.buyContainerContent);
        }
        private void HideItem()
        {
            ShopBuyViewMediator mediator = Facade.RetrieveMediator(ShopBuyViewMediator.NAME) as ShopBuyViewMediator;
            mediator.HideView();
        }

        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                AppConst.C_ShowShopBuy,
                AppConst.C_HideShopBuy,
            };
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case AppConst.C_ShowShopBuy:
                    ShowShopBuyArgs showShopBuyArgs = notification.Body as ShowShopBuyArgs;
                    ShowItem(showShopBuyArgs.shopBuyData);
                    break;
                case AppConst.C_HideShopBuy:
                    HideItem();
                    break;
            }
        }

        public void ClearPanel()
        {
            ViewComponent = null;
            shopType = ShopType.buy;
        }
    }
}