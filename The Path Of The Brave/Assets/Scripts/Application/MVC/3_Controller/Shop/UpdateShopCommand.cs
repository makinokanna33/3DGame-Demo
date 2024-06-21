using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class UpdateShopCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            // 获取商店中介
            ShopViewMediator mediator;

            if (Facade.HasMediator(ShopViewMediator.NAME))
                mediator = Facade.RetrieveMediator(ShopViewMediator.NAME) as ShopViewMediator;
            else
                return;

            ShopBuyDataProxy shopBuyDataProxy = Facade.RetrieveProxy(ShopBuyDataProxy.NAME) as ShopBuyDataProxy;

            // 根据页签显示不同的内容
            mediator.ShowShopContainer();

            // 隐藏物品
            SendNotification(AppConst.C_HideShopBuy);

            // 若是购买界面，根据列表遍历，生成商品列表
            if(mediator.shopType == ShopType.buy)
            {
                ShopBuyData buyData;
                foreach (var item in shopBuyDataProxy.soData.datas)
                {
                    buyData = new ShopBuyData(item);
                    SendNotification(AppConst.C_ShowShopBuy, new ShowShopBuyArgs(buyData));
                }
            }
        }
    }
}
