using System.Collections;
using System.Collections.Generic;
using FrameWork.Base;
using MyApplication;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    public class CharacterDataProxy : Proxy
    {
        public new const string NAME = "CharacterDataProxy";

        public List<CharacterData> playerDataList = new List<CharacterData>();
        public List<CharacterData> enemyDataList = new List<CharacterData>();

        private CharactersSOData charactersSoData;
        public CharacterDataProxy() : base(NAME)
        {
            // 读取 Characters 配置
            // 获取所有的 ScriptableObject 资源
#if UNITY_EDITOR
            charactersSoData = UnityEditor.AssetDatabase.LoadAssetAtPath<CharactersSOData>("Assets/ABRes/Configuration/CharactersConfiguration.asset");
#else
            charactersSoData = ABManager.Instance.LoadRes<CharactersSOData>(AppConst.AB_Config, AppConst.AB_CharactersConfiguration);
#endif

        }

        public void InitCharacterData(int id, int level, int currentExp, int capLevel, int armorLevel, List<bool> skillUnlock, CharacterCamp camp)
        {
            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            foreach (var item in charactersSoData.configDataList)
            {
                if (item.configData.id == id)
                {
                    // 生成装备内存数据
                    ItemDataProxy itemDataProxy = Facade.RetrieveProxy(ItemDataProxy.NAME) as ItemDataProxy;
                    var capData = itemDataProxy.CreateItemData(item.configData.cap.itemConfigData.id, capLevel, 1, id);
                    var armorData = itemDataProxy.CreateItemData(item.configData.armor.itemConfigData.id, armorLevel, 1, id);
                    var jewelryData = itemDataProxy.CreateItemData(saveDataProxy.GetCharacterJewelryId(id), 1, 1, id);

                    CharacterData tmp = new CharacterData(item.configData, level, currentExp, capData, armorData, jewelryData, skillUnlock, camp);
                    if (camp == CharacterCamp.Player)
                    {
                        playerDataList.Add(tmp);
                    }
                    else if(camp == CharacterCamp.Enemy)
                    {
                        enemyDataList.Add(tmp);
                    }
                }
            }
        }

        public CharacterData FindCharacterData(int id, CharacterCamp camp)
        {
            CharacterData tmp = null;
            foreach (var item in playerDataList)
            {
                if(item.configData.id == id && item.camp == camp)
                {
                    tmp = item;
                    break;
                }
            }
            return tmp;
        }

        public void CharacterItemLvUp(int id, CharacterItemType type)
        {
            foreach (var item in playerDataList)
            {
                if (item.configData.id == id)
                {
                    if (type == CharacterItemType.cap)
                    {
                        item.capItemData.UpdateItemData(++item.capItemData.level);
                    }
                    else if(type == CharacterItemType.armor)
                    {
                        item.armorItemData.UpdateItemData(++item.armorItemData.level);
                    }
                    item.UpdateCharacterData();
                    break;
                }
            }
        }

        public void CharacterSkillLock(int id, int skillIndex)
        {
            foreach (var item in playerDataList)
            {
                if (item.configData.id == id)
                {
                    item.skillUnLock[skillIndex] = true;
                    break;
                }
            }
        }

        public void RemoveJewelry(ItemData itemData, int gridIndex)
        {
            if(itemData.configData.tabType != ItemType.Equip)
                return;

            SaveDataProxy saveDataProxy = Facade.RetrieveProxy(SaveDataProxy.NAME) as SaveDataProxy;
            int ownerId = saveDataProxy.MyData.equipJsonDataList[gridIndex].ownerId;

            foreach (var item in playerDataList)
            {
                if (item.configData.id == ownerId)
                {
                    item.jewelryData = null;
                    item.UpdateCharacterData();
                    break;
                }
            }
        }

        public void ClearCharacterData(CharacterCamp camp)
        {
            if (camp == CharacterCamp.Player)
            {
                playerDataList.Clear();
            }
            else if (camp == CharacterCamp.Enemy)
            {
                enemyDataList.Clear();
            }
        }

        // 查找数据库中的玩家数据配置
        public CharacterSOData FindCharacterDataSo(int id)
        {
            foreach (var item in charactersSoData.configDataList)
            {
                if (item.configData.id == id)
                {
                    return item;
                }
            }

            return null;
        }
    }
}


