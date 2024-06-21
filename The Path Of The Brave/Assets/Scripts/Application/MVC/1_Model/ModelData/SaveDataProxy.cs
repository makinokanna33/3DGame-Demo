using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FrameWork.Base;
using PureMVC.Patterns.Proxy;
using UnityEngine;

namespace MyApplication
{
    [SerializeField]
    public class SaveDataProxy : Proxy
    {
        public new const string NAME = "SaveDataProxy";

        public SaveData MyData
        {
            get { return Data as SaveData; }
        }

        private LevelSOData levelSoData;

        public SaveDataProxy() : base(NAME)
        {
            SaveData saveData = new SaveData();

            // 判断玩家存档目录和文件是否存在，若不存在则进行创建
            string saveDirName = AppConst.SaveDir;
            string saveFileName = AppConst.SaveDir + "/SaveData.json";

            if (!Directory.Exists(saveDirName))
                Directory.CreateDirectory(saveDirName);
            if (!File.Exists(saveFileName))
                File.Create(saveFileName).Close();

            // 读取玩家存档数据
            StreamReader sr = new StreamReader(saveFileName);
            // 利用Json数据填充玩家数据
            var tmp = JsonUtility.FromJson<SaveData>(sr.ReadToEnd());
            if (tmp != null)
            {
                saveData = tmp;
            }
            else
            {
                // 设定玩家初始值
                saveData.currentLevel = 1;
                saveData.currentExp = 100;
                saveData.goldNum = 3000;
                saveData.crystalNum = 1000;

                // 初始为玩家分配一个角色
                CharacterJsonData characterJsonData = new CharacterJsonData(0);
                saveData.characterJsonDataList.Add(characterJsonData);

                // 初始化玩家的关卡挑战进度
                saveData.levelJsonDataList.Clear();
                SelectLevelJsonData selectLevelJsonData1 = new SelectLevelJsonData() // 第一关默认解锁
                {
                    id = 0,
                    isLock = false,
                    startNum = 0
                };
                saveData.levelJsonDataList.Add(selectLevelJsonData1);
                for (int i = 1; i < 2; ++i)
                {
                    SelectLevelJsonData selectLevelJsonData = new SelectLevelJsonData()
                    {
                        id = i,
                        isLock = true,
                        startNum = 0
                    };
                    saveData.levelJsonDataList.Add(selectLevelJsonData);
                }

                // 初始化奖励领取状态
                saveData.getStarAward.Clear();
                for (int i = 0; i < 2; ++i)
                {
                    saveData.getStarAward.Add(false);
                }
            }

            sr.Close();

            // 读取 Level 配置
            // 获取 ScriptableObject 资源
#if UNITY_EDITOR
            levelSoData = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelSOData>("Assets/ABRes/Configuration/LevelConfiguration.asset");
#else
            levelSoData = ABManager.Instance.LoadRes<LevelSOData>(AppConst.AB_Config, AppConst.AB_LevelConfiguration);
#endif

            Data = saveData;
        }

        // 获取下一级所需的经验
        public int GetNextLvExp(int currentLv)
        {
            if(currentLv >= levelSoData.maxLevel)
            {
                return 0;
            }
            else
            {
                return levelSoData.exp[currentLv - 1];
            }
        }

        public int GetGoldNum()
        {
            return MyData.goldNum;
        }
        public int GetCrystalNum()
        {
            return MyData.crystalNum;
        }

