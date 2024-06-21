using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MyApplication
{
    public enum ItemType
    {
        Equip,
        Material,
    }

    public enum CharacterItemType
    {
        cap,
        armor,
    }

    [System.Serializable]
    public class ItemJsonData
    {
        public int id; // ID
        public int level; // 等级
        public int ownerId; // 所属者
        public int count; // 数量
    }

    [System.Serializable]
    public class ItemConfigData
    {
        [Tooltip("物品ID")]
        public int id;

        [Tooltip("物品名称")]
        public string itemName;

        [Tooltip("图标")]
        public Sprite icon;

        [Tooltip("是否可堆叠")]
        public bool isStackUp;

        [Tooltip("物品描述")]
        [TextArea] public string description;

        [Tooltip("出售价格")]
        public int sellPrice;

        [Tooltip("物品类别")]
        public ItemType tabType;

        [Tooltip("HP加成")]
        public int hpUp;

        [Tooltip("HP成长")]
        public int hpAdd;

        [Tooltip("攻击力加成")]
        public int atkUp;

        [Tooltip("攻击力成长")]
        public int atkAdd;

        [Tooltip("防御力加成")]
        public int defUp;

        [Tooltip("防御力成长")]
        public int defAdd;
    }

    [System.Serializable]
    public class ItemData
    {
        public ItemConfigData configData;

        public int level; // 装备等级

        public int count; // 物品数量
        public int ownerId; // 持有者 ID

        public int maxHpUp, currentHpUp;
        public int maxAtkUp, currentAtkUp;
        public int maxDefUp, currentDefUp;

        public ItemData(ItemConfigData itemConfigData, int level, int count, int ownerId = -1)
        {
            this.configData = itemConfigData;

            this.level = level;
            this.count = count;
            this.ownerId = ownerId;

            UpdateItemData(level);
        }

        public void UpdateItemData(int level)
        {
            this.level = level;

            maxHpUp = this.configData.hpUp + (level - 1) * this.configData.hpAdd;
            maxDefUp = this.configData.defUp + (level - 1) * this.configData.defAdd;
            maxAtkUp = this.configData.atkUp + (level - 1) * this.configData.atkAdd;

            InitBattleData();
        }

        public void InitBattleData()
        {
            currentHpUp = maxHpUp;
            currentDefUp = maxDefUp;
            currentAtkUp = maxAtkUp;
        }

        public string GetMessage()
        {
            StringBuilder sb = new StringBuilder();

            // 名称
            sb.AppendLine(string.Format("<color=#0000ff>{0}</color>", configData.itemName));

            // 添加类别信息
            sb.Append("");
            string itemTypeName = "";
            if (configData.tabType == ItemType.Equip)
                itemTypeName = "装备";
            else if (configData.tabType == ItemType.Material)
                itemTypeName = "材料";
            sb.AppendLine(string.Format("<color=grey>[{0}]</color>", itemTypeName));

            if (maxHpUp != 0 || maxAtkUp != 0 || maxDefUp != 0)
                sb.AppendLine();
            if (maxHpUp != 0)
                sb.AppendLine(string.Format("<color=green>生命值：{0}</color>", maxHpUp));
            if(maxAtkUp != 0)
                sb.AppendLine(string.Format("<color=red>攻击力：{0}</color>", maxAtkUp));
            if(maxDefUp != 0)
                sb.AppendLine(string.Format("<color=yellow>防御力：{0}</color>", maxDefUp));

            if (!configData.description.Equals(""))
            {
                sb.AppendLine(string.Format("\n<color=grey>{0}</color>", configData.description));
            }

            sb.Append(string.Format("\n<color=#cd7f32>出售价格：{0}</color>", configData.sellPrice));

            return sb.ToString();
        }
    }
}
