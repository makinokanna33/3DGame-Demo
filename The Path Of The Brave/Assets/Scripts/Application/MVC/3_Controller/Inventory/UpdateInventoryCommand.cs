using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class UpdateInventoryCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            // 获取背包中介
            InventoryViewMediator mediator;

            if (Facade.HasMediator(InventoryViewMediator.NAME))
                mediator = Facade.RetrieveMediator(InventoryViewMediator.NAME) as InventoryViewMediator;
            else
                return;

            // 获取数据代理
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            ItemDataProxy itemDataProxy = Facade.RetrieveProxy(ItemDataProxy.NAME) as ItemDataProxy;
            
            var itemList = saveDataProxy.GetItemJsonDatasByType(mediator.NowTabType);

            if (itemList == null)
            {
                LoggerManager.Instance.LogError("玩家背包数据提取为空，请检查玩家数据或背包类型是否出错！");
                return;
            }

            // 隐藏物品
            SendNotification(AppConst.C_HideInventoryItem);

            // 展示物品
            for (int i = 0; i < itemList.Count; ++i)
            {
                ItemData itemData = itemDataProxy.CreateItemData(itemList[i].id, itemList[i].level, itemList[i].count, itemList[i].ownerId);
                SendNotification(AppConst.C_ShowInventoryItem, new ShowItemArgs(i, itemData));
            }
        }
    }
}