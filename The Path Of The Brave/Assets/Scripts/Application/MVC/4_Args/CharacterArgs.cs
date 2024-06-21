using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyApplication
{
    public class CharacterArgs
    {
        // 角色 id
        public int id;

        public CharacterArgs(int id)
        {
            this.id = id;
        }
    }

    public class CharacterLvUpArgs
    {
        // 角色 id
        public int id;
        public CharacterItemType type;
        public ItemBaseSOData upItem;
        public int addNum;

        public CharacterLvUpArgs(int id, CharacterItemType type, ItemBaseSOData upItem, int addNum)
        {
            this.id = id;
            this.type = type;
            this.upItem = upItem;
            this.addNum = addNum;
        }
    }

    public class AddItemOwnerArgs
    {
        public ItemData itemData;
        public int gridIndex;
        public int ownerId;
        public AddItemOwnerArgs(ItemData itemData, int gridIndex, int ownerId)
        {
            this.itemData = itemData;
            this.gridIndex = gridIndex;
            this.ownerId = ownerId;
        }
    }

    public class CharacterSkillLockArgs
    {
        public int id;
        public int skillIndex;
        public ItemBaseSOData lockItem;
        public int lockNum;

        public CharacterSkillLockArgs(int id, int skillIndex, ItemBaseSOData lockItem, int lockNum)
        {
            this.id = id;
            this.skillIndex = skillIndex;
            this.lockItem = lockItem;
            this.lockNum = lockNum;
        }
    }
}
