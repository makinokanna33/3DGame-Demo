using System.Collections;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine;

namespace MyApplication
{
    public class GetStarAwardCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            int starAwardId = (int)notification.Body;

            StarAwardDataProxy awardDataProxy = Facade.RetrieveProxy(StarAwardDataProxy.NAME) as StarAwardDataProxy;
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            var configData = awardDataProxy.GetConfigData(starAwardId);

            // 增加资源
            saveDataProxy.AddPlayerExp(configData.playerAddExp);
            saveDataProxy.AddAllCharacterExp(configData.characterAddExp);
            saveDataProxy.AddGoldNum(configData.addGoldNum);

            // 增加装备
            for (int i = 0; i < configData.trophyItem.Count; i++)
            {
                saveDataProxy.AddItem(configData.trophyItem[i].itemConfigData.id,
                    configData.trophyItem[i].itemConfigData.tabType, configData.trophyNum[i]);
            }

            // 修改存档数据
            saveDataProxy.SetStarAwardStatus(starAwardId);

            // 修改内存数据
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            characterDataProxy.ClearCharacterData(CharacterCamp.Player);
            foreach (var item in saveDataProxy.MyData.characterJsonDataList)
            {
                characterDataProxy.InitCharacterData(item.id, item.level, item.currentExp, item.capLevel, item.armorLevel, item.skillUnLock, CharacterCamp.Player);
            }

            // 展示确认 UI
            SendNotification(AppConst.C_ShowAwardPanel, configData);

            // 更新星星挑战进度 UI
            SendNotification(AppConst.C_UpdateStarStatus);
        }
    }

    public class GetLevelChallengeWinCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var battleWinArgs = notification.Body as BattleWinArgs;

            SelectLevelDataProxy selectLevelDataProxy = battleWinArgs.dataProxy;
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;

            var configData = selectLevelDataProxy.GetSelectLevelData(battleWinArgs.awardId);

            // 增加资源
            saveDataProxy.AddPlayerExp(configData.playerAddExp);
            saveDataProxy.AddAllCharacterExp(configData.characterAddExp);
            saveDataProxy.AddGoldNum(configData.addGoldNum);

            // 增加装备
            for (int i = 0; i < configData.trophyItem.Count; i++)
            {
                saveDataProxy.AddItem(configData.trophyItem[i].itemConfigData.id,
                    configData.trophyItem[i].itemConfigData.tabType, configData.trophyNum[i]);
            }

            // 修改存档数据
            int starNum = 1;
            foreach (var b in battleWinArgs.isComplete)
            {
                if (b)
                {
                    starNum++;
                }
            }
            saveDataProxy.GetStar(battleWinArgs.awardId, starNum);

            // 修改内存数据
            CharacterDataProxy characterDataProxy = Facade.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            characterDataProxy.ClearCharacterData(CharacterCamp.Player);
            foreach (var item in saveDataProxy.MyData.characterJsonDataList)
            {
                characterDataProxy.InitCharacterData(item.id, item.level, item.currentExp, item.capLevel, item.armorLevel, item.skillUnLock, CharacterCamp.Player);
            }

            // 展示确认 UI
            SendNotification(AppConst.C_ShowLevelChallengeWin, battleWinArgs);
        }
    }

}