using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public enum CharacterCamp // 阵营
    {
        None, Player, Enemy
    }

    public enum PropertyKey
    {
        Hp,
        Atk,
        Def
    }


    [System.Serializable]
    public class CharacterJsonData
    {
        public int id; // ID
        public int level; // 等级
        public int currentExp; // 当前经验
        public int capLevel; // 帽子等级
        public int armorLevel; // 护甲等级
        public List<bool> skillUnLock = new List<bool>(); // 技能解锁状态

        public CharacterJsonData()
        {

        }

        public CharacterJsonData(int id)
        {
            this.id = id;
            level = 1;
            currentExp = 0;
            capLevel = 1;
            armorLevel = 1;

            CharacterDataProxy characterDataProxy = GameFacade.Instance.RetrieveProxy(CharacterDataProxy.NAME) as CharacterDataProxy;
            var soData = characterDataProxy.FindCharacterDataSo(id);
            if (soData)
            {
                skillUnLock.Clear();
                for (int i = 0; i < soData.configData.skillSoList.Count; ++i)
                {
                    skillUnLock.Add(false);
                }
            }

        }
    }

    [System.Serializable]
    public class CharacterConfigData
    {
        public int id; // ID
        public string name; // 名字
        public Sprite sprite; // 头像
        public GameObject pfCharacter; // 角色预制体

        
        public int addHp; // HP增量
        public int addDef; // 防御力增量
        public int addAtk; // 攻击力增量

        public ItemBaseSOData cap;
        public ItemBaseSOData armor;

        [Header("脚本配置内容")]
        public int baseHp; // 基础HP
        public int baseDef; // 基础防御力
        public int baseAtk; // 基础攻击力
        public int moveRange; // 移动范围
        public int minAtkRange; // 最小攻击距离
        public int maxAtkRange; // 最大攻击距离
        public bool canSwim; // 是否能游泳

        public List<SkillSOData> skillSoList;
    }

    [System.Serializable]
    public class CharacterData
    {
        #region 字段
        public CharacterConfigData configData;
        public List<bool> skillUnLock = new List<bool>(); // 技能解锁情况

        public int level;
        public int currentExp; // 当前经验
        public ItemData capItemData;
        public ItemData armorItemData;
        public ItemData jewelryData;

        public CharacterCamp camp; // 阵容

        public Vector3 startMovePos; // 开始位置

        public int maxHp, currentHp; // HP
        public int maxDef, currentDef; // 防御力
        public int maxAtk, currentAtk; // 攻击力

        public List<SkillData> skillDataList = new List<SkillData>(); // 技能数据
        #endregion

        #region 属性
        public int MoveRange
        {
            get => configData.moveRange;
            set => configData.moveRange = value;
        }
        public bool CanSwim
        {
            get => configData.canSwim;
            set => configData.canSwim = value;
        }

        public CharacterConfigData ConfigData
        {
            get => configData;
            set => configData = value;
        }

        #endregion

        public CharacterData(CharacterConfigData configData, int level, int currentExp, ItemData capItemData,
            ItemData armorItemData, ItemData jewelryData, List<bool> skillUnLock, CharacterCamp camp)
        {
            this.configData = configData;
            this.camp = camp;
            this.level = level;
            this.currentExp = currentExp;
            this.capItemData = capItemData;
            this.armorItemData = armorItemData;
            this.jewelryData = jewelryData;

            UpdateCharacterData(level);
            UpdateCharacterSkillState(skillUnLock);
        }

        public void UpdateCharacterData(int level)
        {
            maxHp = this.configData.baseHp + (level - 1) * this.configData.addHp +
                    capItemData.maxHpUp + armorItemData.maxHpUp + (jewelryData != null ? jewelryData.maxHpUp : 0);
            maxDef = this.configData.baseDef + (level - 1) * this.configData.addDef +
                     capItemData.maxDefUp + armorItemData.maxDefUp + (jewelryData != null ? jewelryData.maxDefUp : 0);
            maxAtk = this.configData.baseAtk + (level - 1) * this.configData.addAtk +
                     capItemData.maxAtkUp + armorItemData.maxAtkUp + (jewelryData != null ? jewelryData.maxAtkUp : 0);

            InitBattleData();
        }

        public void UpdateCharacterData()
        {
            maxHp = this.configData.baseHp + (level - 1) * this.configData.addHp +
                    capItemData.maxHpUp + armorItemData.maxHpUp + (jewelryData != null ? jewelryData.maxHpUp : 0);
            maxDef = this.configData.baseDef + (level - 1) * this.configData.addDef +
                     capItemData.maxDefUp + armorItemData.maxDefUp + (jewelryData != null ? jewelryData.maxDefUp : 0);
            maxAtk = this.configData.baseAtk + (level - 1) * this.configData.addAtk +
                     capItemData.maxAtkUp + armorItemData.maxAtkUp + (jewelryData != null ? jewelryData.maxAtkUp : 0);

            InitBattleData();
        }

        public void InitBattleData()
        {
            currentHp = maxHp;
            currentDef = maxDef;
            currentAtk = maxAtk;
        }

        public void UpdateCharacterSkillState(List<bool> skillUnLock)
        {
            this.skillUnLock = skillUnLock;
        }

        // 初始化技能数据
        public void InitSkillData()
        {
            if (skillDataList == null)
                skillDataList = new List<SkillData>();
            else
                skillDataList.Clear();

            for (int i = 0; i < skillUnLock.Count; ++i)
            {
                if (skillUnLock[i])
                {
                    SkillData skillData = new SkillData(configData.skillSoList[i].configData);
                    skillDataList.Add(skillData);
                }
            }
        }

}
}


