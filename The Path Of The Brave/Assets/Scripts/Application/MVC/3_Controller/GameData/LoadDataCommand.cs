using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class LoadDataCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;

            characterDataProxy.ClearCharacterData(CharacterCamp.Player);

            // 生成玩家拥有的角色数据信息
            foreach (var item in saveDataProxy.MyData.characterJsonDataList)
            {
                characterDataProxy.InitCharacterData(item.id, item.level, item.currentExp, item.capLevel, item.armorLevel, item.skillUnLock, CharacterCamp.Player);
            }
        }
    }
}


