using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class ShopSellSureCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            ShopSellArgs shopArgs = notification.Body as ShopSellArgs;

            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;

            // 增加玩家货币
            var shopData = shopArgs.itemData;

            saveDataProxy.MyData.goldNum += shopData.configData.sellPrice * shopArgs.count;

            // 删除内存数据
            characterDataProxy.RemoveJewelry(shopData, shopArgs.gridIndex);

            // 删除玩家物品
            saveDataProxy.DelItem(shopData, shopArgs.count, shopArgs.gridIndex);

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
