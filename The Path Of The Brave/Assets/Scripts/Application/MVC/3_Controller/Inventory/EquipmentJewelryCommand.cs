using System.Collections;
using System.Collections.Generic;
using MyApplication;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class EquipmentJewelryCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);

            AddItemOwnerArgs args = notification.Body as AddItemOwnerArgs;

            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            ItemDataProxy itemDataProxy = Facade.RetrieveProxy(ItemDataProxy.NAME) as ItemDataProxy;

            // 查找之前佩戴该饰品的角色
            int oldOwnerId = saveDataProxy.MyData.equipJsonDataList[args.gridIndex].ownerId;
            if (oldOwnerId != -1)
            {
                // 原本持有该饰品的角色卸下该饰品
                foreach (var item in characterDataProxy.playerDataList)
                {
                    if (item.configData.id == oldOwnerId)
                    {
                        item.jewelryData = null;
                        item.UpdateCharacterData();
                    }
                }
            }

            // 清除角色之前持有的饰品
            foreach (var item in saveDataProxy.MyData.equipJsonDataList)
            {
                if (item.ownerId == args.ownerId)
                {
                    item.ownerId = -1;
                }
            }
            // 更换新的佩戴饰品角色
            saveDataProxy.MyData.equipJsonDataList[args.gridIndex].ownerId = args.ownerId;

            // 新的角色装备该饰品
            var jewelryData = itemDataProxy.CreateItemData(args.itemData.configData.id, 1, 1, args.ownerId);
            foreach (var item in characterDataProxy.playerDataList)
            {
                if (item.configData.id == args.ownerId)
                {
                    item.jewelryData = jewelryData;
                    item.UpdateCharacterData();
                }
            }
            // 更新 UI
            SendNotification(AppConst.C_UpdateCharacterInfo);
        }
    }
}


