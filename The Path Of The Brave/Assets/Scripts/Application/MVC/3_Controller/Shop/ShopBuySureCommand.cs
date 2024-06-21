using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShopBuySureCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            ShopArgs shopArgs = notification.Body as ShopArgs;

            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            // 扣除玩家货币
            var shopData = shopArgs.shopBuyData;

            if(shopData.configData.buyType == ShopBuyType.Gold)
            {
                saveDataProxy.MyData.goldNum -= shopData.configData.buyCount;
            }
            else if(shopData.configData.buyType == ShopBuyType.Crystal)
            {
                saveDataProxy.MyData.crystalNum -= shopData.configData.buyCount;
            }

            // 增加玩家物品
            saveDataProxy.AddItem(shopData.itemData, 1);

            // 更新玩家信息 UI
            SendNotification(AppConst.C_UpdatePlayerInfo);
            // 更新背包 UI
            SendNotification(AppConst.C_UpdateInventory);
            // 更新商店 UI
            SendNotification(AppConst.C_UpdateShop);
            // 更新队伍 UI
            SendNotification(AppConst.C_UpdateCharacterInfo);
        }
    }
}