        public int GetItemNum(int id, ItemType itemType)
        {
            if (itemType == ItemType.Equip)
            {
                foreach (var item in MyData.equipJsonDataList)
                {
                    if (item.id == id)
                    {
                        return item.count;
                    }
                }

                return 0;
            }
            else if (itemType == ItemType.Material)
            {
                foreach (var item in MyData.materialJsonDataList)
                {
                    if (item.id == id)
                    {
                        return item.count;
                    }
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }

        public List<ItemJsonData> GetItemJsonDatasByType(ItemType itemType)
        {
            if(itemType == ItemType.Equip)
            {
                return MyData.equipJsonDataList;
            }
            else if(itemType == ItemType.Material)
            {
                return MyData.materialJsonDataList;
            }
            else
            {
                return null;
            }
        }

        public int GetCharacterJewelryId(int characterId)
        {
            int result = -1;
            foreach (var item in MyData.equipJsonDataList)
            {
                if (item.ownerId == characterId)
                {
                    result = item.id;
                    break;
                }
            }
            return result;
        }

        public List<bool> GetStarAward()
        {
            return MyData.getStarAward;
        }

        public int GetHasStarNum()
        {
            int result = 0;

            foreach (var t in MyData.levelJsonDataList)
            {
                result += t.startNum;
            }

            return result;
        }

        public void AddGoldNum(int num)
        {
            MyData.goldNum += num;
        }

        public void AddPlayerExp(int expNum)
        {
            MyData.currentExp += expNum;
            int nextExp = GetNextLvExp(MyData.currentLevel);
            while(MyData.currentExp >= nextExp)
            {
                MyData.currentLevel++;
                MyData.currentExp -= nextExp;
                nextExp = GetNextLvExp(MyData.currentLevel);
            }
        }

        public void AddAllCharacterExp(int expNum)
        {
            foreach (var characterJsonData in MyData.characterJsonDataList)
            {
                characterJsonData.currentExp += expNum;
                int nextExp = GetNextLvExp(characterJsonData.level);
                while (characterJsonData.currentExp >= nextExp)
                {
                    characterJsonData.level++;
                    characterJsonData.currentExp -= nextExp;
                    nextExp = GetNextLvExp(characterJsonData.level);
                }
            }
        }

        public void AddItem(ItemData itemData, int count)
        {
            if (itemData.configData.tabType == ItemType.Equip)
            {
                ItemJsonData itemJsonData = new ItemJsonData
                {
                    id = itemData.configData.id,
                    level = 1,
                    ownerId = -1,
                    count = 1
                };
                MyData.equipJsonDataList.Add(itemJsonData);
            }
            else if(itemData.configData.tabType == ItemType.Material)
            {
                bool flag = false;
                foreach (var item in MyData.materialJsonDataList)
                {
                    if(item.id == itemData.configData.id)
                    {
                        flag = true;
                        item.count += count;
                        break;
                    }
                }
                if(flag == false)
                {
                    ItemJsonData itemJsonData = new ItemJsonData
                    {
                        id = itemData.configData.id,
                        level = 1,
                        ownerId = -1,
                        count = count
                    };
                    MyData.materialJsonDataList.Add(itemJsonData);
                }
            }
        }

        public void AddItem(int itemId, ItemType itemType, int count)
        {
            if (itemType == ItemType.Equip)
            {
                ItemJsonData itemJsonData = new ItemJsonData
                {
                    id = itemId,
                    level = 1,
                    ownerId = -1,
                    count = 1
                };
                MyData.equipJsonDataList.Add(itemJsonData);
            }
            else if (itemType == ItemType.Material)
            {
                bool flag = false;
                foreach (var item in MyData.materialJsonDataList)
                {
                    if (item.id == itemId)
                    {
                        flag = true;
                        item.count += count;
                        break;
                    }
                }
                if (flag == false)
                {
                    ItemJsonData itemJsonData = new ItemJsonData
                    {
                        id = itemId,
                        level = 1,
                        ownerId = -1,
                        count = count
                    };
                    MyData.materialJsonDataList.Add(itemJsonData);
                }
            }
        }

        public void DelItem(ItemData itemData, int count, int gridIndex)
        {
            if (itemData.configData.tabType == ItemType.Equip)
            {
                MyData.equipJsonDataList.RemoveAt(gridIndex);
            }
            else if (itemData.configData.tabType == ItemType.Material)
            {
                foreach (var item in MyData.materialJsonDataList)
                {
                    if (item.id == itemData.configData.id)
                    {
                        item.count -= count;
                        if(item.count <= 0)
                        {
                            MyData.materialJsonDataList.RemoveAt(gridIndex);
                        }
                        break;
                    }
                }
            }
        }

        public void DelItem(ItemType itemType, int itemId, int count)
        {
            if (itemType == ItemType.Equip)
            {
                foreach (var itemJsonData in MyData.equipJsonDataList)
                {
                    if (itemJsonData.id == itemId)
                    {
                        MyData.equipJsonDataList.Remove(itemJsonData);
                        break;
                    }
                }
            }
            else if (itemType == ItemType.Material)
            {
                foreach (var item in MyData.materialJsonDataList)
                {
                    if (item.id == itemId)
                    {
                        item.count -= count;
                        if (item.count <= 0)
                        {
                            MyData.materialJsonDataList.Remove(item);
                        }
                        break;
                    }
                }
            }
        }

        // 玩家添加新角色
        public void AddCharacter(int characterId)
        {
            CharacterJsonData characterJsonData = new CharacterJsonData(characterId);
            MyData.characterJsonDataList.Add(characterJsonData);
        }

        // 角色装备升级
        public void CharacterItemLvUp(int characterId, CharacterItemType type)
        {
            foreach (var characterJsonData in MyData.characterJsonDataList)
            {
                if (characterJsonData.id == characterId)
                {
                    if (type == CharacterItemType.cap)
                    {
                        characterJsonData.capLevel++;
                    }
                    else if (type == CharacterItemType.armor)
                    {
                        characterJsonData.armorLevel++;
                    }
                }
            }
        }

        // 角色技能解锁
        public void CharacterSkillLock(int characterId, int skillIndex)
        {
            foreach (var characterJsonData in MyData.characterJsonDataList)
            {
                if (characterJsonData.id == characterId)
                {
                    characterJsonData.skillUnLock[skillIndex] = true;
                }
            }
        }

        public void GetStar(int index, int startNum)
        {
            MyData.levelJsonDataList[index].startNum = Math.Max(MyData.levelJsonDataList[index].startNum, startNum);
            if (MyData.levelJsonDataList[index].startNum > 0 && index < 1)
            {
                MyData.levelJsonDataList[index + 1].startNum = 0;
                MyData.levelJsonDataList[index + 1].isLock = false;
            }
        }

        // 已获取星星挑战奖励
        public void SetStarAwardStatus(int index)
        {
            MyData.getStarAward[index] = true;
        }

        public void SaveDate()
        {
            string jsonString = JsonUtility.ToJson(MyData, true);

            // 判断玩家存档目录和文件是否存在，若不存在则进行创建
            string saveDirName = AppConst.SaveDir;
            string saveFileName = AppConst.SaveDir + "/SaveData.json";

            if (!Directory.Exists(saveDirName))
                Directory.CreateDirectory(saveDirName);
            if (!File.Exists(saveFileName))
                File.Create(saveFileName).Close();

            StreamWriter sw = new StreamWriter(saveFileName);

            sw.Write(jsonString);

            sw.Close();
        }
    }
}

