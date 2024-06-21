using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class CharacterItemLvUpCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            CharacterLvUpArgs characterArgs = notification.Body as CharacterLvUpArgs;
            int characterId = characterArgs.id;
            CharacterItemType type = characterArgs.type;

            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            var data = characterDataProxy.FindCharacterData(characterId, CharacterCamp.Player);
            int consumeUpNum = 0;
            if (type == CharacterItemType.cap)
            {
                consumeUpNum = data.capItemData.level * characterArgs.addNum;
            }
            else if (type == CharacterItemType.armor)
            {
                consumeUpNum = data.armorItemData.level * characterArgs.addNum;
            }

            // 更新存档数据
            saveDataProxy.DelItem(characterArgs.upItem.itemConfigData.tabType, characterArgs.upItem.itemConfigData.id,
                consumeUpNum);
            saveDataProxy.CharacterItemLvUp(characterId, type);

            // 更新内存数据
            characterDataProxy.CharacterItemLvUp(characterId, type);

            // 更新 UI 面板
            SendNotification(AppConst.C_UpdateInventory);
            SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(characterId));
        }
    }
}
