using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class CharacterItemSkillLockCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            CharacterSkillLockArgs args = notification.Body as CharacterSkillLockArgs;

            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            // 更新存档数据
            saveDataProxy.DelItem(args.lockItem.itemConfigData.tabType, args.lockItem.itemConfigData.id,
                args.lockNum);
            saveDataProxy.CharacterSkillLock(args.id, args.skillIndex);

            // 更新内存数据
            characterDataProxy.CharacterSkillLock(args.id, args.skillIndex);

            // 更新 UI 面板
            SendNotification(AppConst.C_UpdateInventory);
            SendNotification(AppConst.C_UpdateCharacterInfo, new CharacterArgs(args.id));
        }
    }
}