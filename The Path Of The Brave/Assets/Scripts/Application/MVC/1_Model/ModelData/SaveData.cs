using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    [SerializeField]
    public class SaveData
    {
        public int currentLevel; // 当前等级
        public int currentExp; // 当前经验
        public int goldNum; // 金币数量
        public int crystalNum; // 晶石数量

        // 角色列表
        public List<CharacterJsonData> characterJsonDataList = new List<CharacterJsonData>();

        // 装备背包列表
        public List<ItemJsonData> equipJsonDataList = new List<ItemJsonData>();

        // 材料背包列表
        public List<ItemJsonData> materialJsonDataList = new List<ItemJsonData>();

        // 挑战关卡状态
        public List<SelectLevelJsonData> levelJsonDataList = new List<SelectLevelJsonData>();

        // 奖励领取状态
        public List<bool> getStarAward = new List<bool>();
    }
}

